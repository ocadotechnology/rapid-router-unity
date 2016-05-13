using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Road
{
    enum RoadType : int
    {
        DeadEnd = 1,
        SingleSegment = 2,
        TJunction = 3,
        CrossRoads = 4
    }

    public class RoadDrawer : MonoBehaviour
    {

        [Inject]
        BoardTranslator translator;

        [Inject]
        Installer.Settings.RoadTiles tiles;

        public GameObject SetupOrigin(OriginNode origin)
        {
            Direction direction = StringToDirection(origin.direction);
            Coordinate coords = origin.coords;
            return Instantiate(tiles.cfcTile, new Vector3(translator.translateRow(coords.x), translator.translateColumn(coords.y), 0f),
            Quaternion.AngleAxis((int)direction, new Vector3(0, 0, 1))) as GameObject;
        }

        public void SetupDestinations(Coordinate[] destinationNodes)
        {

        }

        public HashSet<GameObject> SetupRoadSegments(PathNode[] nodes)
        {
            HashSet<GameObject> gameObjects = new HashSet<GameObject>();
            for (int currentIndex = 0; currentIndex < nodes.Length; currentIndex++)
            {
                gameObjects.Add(SetupRoadSegment(nodes, currentIndex));
            }
            return gameObjects;
        }

        public GameObject SetupRoadSegment(PathNode[] nodes, int currentNodeIndex)
        {
            PathNode node = nodes[currentNodeIndex];
            switch (node.connectedNodes.Length)
            {
                case 1:
                    return DrawDeadEndSegment(nodes, node);
                case 2:
                    return DrawStraightOrTurnSegment(nodes, node);
                case 3:
                    return DrawTJunctionSegment(nodes, node);
                case 4:
                    return DrawCrossRoadSegment(node);
                default:
                    // should never get here
                    return null;
            }
        }

        public GameObject DrawDeadEndSegment(PathNode[] nodes, PathNode node)
        {
            PathNode connectedNode = nodes[node.connectedNodes[0]];
            Direction direction = RelativeDirection(node, connectedNode);
            float x = translator.translateRow(node.coords.x);
            float y = translator.translateColumn(node.coords.y);
            return Instantiate(tiles.deadEndRoadTile, new Vector3(x, y, 0f),
                Quaternion.AngleAxis((int)direction, new Vector3(0, 0, 1))) as GameObject;
        }

        public GameObject DrawStraightOrTurnSegment(PathNode[] nodes, PathNode node)
        {
            PathNode connectedNode1 = nodes[node.connectedNodes[0]];
            PathNode connectedNode2 = nodes[node.connectedNodes[1]];
            Direction direction1 = RelativeDirection(node, connectedNode1);
            Direction direction2 = RelativeDirection(node, connectedNode2);
            if (AreOppositeDirections(direction1, direction2))
            {
                return DrawStraightSegment(node, direction1);
            }
            else
            {
                return DrawTurnSegment(node, direction1, direction2);
            }
        }

        public GameObject DrawStraightSegment(PathNode node, Direction direction)
        {
            float x = translator.translateRow(node.coords.x);
            float y = translator.translateColumn(node.coords.y);
            return Instantiate(tiles.straightRoadTile, new Vector3(x, y, 0f),
                Quaternion.AngleAxis((int)direction, new Vector3(0, 0, 1))) as GameObject;
        }

        public GameObject DrawTurnSegment(PathNode node, Direction direction1, Direction direction2)
        {
            float x = translator.translateRow(node.coords.x);
            float y = translator.translateColumn(node.coords.y);
            int rotationAngle = GetRotationAngleForTurnRoad(direction1, direction2);
            return Instantiate(tiles.turnRoadTile, new Vector3(x, y, 0f),
            Quaternion.AngleAxis(rotationAngle, new Vector3(0, 0, 1))) as GameObject;
        }

        private int GetRotationAngleForTurnRoad(Direction direction1, Direction direction2)
        {
            HashSet<Direction> directions = new HashSet<Direction>();
            directions.Add(direction1);
            directions.Add(direction2);
            int rotationAngle = 0;
            if (directions.Contains(Direction.North) && directions.Contains(Direction.East))
            {
                rotationAngle = (int)Direction.North;
            }
            else if (directions.Contains(Direction.South) && directions.Contains(Direction.East))
            {
                rotationAngle = (int)Direction.East;
            }
            else if (directions.Contains(Direction.North) && directions.Contains(Direction.West))
            {
                rotationAngle = (int)Direction.West;
            }
            else if (directions.Contains(Direction.South) && directions.Contains(Direction.West))
            {
                rotationAngle = (int)Direction.South;
            }
            return rotationAngle;
        }

        public GameObject DrawTJunctionSegment(PathNode[] nodes, PathNode node)
        {
            float x = translator.translateRow(node.coords.x);
            float y = translator.translateColumn(node.coords.y);
            Direction direction1 = RelativeDirection(node, nodes[node.connectedNodes[0]]);
            Direction direction2 = RelativeDirection(node, nodes[node.connectedNodes[1]]);
            Direction direction3 = RelativeDirection(node, nodes[node.connectedNodes[2]]);
            int rotationAngle = GetRotationAngleForTJunctionRoad(direction1, direction2, direction3);
            return Instantiate(tiles.tJunctionTile, new Vector3(x, y, 0f),
            Quaternion.AngleAxis((int)rotationAngle, new Vector3(0, 0, 1))) as GameObject;
        }

        private int GetRotationAngleForTJunctionRoad(Direction direction1, Direction direction2, Direction direction3)
        {
            HashSet<Direction> directions = new HashSet<Direction>();
            directions.Add(direction1);
            directions.Add(direction2);
            directions.Add(direction3);
            if (directions.Contains(Direction.North) && directions.Contains(Direction.South))
            {
                directions.Remove(Direction.North);
                directions.Remove(Direction.South);
            }
            else if (directions.Contains(Direction.West) && directions.Contains(Direction.East))
            {
                directions.Remove(Direction.West);
                directions.Remove(Direction.East);
            }
            Direction direction = Direction.North;
            foreach (Direction directionValue in directions)
            {
                direction = directionValue;
            }
            return (int)direction;
        }

        public GameObject DrawCrossRoadSegment(PathNode node)
        {
            float x = translator.translateRow(node.coords.x);
            float y = translator.translateColumn(node.coords.y);
            return Instantiate(tiles.crossRoadTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
        }

        public bool AreOppositeDirections(Direction direction1, Direction direction2)
        {
            HashSet<Direction> directions = new HashSet<Direction>();
            directions.Add(direction1);
            directions.Add(direction2);
            return (directions.Contains(Direction.North) && directions.Contains(Direction.South)) ||
                (directions.Contains(Direction.West) && directions.Contains(Direction.East));
        }

        public Direction RelativeDirection(PathNode subjectNode, PathNode otherNode)
        {
            if (subjectNode.coords.x < otherNode.coords.x)
            {
                return Direction.West;
            }
            else if (subjectNode.coords.x > otherNode.coords.x)
            {
                return Direction.East;
            }
            else if (subjectNode.coords.y < otherNode.coords.y)
            {
                return Direction.North;
            }
            else if (subjectNode.coords.y > otherNode.coords.y)
            {
                return Direction.South;
            }
            // should never get here
            return Direction.North;
        }

        public Direction StringToDirection(String directionString)
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
