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
	RoadBuilder roadBuilder;

    [Inject]
    RoadDrawer roadDrawer;

	[Inject]
	HouseDrawer houseDrawer;

    [Inject]
    DecorDrawer decorDrawer;

    private static Transform boardHolder;

    [Inject]
    Installer.Settings.MapSettings mapDimensions;

	public static HashSet<Coordinate> roadCoordinates = new HashSet<Coordinate>();
	private const float BackgroundLeftBoundaryPadding = -0.5f;

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
		GameObject backgroundTileParent = new GameObject("BackgroundTiles");
		for (int x = 0; x < columns; x++)
		{
			for (int y = 0; y < rows; y++)
			{
				GameObject tile = Instantiate(floorTiles.grassTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
				SetStatic(tile);
				tile.transform.SetParent(backgroundTileParent.transform);
			}
		}
		backgroundTileParent.transform.position = new Vector3(BackgroundLeftBoundaryPadding, 0f, 0f);
		SetBoardAsParent(backgroundTileParent);
	}

	private void SetupRoute() 
	{
		GameObject cfcOrigin = SetupOrigin (currentLevel.origin);
		RoadSegment[] roadSegments = roadBuilder.CreateRoadSegments (currentLevel.path);
		GameObject[] roadObjects = roadDrawer.DrawRoad(roadSegments);
		foreach (GameObject roadObject in roadObjects) {
			Coordinate currCoord = new Coordinate(roadObject.transform.localPosition);
			roadCoordinates.Add(currCoord);
		}

		List<GameObject> homeDestinations = SetupDestinations(roadSegments, currentLevel.destinationCoords);

		List<GameObject> roadObjectsList = new List<GameObject>(roadObjects);
		roadObjectsList.Add (cfcOrigin);
		roadObjectsList.AddRange (homeDestinations);
		foreach (GameObject roadObject in roadObjectsList) {
			SetStaticWithBoardAsParent (roadObject);
		}
	}

	private GameObject SetupOrigin(OriginNode origin)
	{
		Direction direction = RoadDrawer.StringToDirection(origin.direction);
		Coordinate coords = origin.coords;
		return Instantiate(roadTiles.cfcTile, new Vector3(coords.x, coords.y, 0f),
			Quaternion.Euler(0, 0, (float)direction)) as GameObject;
	}

	private List<GameObject> SetupDestinations(RoadSegment[] roadSegments, HashSet<Coordinate> destinationNodes)
	{
		return houseDrawer.DrawHouses(roadSegments, destinationNodes);
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
		van.transform.localPosition = currentLevel.origin.coords.vector;
		int direction = (int)RoadDrawer.StringToDirection(currentLevel.origin.direction);

		van.transform.rotation = Quaternion.identity;
		van.transform.Rotate(new Vector3(0, 0, direction));
		van.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		van.transform.localPosition += VehicleMover.ForwardABit(van.transform, 0.5f);
		DOTween.defaultEaseOvershootOrAmplitude = 0;
        van.GetComponent<SpriteRenderer>().color = Color.white;

        BoardManager.SetBoardAsParent (van);
	}

	private static void SetStaticWithBoardAsParent(GameObject childObject)
	{
		SetStatic (childObject);
		SetBoardAsParent (childObject);
	}

	private static void SetStatic(GameObject staticObject)
	{
		staticObject.isStatic = true;
	}

	public static void SetBoardAsParent(GameObject childObject)
	{
		childObject.transform.SetParent(boardHolder);
	}
}