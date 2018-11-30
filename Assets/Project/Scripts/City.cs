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
    public float foodSafety = 1.75f;                    // safety multiplier for people population increasing
    public float badMoralePenalty = 0.33f;              // penalty to Money increasing from canibalizm
    public float goodMoraleBonus = 1.2f;                // bonus to Money increasing 
    public float staticPopulationRate = 0.025f;         // passive value for population incresing if there enough food
    public int canibalizmCooldown = 3;                  // 3 days cooldown before morale stabilize after canibalizm act

    public int Day { get; set; }

    // calculated values
    public int Money { get; set; }                      // available amount of money = JobCurrent * moneyPerJob (max 1000000)
    public float Income { get; set; }                   // amount of money earned per Day
    public float PopulationCurrent { get; set; }        // current amount of people
    public float PopulationCeiling { get; set; }        // #house * houseCapacity (max 1000000)
    public int JobCurrent { get; set; }                 // number of working people
    public int JobCeiling { get; set; }                 // #factory * jobsPerFactory (max 999998)
    public float Food { get; set; }                     // available amount of food = #farm * foodPerFarm (max 1000000)
    public float FoodConsuming { get; set; }            // amount of food consuming per Day = PopulationCurrent * foodConsumingPerOnePeople

    // buildings storing array
    public int[] buildingCounts = new int[3];            // 0 - road, 1 - house, 2 - farm, 3 - factory

    // using for buildingCounts access to appropriate values
    //private const int road = 0;
    private const int house = 0;
    private const int farm = 1;
    private const int factory = 2;

    //temporary effects switch
    private bool badMorale = false;                     // true - bad morale effect active - redure Money income
    private bool goodMorale = false;                    // true - good morale effect active - increase Money income
    private bool canibalizm = false;                    // true - if Food < FoodConsuming and people were eaten

    private int canibalizmPenaltiTimer = 0;             // on '0' - bad morale after canibalizm is gone

    // maximim values
    private const int maxMoney = 1000000;
    private const int maxIncome = 99998;
    private const float maxFood = 1000000f;
    private const int maxPopulation = 1000000;
    private const int maxJobs = 999998;

    private UIController uIController;

    // Use this for initialization
    void Start () {

        uIController = FindObjectOfType<UIController>();
        PopulationCeiling = 1;
        PopulationCurrent = 1;
        Money = 500;
    }

    public void EndTurn() {
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

    private void CalculateJobs() {
        JobCeiling = buildingCounts[factory] * jobsPerFactory;          // move to update after building was deployed ?
        if (JobCeiling > maxJobs)
            JobCeiling = maxJobs;

        JobCurrent = Mathf.Min((int)PopulationCurrent, JobCeiling);
    }

    private void CalculateMoney() {
        Income = JobCurrent * moneyPerJob * ApplyMorale();              // move to update after building was deployed ?
        if (Income > maxIncome)
            Income = maxIncome;

        Money += (int)Income;
        if (Money > maxMoney)
            Money = maxMoney;
    }

    private void CalculateFoodFromFarm() {
        Food += buildingCounts[farm] * foodPerFarm;                     // move to update after building was deployed ?
        if (Food > maxFood)                                             // move to update after building was deployed ?
            Food = maxFood;                                             // move to update after building was deployed ?
    }

    private void CalculatePopulation() {
        int loosingPeople = 0;
        if (buildingCounts[house] > 0)
            PopulationCeiling = buildingCounts[house] * houseCapacity;  // move to update after building was deployed ?
        if (PopulationCeiling > maxPopulation)                          // move to update after building was deployed ?
            PopulationCeiling = maxPopulation;

        if (Food >= FoodConsuming) {                                    // add calculation if food not enough for all people
            if (Food >= FoodConsuming + foodConsumingPerOnePeople) {
                PopulationCurrent += PopulationCurrent * staticPopulationRate * ApplyMorale();
                if (PopulationCurrent > PopulationCeiling)
                    PopulationCurrent = PopulationCeiling;
            }
            Food -= CalculateFoodConsuming();
        }
        else {
            loosingPeople = (int)PopulationCurrent / 10;
            if ((int)PopulationCurrent % 10 != 0)
                loosingPeople++;
            canibalizm = true;
            canibalizmPenaltiTimer = canibalizmCooldown;
            PopulationCurrent -= loosingPeople;
            if (PopulationCurrent == 0) 
                PopulationCurrent = 1;
            Food = loosingPeople * foodFromOnePeole;
        }
    }

    private float CalculateFoodConsuming() {
        FoodConsuming = PopulationCurrent > 0 ?  PopulationCurrent * foodConsumingPerOnePeople : 0;
        return FoodConsuming;
    }

    private float ApplyMorale() {
        return (badMorale ? badMoralePenalty : 1f) * (goodMorale ? goodMoraleBonus : 1f);
    }

    private void MoraleReview() {
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

    private void textUpdate() {
        uIController.UpdateMoney();
        uIController.UpdateDay();
        uIController.UpdateFood();
        uIController.UpdatePopulation();
        uIController.UpdateFood();
        uIController.UpdateJob();

        if(goodMorale)
            uIController.SetGreen();
        else if (badMorale)
            uIController.SetRed();
        else
            uIController.SetBlack();
    }
}
