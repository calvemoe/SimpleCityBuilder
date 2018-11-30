using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : MonoBehaviour {
	[SerializeField]
	private City city;
	[SerializeField]
	private UIController uIController;
	[SerializeField]
	private Building[] buildings;
	[SerializeField]
	private Board board;
	private Building selectedBuilding;

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) && selectedBuilding != null) {
			InteractWithBoard();
		}
	}

	void InteractWithBoard() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			Vector3 gridPosition = board.CalculateGridPosition(hit.point);
			if (!board.CheckForBuildinginPosition(gridPosition)) {
				if (city.Money >= selectedBuilding.cost) {
					city.Money -= selectedBuilding.cost;
					uIController.UpdateMoney();
					if (selectedBuilding.id != 0)
						city.buildingCounts[selectedBuilding.id - 1]++;
					board.AddBuilding(selectedBuilding, gridPosition);
				}
			}
		}
	}

	public void EnableBuilder(int building) {
		selectedBuilding = buildings[building];
		Debug.Log(selectedBuilding);
	}
}
