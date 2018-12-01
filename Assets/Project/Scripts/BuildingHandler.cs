using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : MonoBehaviour {
	[SerializeField]
	private City city;
	[SerializeField]
	private Building[] buildings;
	[SerializeField]
	private Board board;
	private Building selectedBuilding;

	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift) && selectedBuilding != null) {	// build selected building by dragging mouse on the Ground
			InteractWithBoard();
		} else if (Input.GetMouseButtonDown(0) && selectedBuilding != null) {							// build selected building by clicking on ground
			InteractWithBoard();
		}
	}

	void InteractWithBoard() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {															// if there is RaycastHit 
			Vector3 gridPosition = board.CalculateGridPosition(hit.point);								// calculate corret position for buildings grid based on clicked position
			if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() 				// check if not pointed on ui object
				&& !board.CheckForBuildinginPosition(gridPosition)) {									// if no building on pointed spot
				if (city.Money >= selectedBuilding.cost) {												// if enought money for building
					city.CalculateSpentMoney(selectedBuilding.cost);									// substract cost from Money
					if (selectedBuilding.id != 0) {
						city.buildingCounts[selectedBuilding.id]++;
						switch (selectedBuilding.id) {													// for UI update
							case City.house :
								city.CalculatePopulationCeiling();
								break;
							case City.farm :
								city.CalculateFoodIncome();
								break;	
							case City.factory :
								city.CalculateJobCeiling();
								break;
							default:
								break;
						}
					}
					board.AddBuilding(selectedBuilding, gridPosition);										// adding selected building on pointed spot
				}
			}
		}
	}

	public void EnableBuilder(int building) {															// select needed building
		selectedBuilding = buildings[building];
	}
}
