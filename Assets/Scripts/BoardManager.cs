using System;
using System.Collections.Generic;
using UnityEngine;
using Road;
using Zenject;
using DG.Tweening;

public enum Direction : int
{
    North = 0,
    East = -90,
    South = -180,
    West = -270
};

public class Level
{
    public PathNode[] path;
    public OriginNode origin;

    public int[][] destinations;

    public HashSet<Coordinate> destinationCoords {
		get {
            HashSet<Coordinate> coordinates = new HashSet<Coordinate>();
            for (int i = 0; i < destinations.Length; i++) {
                int[] dest = destinations[i];
                coordinates.Add(new Coordinate(dest[0], dest[1]));
            }
            return coordinates;
		}
	}

    public LevelDecor[] leveldecor_set;
}

public class LevelDecor {
    public int x;
    public int y;
    public String decorName;
}

public class BoardManager : MonoBehaviour, IInitializable
{
    public float rows;
    public float columns;

    public static Level currentLevel;

    [Inject]
	Installer.Settings.FloorTiles floorTiles;
	[Inject]
	Installer.Settings.RoadTiles roadTiles;

    [Inject]
    RoadDrawer roadDrawer;

    [Inject]
    DecorDrawer decorDrawer;

    private static Transform boardHolder;

    [Inject]
    BoardTranslator translator;

    [Inject]
    Installer.Settings.MapSettings mapDimensions;

	public static HashSet<Coordinate> roadCoordinates = new HashSet<Coordinate>();

    [PostInject]
    public void Initialize() {
        rows = mapDimensions.rows;
        columns = mapDimensions.columns;
    }
		
	public void SetupScene(int level)
	{
		GameObject currentBoard = GameObject.Find ("Board");
		if (currentBoard != null) {
			GameObject.DestroyObject (currentBoard);
		}
		boardHolder = new GameObject("Board").transform;
		SetupLevel(level);
	}

    private void SetupLevel(int levelNumber)
    {
        currentLevel = LevelReader.ReadLevelFromFile(levelNumber);

        SetupBoard();
		SetupRoute ();
        SetupDecorations();
        SetupVan ();
    }

	private void SetupBoard()
	{
		for (int x = 0; x < columns; x++)
		{
			for (int y = 0; y < rows; y++)
			{
				SetStaticWithBoardAsParent(
					Instantiate(floorTiles.grassTile, 
								new Vector3(translator.translateToSceneRow(x, true), translator.translateToSceneColumn(y, true), 0f),
								Quaternion.identity) as GameObject);
			}
		}
	}

	private void SetupRoute() 
	{
		GameObject cfcOrigin = SetupOrigin (currentLevel.origin);
		GameObject[] roadObjects = roadDrawer.SetupRoadSegments(currentLevel.path);
		foreach (GameObject roadObject in roadObjects) {
			Coordinate currCoord = new Coordinate(roadObject.transform.position);
			roadCoordinates.Add(currCoord);
		}

		GameObject homeDestination = SetupDestinations(currentLevel.destinationCoords);

		List<GameObject> roadObjectsList = new List<GameObject>(roadObjects);
		roadObjectsList.Add (cfcOrigin);
		roadObjectsList.Add (homeDestination);
		foreach (GameObject roadObject in roadObjectsList) {
			SetStaticWithBoardAsParent (roadObject);
		}
	}

	private GameObject SetupOrigin(OriginNode origin)
	{
		Direction direction = RoadDrawer.StringToDirection(origin.direction);
		Coordinate coords = origin.coords;
		return Instantiate(roadTiles.cfcTile, new Vector3(translator.translateToSceneRow(coords.x), translator.translateToSceneColumn(coords.y), 0f),
			Quaternion.Euler(0, 0, (float)direction)) as GameObject;
	}

	private GameObject SetupDestinations(HashSet<Coordinate> destinationNodes)
	{
		return new GameObject ();
	}

	private void SetupDecorations() {
        GameObject[] decorations = decorDrawer.SetupDecorations(currentLevel.leveldecor_set);
		foreach (GameObject decorObject in decorations) {
            SetStaticWithBoardAsParent(decorObject);
        }
    }
		
	private void SetupVan() 
	{
		GameObject van = GameObject.Find ("Van");
		van.transform.position = translator.translateToSceneVector(currentLevel.origin.coords.vector);
		int direction = (int)RoadDrawer.StringToDirection(currentLevel.origin.direction);

		van.transform.rotation = Quaternion.identity;
		van.transform.Rotate(new Vector3(0, 0, direction));
		van.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		van.transform.position += VehicleMover.ForwardABit(van.transform, 0.5f);
		DOTween.defaultEaseOvershootOrAmplitude = 0;

		BoardManager.SetBoardAsParent (van);
	}

	private static void SetStaticWithBoardAsParent(GameObject childObject) {
		SetStatic (childObject);
		SetBoardAsParent (childObject);
	}

	private static void SetStatic(GameObject staticObject) {
		staticObject.isStatic = true;
	}

	public static void SetBoardAsParent(GameObject childObject) {
		childObject.transform.SetParent(boardHolder);
	}
}