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
		PopulateList();
	}

	void PopulateList() {
		UnityEngine.Object[] levels = Resources.LoadAll("Levels");
		int levelCount = levels.Length;
		for (int i = 1; i <= levelCount; i++) {
			items.Add ("Level " + i);
			Resources.UnloadAsset((UnityEngine.Object) levels.GetValue(i - 1));
		}
		
		dropdown.AddOptions(items);
		dropdown.captionText.alignment = TextAnchor.MiddleCenter;
	}

	public void LevelChangeListener(string newLevelIndex) {
		LevelChange(int.Parse(newLevelIndex));
	}

	public void LevelChange(int levelSelectIndex) {
		boardScript.SetupScene(levelSelectIndex + 1);
	}
}

