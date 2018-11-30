using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	private Building[,] boardStorage = new Building[100, 100];
	
	public void AddBuilding(Building building, Vector3 pos) {
		Building toAdd = Instantiate(building, pos, Quaternion.identity);
		toAdd.transform.SetParent(transform);
		boardStorage[(int)pos.x, (int)pos.z] = toAdd;
		Debug.Log("toAdd " + toAdd.buldingName);
	}

	public bool CheckForBuildinginPosition(Vector3 pos) {
		return boardStorage[(int)pos.x, (int)pos.z] != null;
	}

	public Vector3 CalculateGridPosition(Vector3 position) {
		return new Vector3(Mathf.Round(position.x), 0.5f, Mathf.Round(position.z));
	}
}