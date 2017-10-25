using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Road;
using Zenject;

public class HouseDrawer : MonoBehaviour {

	[Inject]
	BoardTranslator translator;

	[Inject]
	Installer.Settings.RoadTiles tiles;

	public List<GameObject> DrawHouses (RoadSegment[] roadSegments, HashSet<Coordinate> destinationCoords) {
		List<GameObject> houses = new List<GameObject>();
		foreach (RoadSegment roadSegment in roadSegments) {
			if (destinationCoords.Contains (roadSegment.coords)) {
				houses.Add(DrawHouseAt(roadSegment));
			}
		}
		return houses;
	}

	private GameObject DrawHouseAt (RoadSegment roadSegment) {
		switch (roadSegment.roadType) {
		case RoadType.DeadEnd:
			return DrawHouseAtDeadEnd(roadSegment.coords, roadSegment.direction);
		case RoadType.Straight:
			return DrawHouseAtStraight(roadSegment.coords, roadSegment.direction);
		case RoadType.Turn:
			return DrawHouseAtTurn(roadSegment.coords, roadSegment.direction);
		case RoadType.TJunction:
			return DrawHouseAtTJunction(roadSegment.coords, roadSegment.direction);
		default:
			return null; // Shouldn't get here - houses at crossroads not supported
		}
	}

	private GameObject DrawHouseAtDeadEnd (Coordinate coords, Direction direction) {
		float sceneX = translator.translateToSceneRow(coords.x);
		float sceneY = translator.translateToSceneColumn(coords.y);

		Vector3 roadPosition = new Vector3(sceneX, sceneY, 0f);

		return Instantiate(tiles.houseTile, roadPosition + (ToDirectionVector(ToRadians((float)direction)) * 0.75f),
				Quaternion.Euler(0, 0, (float)direction + 90)) as GameObject;
	}

	private GameObject DrawHouseAtStraight (Coordinate coords, Direction direction) {
		float sceneX = translator.translateToSceneRow(coords.x);
		float sceneY = translator.translateToSceneColumn(coords.y);

		Vector3 roadPosition = new Vector3(sceneX, sceneY, 0f);

		return Instantiate(tiles.houseTile, roadPosition - (ToDirectionVector(ToRadians((float)direction)) * 0.75f),
				Quaternion.Euler(0, 0, (float)direction - 90)) as GameObject;
	}

	private GameObject DrawHouseAtTurn (Coordinate coords, Direction direction) {
		float sceneX = translator.translateToSceneRow(coords.x);
		float sceneY = translator.translateToSceneColumn(coords.y);

		Vector3 roadPosition = new Vector3(sceneX, sceneY, 0f);

		return Instantiate(tiles.houseTile, roadPosition - (ToDirectionVector(ToRadians((float)direction + 45)) * 0.55f),
				Quaternion.Euler(0, 0, (float)direction - 45)) as GameObject;
	}

	private GameObject DrawHouseAtTJunction (Coordinate coords, Direction direction) {
		float sceneX = translator.translateToSceneRow(coords.x);
		float sceneY = translator.translateToSceneColumn(coords.y);

		Vector3 roadPosition = new Vector3(sceneX, sceneY, 0f);

		return Instantiate(tiles.houseTile, roadPosition - (ToDirectionVector(ToRadians((float)direction + 90)) * 0.75f),
				Quaternion.Euler(0, 0, (float)direction)) as GameObject;
	}

	private float ToRadians (float angleInDegrees) {
		return (angleInDegrees * Mathf.PI) / 180;
	}

	private Vector3 ToDirectionVector (float angleInRadians) {
		return new Vector3(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians), 0f);
	}
}
