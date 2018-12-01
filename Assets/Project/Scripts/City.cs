using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {
    // balancing values
    public int moneyPerJob = 2;                         // amount of money gained per one working people on factory
    public float foodPerFarm = 4f;                      // amount of produced food per one farm
    public int jobsPerFactory = 10;                     // number of jobs per one factory - extend JobCeiling
    public int houseCapacity = 5;                       // number of people per one house - extend PopulationCeiling
    public float foodConsumingPerOnePeople = 0.5f;      // amount of food consumed per one people per one Day
    public float foodFromOnePeole = 2.5f;               // food gain after one people has to be eaten
    public float foodSafety = 2f;                       // safety multiplier for people population increasing
    public float badMoralePenalty = 0.33f;              // penalty to Money increasing from canibalizm
    public float goodMoraleBonus = 1.2f;                // bonus to Money increasing 
    public float staticPopulationRate = 0.025f;         // passive value for population incresing if there enough food
    public int canibalizmCooldown = 3;                  // 3 days cooldown before morale stabilize after canibalizm act

    public int Day { get; set; }

    // calculated values
    public int Money { get; set; }                      // available amount of money = JobCurrent * moneyPerJob (max 1000000)
    public float Income { get; set; }                   // amount of money earned per Day
    public float PopulationCurrent { get; set; }        // current amount of people
    public int PopulationCeiling { get; set; }          // #house * houseCapacity (max 1000000)
    public int JobCurrent { get; set; }                 // number of working people
    public int JobCeiling { get; set; }                 // #factory * jobsPerFactory (max 999998)
    public float Food { get; set; }                     // available amount of food = #farm * foodPerFarm (max 1000000)
    public float FoodIncome { get; set; }               // amount of food earned per Day
    public float FoodConsuming { get; set; }            // amount of food consuming per Day = PopulationCurrent * foodConsumingPerOnePeople

    // buildings storing array
    public int[] buildingCounts = new int[4];            // 0 - road, 1 - house, 2 - farm, 3 - factory

    // using for buildingCounts access to appropriate values
    public const int house = 1;
    public const int farm = 2;
    public const int factory = 3;

    //temporary effects switch
    private bool badMorale = false;                     // true - bad morale effect active - redure Money income
    private bool goodMorale = false;                    // true - good morale effect active - increase Money income
    private bool canibalizm = false;                    // true - if Food < FoodConsuming and people were eaten     // todo: if we need it?

    private int canibalizmPenaltiTimer = 0;             // on '0' - bad morale after canibalizm is gone

    // maximim values
    private const int maxMoney = 1000000;
    private const int maxIncome = 99998;
    private const float maxFood = 1000000f;
    private const float maxFoodIncome = 50000f;
    private const int maxPopulation = 1000000;
    private const int maxJobs = 999900;

    private UIController uIController;

    // Use this for initialization
    void Start () {
        uIController = FindObjectOfType<UIController>();
        PopulationCeiling = 1;
        PopulationCurrent = 1;
        Money = 500;

        uIController.UpdateMoney(Money);
        uIController.UpdateDay();
        uIController.UpdateFood();
        uIController.UpdatePopulation(PopulationCurrent, PopulationCeiling);
        uIController.UpdateJob();
    }

    public void EndTurn() {                                         // called by OnClick() - TurnButton
        if (PopulationCeiling == 0) {
            PopulationCeiling = 1;
            PopulationCurrent = 1;
        }
        Day++;
        CalculateFoodConsuming();
        CalculateJobs();
        CalculateMoney();
        CalculateFoodFromFarm();
        CalculatePopulation();
        if (canibalizm) {
            CalculateJobs();
            CalculateFoodConsuming();
            canibalizm = false;
        }
        MoraleReview();
        textUpdate();
    }

    public int CalculatePopulationCeiling() {                       // called only if house was built
        PopulationCeiling = buildingCounts[house] * houseCapacity;
        if (PopulationCeiling > maxPopulation)
            PopulationCeiling = maxPopulation;
        if (PopulationCeiling < PopulationCurrent)
            CalculatePopulation(true);
		uIController.UpdatePopulation(PopulationCurrent, PopulationCeiling);
        return PopulationCeiling;
    }

    public int CalculateJobCeiling() {                              // called only if factory was built
        JobCeiling = buildingCounts[factory] * jobsPerFactory;
        if (JobCeiling > maxJobs)
            JobCeiling = maxJobs;
            uIController.UpdateJob(JobCurrent, JobCeiling);
        return JobCeiling;
    }

    public float CalculateFoodIncome() {                            // called only if farm was built
        FoodIncome = buildingCounts[farm] * foodPerFarm;
        if (FoodIncome > maxFoodIncome)
            FoodIncome = maxFoodIncome;
        uIController.UpdateFood(Food, FoodConsuming, FoodIncome);
        return FoodIncome;
    }

    public void CalculateSpentMoney(int spent) {                    // called only if any buylding was built
        Money -= spent;
        uIController.UpdateMoney(Money, Income);
    }

    private void CalculateJobs() {                                  // called if PopulationCurrent was changed
        JobCurrent = Mathf.Min((int)PopulationCurrent, CalculateJobCeiling());
    }

    private void CalculateMoney() {                                 // called at the end of day
        Money += Mathf.RoundToInt(CalculateMoneyIncome());
        if (Money > maxMoney)
            Money = maxMoney;
    }

    private void CalculatePopulation(bool trimCeiling = false) {    // called at the end of day or happens that PopulationCurrent > PopulationCeiling
        if (Food >= FoodConsuming && !trimCeiling) {
            if (Food >= FoodConsuming + foodConsumingPerOnePeople) {
                PopulationCurrent += PopulationCurrent * staticPopulationRate * ApplyMorale();
            }
            Food -= CalculateFoodConsuming();
        }
        else if (!trimCeiling) {
            int loosingPeople = (int)PopulationCurrent / 10;
            if ((int)PopulationCurrent % 10 != 0)
                loosingPeople++;
            canibalizm = true;
            canibalizmPenaltiTimer = canibalizmCooldown;
            PopulationCurrent -= loosingPeople;
            if (PopulationCurrent < 1)
                PopulationCurrent = 1;
            Food = loosingPeople * foodFromOnePeole;
        }
        if (PopulationCurrent > PopulationCeiling)
            PopulationCurrent = PopulationCeiling;
    }

    private float ApplyMorale() {                                   // modifyed increasing of Population and Income
        return (badMorale ? badMoralePenalty : 1f)                  // sets badMoralePenalty if badMorale = true
        * (goodMorale ? goodMoraleBonus : 1f);                      // sets goodMoralePenalty if goodMorale = true
    }

    private float CalculateMoneyIncome() {                          // called at the end of day
        Income = JobCurrent * moneyPerJob * ApplyMorale();
        if (Income > maxIncome)
            Income = maxIncome;
        return Income;
    }

    private void CalculateFoodFromFarm() {                          // called at the end of day
        Food += FoodIncome;
        if (Food > maxFood)
            Food = maxFood;
    }

    private float CalculateFoodConsuming() {                        // called if PopulationCurrent was changed
        FoodConsuming = PopulationCurrent > 1 ?  PopulationCurrent * foodConsumingPerOnePeople : 0;
        return FoodConsuming;
    }

    private void MoraleReview() {                                   // called at the end of day
        if (canibalizmPenaltiTimer > 0) {
            badMorale = true;
            goodMorale = false;
        } 
        else if (Food > FoodConsuming * goodMoraleBonus && !badMorale) {
            goodMorale = true;
        } 
        else if (Food > FoodConsuming * foodSafety) {
            badMorale = false;
            goodMorale = false;
        }
        canibalizmPenaltiTimer--;
    }

    private void textUpdate() {                                     // called at the end of day
        uIController.UpdateMoney(Money, Income);
        uIController.UpdateDay(Day);
        uIController.UpdateFood(Food, FoodConsuming, FoodIncome);
        uIController.UpdatePopulation(PopulationCurrent, PopulationCeiling);
        uIController.UpdateJob(JobCurrent, JobCeiling);

        if (goodMorale)
            uIController.SetGreen();
        else if (badMorale)
            uIController.SetRed();
        else
            uIController.SetBlack();
    }
}
