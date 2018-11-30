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

	private City city;

	// Use this for initialization
	void Start () {
		city = FindObjectOfType<City>();
	}

	public void UpdateDay() {
		DayText.text = "Day " + city.Day;
	}

	public void UpdateJob() {
		JobText.text = "Jobs:\t\t" + city.JobCurrent + "/" + city.JobCeiling;
	}

	public void UpdateMoney() {
		MoneyText.text = "Money:\t\t$" + city.Money + " (+$" + (int)city.Income + ")";
	}

	public void UpdateFood() {
		FoodText.text = "Food:\t\t" + (int)city.Food + " (-" + Mathf.RoundToInt(city.FoodConsuming) + ")";
	}

	public void UpdatePopulation() {
		PopulationText.text = "Population:\t" + (int)city.PopulationCurrent + "/" + city.PopulationCeiling;
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
