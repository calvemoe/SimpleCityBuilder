using System;
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
		if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift) && selectedBuilding != null) {	// build selected building by dragging holded MBL on the Ground
			InteractWithBoard();
		} else if (Input.GetMouseButtonDown(0) && selectedBuilding != null) {							// build selected building by clicking MBL on ground
			InteractWithBoard();
		}

		if (Input.GetMouseButtonDown(1)) {																// on MBR
			InteractWithBoard(false);																	// destuction mode
		}
	}

	void InteractWithBoard(bool creation = true) {														// if creation = true - build mode; false - destruction mode;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {															// if there is RaycastHit 
			Vector3 gridPosition = board.CalculateGridPosition(hit.point);								// calculate corret position for buildings grid based on clicked position
			if (gridPosition.x < 1 || gridPosition.x > 99 || gridPosition.z < 1 || gridPosition.z > 99)
				return;
			if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) { 				// check if not pointed on ui object
				if (creation && board.CheckForBuildinginPosition(gridPosition) == null) {				// if no building on pointed spot
					if (city.Money >= selectedBuilding.cost) {											// if enought money for building
						city.CalculateSpentMoney(selectedBuilding.cost);								// substract cost from Money
						if (selectedBuilding.id != 0) {
							city.buildingCounts[selectedBuilding.id]++;
							UpdatingAfterInteraction(selectedBuilding.id);
						}
						board.AddBuilding(selectedBuilding, gridPosition);								// adding selected building on pointed spot
					}
				}
				else if (!creation && board.CheckForBuildinginPosition(gridPosition) != null) {
					city.buildingCounts[board.CheckForBuildinginPosition(gridPosition).id]--;
					UpdatingAfterInteraction(board.CheckForBuildinginPosition(gridPosition).id);
					if (board.CheckForBuildinginPosition(gridPosition).id != 0)
						city.CalculateRefundMoney(board.CheckForBuildinginPosition(gridPosition).cost);
					else
						city.CalculateSpentMoney(board.CheckForBuildinginPosition(gridPosition).cost / 2); 
					board.RemoveBuilding(gridPosition);
				}
			}
		}
	}

	public void EnableBuilder(int building) {															// select needed building
		selectedBuilding = buildings[building];
	}

	void UpdatingAfterInteraction(int biuldingId) {														// for UI update
		switch (biuldingId) {
			case (int)City.buildingsList.house :
				city.CalculatePopulationCeiling();
				break;
			case (int)City.buildingsList.farm :
				city.CalculateFoodIncome();
				break;	
			case (int)City.buildingsList.factory :
				city.CalculateJobCeiling();
				break;
			case(int)City.buildingsList.road :
				//Debug.Log("Remove road: -2$");
				break;
			default:
				throw new ArgumentException();
		}
	}
}
