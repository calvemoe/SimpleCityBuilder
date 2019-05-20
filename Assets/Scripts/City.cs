using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {
    // balancing values
    [SerializeField]
    private int moneyPerJob = 2;                         // amount of money gained per one working people on factory
    [SerializeField]
    private float foodPerFarm = 4f;                      // amount of produced food per one farm
    [SerializeField]
    private int jobsPerFactory = 10;                     // number of jobs per one factory - extend JobCeiling
    [SerializeField]
    private int houseCapacity = 5;                       // number of people per one house - extend PopulationCeiling
    [SerializeField]
    private float foodConsumingPerOnePeople = 0.5f;      // amount of food consumed per one people per one Day
    [SerializeField]
    private float foodFromOnePeople = 2.5f;              // food gain after one people has to be eaten
    [SerializeField]
    private float foodSafety = 2f;                       // safety multiplier for people population increasing
    [SerializeField]
    private float badMoralePenalty = 0.33f;              // penalty to Money increasing from cannibalizm
    [SerializeField]
    private float goodMoraleBonus = 1.2f;                // bonus to Money increasing 
    [SerializeField]
    private float staticPopulationRate = 0.025f;         // passive value for population incresing if there enough food
    [SerializeField]
    private int cannibalizmCooldown = 5;                 // 3 days cooldown before morale stabilize after cannibalizm act

    public int Day { get; set; }

    // calculated values
    public float Money { get; set; }                      // available amount of money = JobCurrent * moneyPerJob (max 1000000)
    public float Income { get; set; }                   // amount of money earned per Day
    public float PopulationCurrent { get; set; }        // current amount of people
    public int PopulationCeiling { get; set; }          // #house * houseCapacity (max 1000000)
    public int JobCurrent { get; set; }                 // number of working people
    public int JobCeiling { get; set; }                 // #factory * jobsPerFactory (max 999998)
    public float Food { get; set; }                     // available amount of food = #farm * foodPerFarm (max 1000000)
    public float FoodIncome { get; set; }               // amount of food earned per Day
    public float FoodConsuming { get; set; }            // amount of food consuming per Day = PopulationCurrent * foodConsumingPerOnePeople

    // buildings storing array
    public int[] buildingCounts = new int[4];           // 0 - road, 1 - house, 2 - farm, 3 - factory

    //temporary effects switch
    private bool badMorale = false;                     // true - bad morale effect active - redure Money income
    private bool goodMorale = false;                    // true - good morale effect active - increase Money income
    private bool cannibalizm = false;                   // true - if Food < FoodConsuming and people were eaten     // todo: if we need it?

    private int cannibalizmPenaltiTimer = 0;            // on '0' - bad morale after cannibalizm is gone

    // maximum values
    private const float MAX_MONEY = 1000000f;
    private const int MAX_INCOME = 99998;
    private const float MAX_FOOD = 1000000f;
    private const float MAX_FOOD_INCOME = 50000f;
    private const int MAX_POPULATION = 1000000;
    private const int MAX_JOBS = 999900;

    private UIController uIController;

    // using for buildingCounts access to appropriate values
    public enum buildingsList {
        road,
        house,
        farm,
        factory
    }

    // Use this for initialization
    void Start () {
        uIController = FindObjectOfType<UIController>();
        PopulationCeiling = 1;
        PopulationCurrent = 1;
        Money = 500f;

        uIController.UpdateDay();
        uIController.UpdateMoney(Money);
        uIController.UpdateIncome();
        uIController.UpdateFood();
        uIController.UpdateFoodIncome();
        uIController.UpdateFoodOutcome();
        uIController.UpdatePopulation(PopulationCurrent);
        uIController.UpdatePopulationCeiling(PopulationCeiling);
        uIController.UpdateJob();
        uIController.UpdateJobCeiling();
    }

    public void EndTurn() {                                         // called by OnClick() - TurnButton
        
        if (PopulationCeiling == 0) {
            PopulationCeiling = 1;
            PopulationCurrent = 1;
        }
        else if (PopulationCeiling != 0 && PopulationCurrent == 0) {
            PopulationCurrent = 1;
        }

        Day++;
        CalculateJobs();
        CalculateMoney();
        CalculateFoodFromFarm();
        CalculatePopulation();
        if (cannibalizm) {
            CalculateJobs();
            CalculateFoodConsuming();
            cannibalizm = false;
        }
        MoraleReview();
        TextUpdate();
    }

    public int CalculatePopulationCeiling() {                       // called only if house was built or sold
        PopulationCeiling = buildingCounts[(int)buildingsList.house] * houseCapacity;
        if (PopulationCeiling > MAX_POPULATION)
            PopulationCeiling = MAX_POPULATION;
        if (PopulationCeiling < PopulationCurrent)
            CalculatePopulation(true);
		uIController.UpdatePopulationCeiling(PopulationCeiling);
        return PopulationCeiling;
    }

    public int CalculateJobCeiling() {                              // called only if factory was built or sold
        JobCeiling = buildingCounts[(int)buildingsList.factory] * jobsPerFactory;
        if (JobCeiling > MAX_JOBS)
            JobCeiling = MAX_JOBS;
            uIController.UpdateJobCeiling(JobCeiling);
        return JobCeiling;
    }

    public float CalculateFoodIncome() {                            // called only if farm was built or sold
        FoodIncome = buildingCounts[(int)buildingsList.farm] * foodPerFarm;
        if (FoodIncome > MAX_FOOD_INCOME)
            FoodIncome = MAX_FOOD_INCOME;
        uIController.UpdateFoodIncome(FoodIncome);
        return FoodIncome;
    }

    public void CalculateSpentMoney(float spent) {                    // called only if any buylding was built or sold
        Money -= spent;
        uIController.UpdateMoney(Money);
    }

    public void CalculateRefundMoney(float refund) {
        Money += refund / 2;
        uIController.UpdateMoney(Money);
    }

    private void CalculateJobs() {                                  // called if PopulationCurrent was changed
        JobCurrent = Mathf.Min((int)PopulationCurrent, CalculateJobCeiling());
    }

    private void CalculateMoney() {                                 // called at the end of day
        Money += CalculateMoneyIncome();
        if (Money > MAX_MONEY)
            Money = MAX_MONEY;
    }

    private void CalculatePopulation(bool trimCeiling = false) {    // called at the end of day or happens that PopulationCurrent > PopulationCeiling
        if (Food >= FoodConsuming && !trimCeiling) {
            if (Food >= FoodConsuming + foodConsumingPerOnePeople * PopulationCurrent * badMoralePenalty) {
                PopulationCurrent += PopulationCurrent * staticPopulationRate * ApplyMorale();
            }
        }
        else if (!trimCeiling) {
            int loosingPeople = (int)PopulationCurrent / 10;
            if ((int)PopulationCurrent % 10 != 0)
                loosingPeople++;
            cannibalizm = true;
            cannibalizmPenaltiTimer = cannibalizmCooldown;
            PopulationCurrent -= loosingPeople;

            Food += loosingPeople * foodFromOnePeople;
        }
        Food -= CalculateFoodConsuming();
        if (PopulationCurrent > PopulationCeiling)
            PopulationCurrent = PopulationCeiling;
        uIController.UpdatePopulation(PopulationCurrent);
    }

    private float ApplyMorale() {                                   // affect Population incrising and Income
        return (badMorale ? badMoralePenalty : 1f)                  // sets badMoralePenalty if badMorale = true
        * (goodMorale ? goodMoraleBonus : 1f);                      // sets goodMoralePenalty if goodMorale = true
    }

    private float CalculateMoneyIncome() {                          // called at the end of day
        if (JobCeiling != 0) {
            Income = JobCurrent * moneyPerJob * ApplyMorale();
            if (Income > MAX_INCOME)
                Income = MAX_INCOME;
            return Income;
        }
        else
            return Income = 0.4f;
    }

    private void CalculateFoodFromFarm() {                          // called at the end of day
        Food += FoodIncome;
        if (Food > MAX_FOOD)
            Food = MAX_FOOD;
    }

    private float CalculateFoodConsuming() {                        // called if PopulationCurrent was changed
        FoodConsuming = PopulationCurrent >= 2 ?  PopulationCurrent * foodConsumingPerOnePeople : 0;           // if PopulationCurrent < 2 - FoodConsuling = 0
        uIController.UpdateFoodOutcome(FoodConsuming);
        return FoodConsuming;
    }

    private void MoraleReview() {                                   // called at the end of day
        if (cannibalizmPenaltiTimer > 0) {
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
        cannibalizmPenaltiTimer--;
    }

    private void TextUpdate() {                                     // called at the end of day
        uIController.UpdateDay(Day);
        uIController.UpdateFood(Food);
        uIController.UpdateFoodIncome(FoodIncome);
        uIController.UpdateFoodOutcome(FoodConsuming);
        uIController.UpdatePopulation(PopulationCurrent);
        uIController.UpdateJob(JobCurrent);
        uIController.UpdateMoney(Money);
        uIController.UpdateIncome(Income);


        if (goodMorale)
            uIController.SetGreen();
        else if (badMorale)
            uIController.SetRed();
        else
            uIController.SetBlack();
    }
}