using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	private Building[,] boardStorage = new Building[100, 100];
	
	public void AddBuilding(Building building, Vector3 pos) {							// adding building to storage on desired indexes
		Building toAdd = Instantiate(building, pos, Quaternion.identity);
		toAdd.transform.SetParent(transform);
		boardStorage[(int)pos.x, (int)pos.z] = toAdd;
	}

	public void RemoveBuilding(Vector3 pos) {
		Destroy(boardStorage[(int)pos.x, (int)pos.z].gameObject);
		boardStorage[(int)pos.x, (int)pos.z] = null;
	}

	public Building CheckForBuildinginPosition(Vector3 pos) {								// check if there is no building on desired indexes
		return boardStorage[(int)pos.x, (int)pos.z];
	}

	public Vector3 CalculateGridPosition(Vector3 position) {							// correcting coordinates for right building placement 
		return new Vector3(Mathf.Round(position.x), 0.5f, Mathf.Round(position.z));
	}
}