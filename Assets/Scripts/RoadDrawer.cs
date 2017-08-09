using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Road
{
    public class RoadDrawer : MonoBehaviour
    {

        [Inject]
        BoardTranslator translator;

        [Inject]
        Installer.Settings.RoadTiles tiles;

		public GameObject[] DrawRoad(RoadSegment[] roadSegments) {
			GameObject[] gameObjects = new GameObject[roadSegments.Length];
			for (int currentIndex = 0; currentIndex < roadSegments.Length; currentIndex++)
			{
				gameObjects[currentIndex] = DrawRoadSegment(roadSegments, currentIndex);
			}
			return gameObjects;
		}

		public GameObject DrawRoadSegment(RoadSegment[] roadSegments, int currentSegmentIndex)
		{
			RoadSegment roadSegment = roadSegments[currentSegmentIndex];
			switch (roadSegment.roadType)
			{
			case RoadType.DeadEnd:
				return DrawDeadEnd(roadSegment.coords, roadSegment.direction);
			case RoadType.Straight:
				return DrawStraight(roadSegment.coords, roadSegment.direction);
			case RoadType.Turn:
				return DrawTurn (roadSegment.coords, roadSegment.direction);
			case RoadType.TJunction:
				return DrawTJunction(roadSegment.coords, roadSegment.direction);
			case RoadType.CrossRoads:
				return DrawCrossRoad(roadSegment.coords);
			default:
				// should never get here
				return null;
			}
		}

		public GameObject DrawDeadEnd(Coordinate coords, Direction direction)
		{
			float x = translator.translateToSceneRow(coords.x);
			float y = translator.translateToSceneColumn(coords.y);
			return Instantiate(tiles.deadEndRoadTile, new Vector3(x, y, 0f),
				Quaternion.Euler(0, 0, (float)direction)) as GameObject;
		}

		public GameObject DrawStraight(Coordinate coords, Direction direction)
		{
			float x = translator.translateToSceneRow(coords.x);
			float y = translator.translateToSceneColumn(coords.y);
			return Instantiate(tiles.straightRoadTile, new Vector3(x, y, 0f),
				Quaternion.Euler(0, 0, (float)direction)) as GameObject;
		}

		public GameObject DrawTurn(Coordinate coords, Direction direction)
		{
			float x = translator.translateToSceneRow(coords.x);
			float y = translator.translateToSceneColumn(coords.y);
			return Instantiate(tiles.turnRoadTile, new Vector3(x, y, 0f),
				Quaternion.Euler(0, 0, (float)direction)) as GameObject;
		}

		public GameObject DrawTJunction(Coordinate coords, Direction direction)
		{
			float x = translator.translateToSceneRow(coords.x);
			float y = translator.translateToSceneColumn(coords.y);
			return Instantiate(tiles.tJunctionTile, new Vector3(x, y, 0f),
				Quaternion.Euler(0, 0, (float)direction)) as GameObject;
		}

		public GameObject DrawCrossRoad(Coordinate coords)
		{
			float x = translator.translateToSceneRow(coords.x);
			float y = translator.translateToSceneColumn(coords.y);
			return Instantiate(tiles.crossRoadTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
		}

        public static Direction StringToDirection(String directionString)
        {
            switch (directionString)
            {
                case "N":
                    return Direction.North;
                case "E":
                    return Direction.East;
                case "S":
                    return Direction.South;
                case "W":
                    return Direction.West;
                default:
                    return Direction.North;
            }
        }

    }
}
