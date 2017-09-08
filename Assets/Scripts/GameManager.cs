using System;
using UnityEngine;
using Zenject;

public class GameManager: IInitializable {

	[Inject]
	BoardManager boardScript;
	private static int level = 13;

	[PostInject]
    public void Initialize()
    {
		boardScript.SetupScene (level);
    }
}
