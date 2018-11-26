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

    public int Day { get; set; }

    // calculated values
    public int Money { get; set; }                      // available amount of money = JobCurrent * moneyPerJob (max 1000000)
    public int Income { get; set; }                     // amount of money earned per Day
    public float PopulationCurrent { get; set; }        // current amount of people
    public float PopulationCeiling { get; set; }        // #house * houseCapacity (max 1000000)
    public int JobCurrent { get; set; }                 // number of working people
    public int JobCeiling { get; set; }                 // #factory * jobsPerFactory (max 999998)
    public float Food { get; set; }                     // available amount of food = #farm * foodPerFarm (max 1000000)

    public float FoodConsuming { get; set; }           // amount of food consuming per Day = PopulationCurrent * foodConsumingPerOnePeople

    // buildings storing array
    private int[] buildingCounts = new int[3];           // 0 - house, 1 - farm, 2 - factory

    // using for buildingCounts access to appropriate values
    private const int house = 0;
    private const int farm = 1;
    private const int factory = 2;

    // maximim values
    private const int maxMoney = 1000000;
    private const int maxIncome = 99998;
    private const float maxFood = 1000000f;
    private const int maxPopulation = 1000000;
    private const int maxJobs = 999998;

    // Use this for initialization
    void Start () {
		
	}

    public void EndTurn()
    {
        Day++;
        CalculateJobs();
        CalculateMoney();
        CalculateFoodFromFarm();
    }

    void CalculateJobs()
    {
        JobCeiling = buildingCounts[factory] * jobsPerFactory;
        if (JobCeiling > maxJobs)
            JobCeiling = maxJobs;

        JobCurrent = Mathf.Min((int)PopulationCurrent, JobCeiling);
    }

    void CalculateMoney()
    {
        Income = JobCurrent * moneyPerJob;
        if (Income > maxIncome)
            Income = maxIncome;

        Money += Income;
        if (Money > maxMoney)
            Money = maxMoney;
    }

    void CalculateFoodFromFarm()
    {
        Food += buildingCounts[farm] * foodPerFarm;
        if (Food > maxFood)
            Food = maxFood;
    }

    void CalculatePopulation()
    {
        
        PopulationCeiling = buildingCounts[house] * houseCapacity;
        if (PopulationCeiling > maxPopulation)
            PopulationCeiling = maxPopulation;

        if (Food > 0)
        {
            if (PopulationCurrent < PopulationCeiling)
            {

            }
        }
        else
        {
            int loosingPeople = (int)PopulationCurrent / 10;
            if ((int)PopulationCurrent % 10 != 0)
                loosingPeople++;
            PopulationCurrent -= loosingPeople;
        }

    }
}
