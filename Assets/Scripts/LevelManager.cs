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

	public void LevelChangeListener(string newLevelIndex) {
		LevelChange(int.Parse(newLevelIndex));
	}

	public void LevelChange(int levelSelectIndex) {
		boardScript.SetupScene(levelSelectIndex + 1);
	}
}

