using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	[SerializeField]
    private Text DayText;
	[SerializeField]
    private Text MoneyText;
	[SerializeField]
    private Text IncomeText;
	[SerializeField]
    private Text FoodText;
	[SerializeField]
    private Text FoodIncomeText;
	[SerializeField]
    private Text FoodOutcomeText;
	[SerializeField]
    private Text PopulationText;
	[SerializeField]
    private Text PopulationCeilingText;
	[SerializeField]
    private Text JobText;
	[SerializeField]
    private Text JobCeilingText;

	public void UpdateDay(int Day = 1) {
		DayText.text = Day.ToString();
	}

	public void UpdateJob(int JobCurrent = 0) {
		JobText.text = JobCurrent.ToString();
	}

	public void UpdateJobCeiling(int JobCeiling = 0) {
		JobCeilingText.text = JobCeiling.ToString();
	}

	public void UpdateMoney(int Money = 0) {
		MoneyText.text = Money.ToString();
	}

	public void UpdateIncome(float Income = 0) {
		IncomeText.text = Income.ToString();
	}

	public void UpdateFood(float Food = 0) {
		FoodText.text = Food.ToString("F1");
	}

	public void UpdateFoodIncome(float FoodIncome = 0) {
		FoodIncomeText.text = FoodIncome.ToString();
	}

	public void UpdateFoodOutcome(float FoodConsuming = 0) {
		FoodOutcomeText.text = FoodConsuming.ToString("F1");
	}

	public void UpdatePopulation(float PopulationCurrent = 1) {
		PopulationText.text = PopulationCurrent.ToString("F0");
	}

	public void UpdatePopulationCeiling(int PopulationCeiling = 1) {
		PopulationCeilingText.text = PopulationCeiling.ToString();
	}

	public void SetRed() {
		FoodText.color = Color.red;
		IncomeText.color = Color.red;
	}

	public void SetGreen() {
		FoodText.color = Color.green;
		IncomeText.color = Color.green;
	}

	public void SetBlack() {
		FoodText.color = Color.black;
		IncomeText.color = Color.black;
	}
}
