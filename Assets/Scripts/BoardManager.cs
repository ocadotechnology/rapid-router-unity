using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;

public class BoardManager: MonoBehaviour {

	[Serializable]
	public class Count {
		public int minimum;
		public int maximum;

		public Count(int min, int max) {
			minimum = min;
			maximum = max;
		}
	}

	public int rows = 8;
	public int columns = 10;

	public GameObject grassTile;

	private Transform boardHolder;
	private List<Vector3> gridPositions = new List<Vector3>();

	void InitialiseList() {
		gridPositions.Clear ();
		for (int x = 0; x <= columns - 1; x++) {
			for (int y = 0; y <= rows - 1; y++) {
				gridPositions.Add (new Vector3 (x, y, 0f));
			}
		}
	}

	void BoardSetup() {
		boardHolder = new GameObject ("Board").transform;
		for (int x = 0; x <= columns - 1; x++) {
			for (int y = 0; y <= rows - 1; y++) {
				GameObject grassObject = Instantiate (grassTile, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				grassObject.transform.SetParent (boardHolder);
			}
		}
	}


	public void SetupScene(int level) {
		InitialiseList ();
		BoardSetup ();
	}


	// Use this for initialization
	public JSONNode LoadJSONFile () {
		TextAsset levelTextAsset = (TextAsset)Resources.Load ("Levels/1", typeof(TextAsset));
		string levelText = levelTextAsset.text;
		return JSON.Parse (levelText);
	}


}
