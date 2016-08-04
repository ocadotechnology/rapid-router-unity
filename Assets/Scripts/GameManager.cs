using System;
using UnityEngine;
using Zenject;

public class GameManager: IInitializable {

	[Inject]
	BoardManager boardScript;

	private int level = 40;

	[PostInject]
    public void Initialize()
    {
		boardScript.SetupScene(level);
    }
	
}
