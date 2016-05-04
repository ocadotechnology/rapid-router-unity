using UnityEngine;

public class GameManager : MonoBehaviour {

	public BoardManager boardScript;

	private int level = 1;

	// Use this for initialization
	void Awake () {
		boardScript = GetComponent<BoardManager> ();
		InitGame ();
	}

	void InitGame() {
		boardScript.SetupScene (level);
	}
	
}
