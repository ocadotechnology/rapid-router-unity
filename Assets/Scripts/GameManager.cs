using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public BoardManager boardScript;

	private int level = 3;

	// Use this for initialization
	void Awake () {
		boardScript = GetComponent<BoardManager> ();
		InitGame ();
	}

	void InitGame() {
		boardScript.SetupScene (level);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
