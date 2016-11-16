using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Zenject;

public class LevelManager : MonoBehaviour 
{

	[Inject]
	BoardManager boardScript;

	private List<string> items = new List<string>();
	public Dropdown dropdown;

	void Start() {
		PopulateList ();
	}

	void PopulateList() {
		int levelCount = Directory.GetFiles("Assets/Resources/Levels", "*.json").Length;
		for (int i = 1; i <= levelCount; i++) {
			items.Add ("Level " + i);
		}
		dropdown.AddOptions (items);
	}

	public void Level_Change(int levelSelectIndex) {
		boardScript.SetupScene (levelSelectIndex + 1);
	}
}

