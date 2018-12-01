using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Text DayText;
    public Text MoneyText;
    public Text FoodText;
    public Text PopulationText;
    public Text JobText;

	public void UpdateDay(int Day = 1) {
		DayText.text = "Day " + Day;
	}

	public void UpdateJob(int JobCurrent = 0, int JobCeiling = 0) {
		JobText.text = "Jobs:\t\t\t" + JobCurrent + "/" + JobCeiling;
	}

	public void UpdateMoney(int Money = 0, float Income = 0) {
		MoneyText.text = "Money:\t\t\t$" + Money + " (+$" + (int)Income + ")";
	}

	public void UpdateFood(float Food = 0, float FoodConsuming = 0, float FoodIncome = 0) {
		FoodText.text = "Food:\t\t\t" + Food.ToString("F1") + " (-" + FoodConsuming.ToString("F1") + ")/(+" + FoodIncome + ")";
	}

	public void UpdatePopulation(float PopulationCurrent = 1, int PopulationCeiling = 1) {
		PopulationText.text = "Population:\t" + (int)PopulationCurrent + "/" + PopulationCeiling;
	}

	public void SetRed() {
		FoodText.color = Color.red;
	}

	public void SetGreen() {
		FoodText.color = Color.green;
	}

	public void SetBlack() {
		FoodText.color = Color.black;
	}
}
