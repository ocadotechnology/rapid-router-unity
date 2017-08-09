using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Road
{

	public enum RoadType : int
	{
		DeadEnd = 1,
		Straight = 2,
		Turn = 3,
		TJunction = 4,
		CrossRoads = 5
	}

	public class RoadSegment
	{
		public RoadType roadType;
		public Coordinate coords;
		public Direction direction;

		public RoadSegment (RoadType roadType, Coordinate coords, Direction direction)
		{
			this.roadType = roadType;
			this.coords = coords;
			this.direction = direction;
		}
	}

	public class RoadBuilder : MonoBehaviour
	{
		public RoadSegment[] CreateRoadSegments (PathNode[] nodes)
		{
			RoadSegment[] roadSegments = new RoadSegment[nodes.Length];
			for (int currentIndex = 0; currentIndex < nodes.Length; currentIndex++) {
				roadSegments [currentIndex] = CreateRoadSegment (nodes, currentIndex);
			}
			return roadSegments;
		}

		private RoadSegment CreateRoadSegment (PathNode[] nodes, int currentNodeIndex)
		{
			PathNode node = nodes [currentNodeIndex];
			switch (node.connectedNodes.Length) {
			case 1:
				return CreateDeadEnd (node.coords, nodes [node.connectedNodes [0]].coords);
			case 2:
				return CreateStraightOrTurn (nodes, node);
			case 3:
				return CreateTJunction (nodes, node);
			case 4:
				return CreateCrossRoad (node);
			default:
				// we don't currently support lone pieces of road
				return null;
			}
		}

		private RoadSegment CreateDeadEnd (Coordinate coords, Coordinate neighbourCoords)
		{
			Direction direction = RelativeDirection (coords, neighbourCoords);
			return new RoadSegment (RoadType.DeadEnd, coords, direction);
		}

		private RoadSegment CreateStraightOrTurn (PathNode[] nodes, PathNode node)
		{
			PathNode neighbour1 = nodes [node.connectedNodes [0]];
			PathNode neighbour2 = nodes [node.connectedNodes [1]];

			return CreateStraightOrTurn (node.coords, neighbour1.coords, neighbour2.coords);
		}

		private RoadSegment CreateStraightOrTurn (Coordinate coords, Coordinate neighbour1, Coordinate neighbour2)
		{
			Direction direction1 = RelativeDirection (coords, neighbour1);
			Direction direction2 = RelativeDirection (coords, neighbour2);
			if (AreOppositeDirections (direction1, direction2)) {
				return new RoadSegment (RoadType.Straight, coords, direction2);
			} else {
				return CreateTurn (coords, direction1, direction2);
			}
		}

		private RoadSegment CreateTurn (Coordinate coords, Direction direction1, Direction direction2)
		{
			Direction direction = GetRotationAngleForTurnRoad (direction1, direction2);
			return new RoadSegment (RoadType.Turn, coords, direction);
		}

		private RoadSegment CreateTJunction (PathNode[] nodes, PathNode node)
		{
			PathNode neighbour1 = nodes [node.connectedNodes [0]];
			PathNode neighbour2 = nodes [node.connectedNodes [1]];
			PathNode neighbour3 = nodes [node.connectedNodes [2]];

			return CreateTJunction (node.coords, neighbour1.coords, neighbour2.coords, neighbour3.coords);
		}

		private RoadSegment CreateTJunction (Coordinate coords, Coordinate neighbour1, Coordinate neighbour2, Coordinate neighbour3)
		{
			Direction direction1 = RelativeDirection (coords, neighbour1);
			Direction direction2 = RelativeDirection (coords, neighbour2);
			Direction direction3 = RelativeDirection (coords, neighbour3);
			Direction rotationAngle = GetRotationAngleForTJunctionRoad (direction1, direction2, direction3);
			return new RoadSegment (RoadType.TJunction, coords, rotationAngle);
		}

		private RoadSegment CreateCrossRoad (PathNode node)
		{
			return new RoadSegment (RoadType.CrossRoads, node.coords, Direction.North);
		}

		private Direction RelativeDirection (Coordinate subjectCoords, Coordinate otherCoords)
		{
			if (subjectCoords.x < otherCoords.x) {
				return Direction.East;
			} else if (subjectCoords.x > otherCoords.x) {
				return Direction.West;
			} else if (subjectCoords.y < otherCoords.y) {
				return Direction.North;
			} else if (subjectCoords.y > otherCoords.y) {
				return Direction.South;
			}
			// should never get here
			return Direction.North;
		}

		private Direction GetRotationAngleForTurnRoad (Direction direction1, Direction direction2)
		{
			HashSet<Direction> directions = new HashSet<Direction> ();
			directions.Add (direction1);
			directions.Add (direction2);

			if (directions.Contains (Direction.North) && directions.Contains (Direction.East)) {
				return Direction.North;
			} else if (directions.Contains (Direction.South) && directions.Contains (Direction.East)) {
				return Direction.East;
			} else if (directions.Contains (Direction.North) && directions.Contains (Direction.West)) {
				return Direction.West;
			} else if (directions.Contains (Direction.South) && directions.Contains (Direction.West)) {
				return Direction.South;
			}

			return Direction.North;
		}

		private Direction GetRotationAngleForTJunctionRoad (Direction direction1, Direction direction2, Direction direction3)
		{
			HashSet<Direction> directions = new HashSet<Direction> ();
			directions.Add (direction1);
			directions.Add (direction2);
			directions.Add (direction3);
			if (directions.Contains (Direction.North) && directions.Contains (Direction.South)) {
				directions.Remove (Direction.North);
				directions.Remove (Direction.South);
			} else if (directions.Contains (Direction.West) && directions.Contains (Direction.East)) {
				directions.Remove (Direction.West);
				directions.Remove (Direction.East);
			}
			Direction direction = Direction.North;
			foreach (Direction directionValue in directions) {
				direction = directionValue;
			}
			return direction;
		}

		private bool AreOppositeDirections (Direction direction1, Direction direction2)
		{
			HashSet<Direction> directions = new HashSet<Direction> ();
			directions.Add (direction1);
			directions.Add (direction2);
			return (directions.Contains (Direction.North) && directions.Contains (Direction.South)) ||
			(directions.Contains (Direction.West) && directions.Contains (Direction.East));
		}

	}
}