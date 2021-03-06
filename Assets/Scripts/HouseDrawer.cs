﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Road;
using Zenject;

public class HouseDrawer : MonoBehaviour 
{
	[Inject]
	Installer.Settings.RoadTiles tiles;

	private const float DeadEndHouseDistance = 0.75f;
	private const float StraightHouseDistance =  0.75f;
	private const float TurnHouseDistance = 0.55f;
	private const float TJunctionHouseDistance = 0.75f;

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
		Vector3 roadPosition = new Vector3(coords.x, coords.y, 0f);

		return Instantiate(tiles.houseTile, roadPosition + (ToDirectionVector(ToRadians((float)direction)) * DeadEndHouseDistance),
				Quaternion.Euler(0, 0, (float)direction + 90)) as GameObject;
	}

	private GameObject DrawHouseAtStraight (Coordinate coords, Direction direction) {
		Vector3 roadPosition = new Vector3(coords.x, coords.y, 0f);

		return Instantiate(tiles.houseTile, roadPosition - (ToDirectionVector(ToRadians((float)direction)) * StraightHouseDistance),
				Quaternion.Euler(0, 0, (float)direction - 90)) as GameObject;
	}

	private GameObject DrawHouseAtTurn (Coordinate coords, Direction direction) {
		Vector3 roadPosition = new Vector3(coords.x, coords.y, 0f);

		return Instantiate(tiles.houseTile, roadPosition - (ToDirectionVector(ToRadians((float)direction + 45)) * TurnHouseDistance),
				Quaternion.Euler(0, 0, (float)direction - 45)) as GameObject;
	}

	private GameObject DrawHouseAtTJunction (Coordinate coords, Direction direction) {
		Vector3 roadPosition = new Vector3(coords.x, coords.y, 0f);

		return Instantiate(tiles.houseTile, roadPosition - (ToDirectionVector(ToRadians((float)direction + 90)) * TJunctionHouseDistance),
				Quaternion.Euler(0, 0, (float)direction)) as GameObject;
	}

	private float ToRadians (float angleInDegrees) {
		return (angleInDegrees * Mathf.PI) / 180;
	}

	private Vector3 ToDirectionVector (float angleInRadians) {
		return new Vector3(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians), 0f);
	}
}
