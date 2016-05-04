using System;
using System.Collections.Generic;
using UnityEngine;

enum Direction : int
{
    North = 0,
    East = 90,
    South = 180,
    West = 270
};

enum RoadType : int
{
    DeadEnd = 1,
    SingleSegment = 2,
    TJunction = 3,
    CrossRoads = 4
}

[Serializable]
public class Node {
    public Coordinate coords {
        get
        {
            return new Coordinate(this.coordinate[0], this.coordinate[1]);
        }
    }

    public int[] coordinate;
}

[Serializable]
public class PathNode: Node {
    public int[] connectedNodes;
}

[Serializable]
public class OriginNode: Node {
    public String direction;
}

[Serializable]
public class Coordinate {
    public int x;
    public int y;
    
    public Coordinate(int x, int y) {
        this.x = x;
        this.y = y;
    }
}

[Serializable]
public class Level {
    public PathNode[] path;
    public OriginNode origin;
    public Coordinate[] destinationCoords {
        get
        {
            Coordinate[] coordinates = new Coordinate[destinations.Length];
            for (int i = 0; i < destinations.Length; i++)
            {
                int[] dest = destinations[i];
                coordinates[i] = new Coordinate(dest[0], dest[1]);
            }
            return coordinates;
        }
    }

    public int[][] destinations;
}

public class BoardManager : MonoBehaviour
{

    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public float rows;
    public float columns;

    public GameObject grassTile;
    public GameObject straightRoadTile;
    public GameObject deadEndRoadTile;
    public GameObject turnRoadTile;
    public GameObject tJunctionTile;
    public GameObject crossRoadTile;
    public GameObject cfcTile;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    float halfRows;
    float halfColumns;

    float translateRow(float x)
    {
        return x - halfRows;
    }

    float translateColumn(float y)
    {
        return y - halfColumns;
    }

    void InitialiseList()
    {
        gridPositions.Clear();
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    private void SetupLevel() {
        Level level = LoadJSONFile();
        SetupBoard();
        SetupOrigin(level.origin);
        SetupRoadSegments(level.path);
        SetupDestinations(level.destinationCoords);
    }
    
    private void SetupBoard()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {

                GameObject grassObject = Instantiate(grassTile, new Vector3(translateRow(x), translateColumn(y), 0f), Quaternion.identity) as GameObject;
                grassObject.transform.SetParent(boardHolder);
            }
        }
    }

    void SetupRoadSegments(PathNode[] nodes)
    {
        for (int currentIndex = 0; currentIndex < nodes.Length; currentIndex++) {
            SetupRoadSegment(nodes, currentIndex);
        }
    }

    void SetupOrigin(OriginNode origin)
    {
        Direction direction = StringToDirection(origin.direction);
        Coordinate coords = origin.coords;
        GameObject cfcObject = Instantiate(cfcTile, new Vector3(translateRow(coords.x), translateColumn(coords.y), 0f), 
            Quaternion.AngleAxis((int)direction, new Vector3(0, 0, 1))) as GameObject;
        cfcObject.transform.SetParent(boardHolder);
    }

    void SetupDestinations(Coordinate[] destinationNodes)
    {
        
    }

    private void SetupRoadSegment(PathNode[] nodes, int currentNodeIndex)
    {
        PathNode node = nodes[currentNodeIndex];
        switch (node.connectedNodes.Length)
        {
            case 1:
                DrawDeadEndSegment(nodes, node);
                break;
            case 2:
                DrawStraightOrTurnSegment(nodes, node);
                break;
            case 3:
                DrawTJunctionSegment(nodes, node);
                break;
            case 4:
                DrawCrossRoadSegment(node);
                break;
            default:
                break;
        }
    }
    
    private void DrawDeadEndSegment(PathNode[] nodes, PathNode node) {
        PathNode connectedNode = nodes[node.connectedNodes[0]];
        Direction direction = RelativeDirection(node, connectedNode);
        float x = translateRow(node.coords.x);
        float y = translateColumn(node.coords.y);
        GameObject deadEndRoadObject = Instantiate(deadEndRoadTile, new Vector3(x, y, 0f), 
            Quaternion.AngleAxis((int)direction, new Vector3(0, 0, 1))) as GameObject;
        deadEndRoadObject.transform.SetParent(boardHolder);
    }
    
    private void DrawStraightOrTurnSegment(PathNode[] nodes, PathNode node) {
        PathNode connectedNode1 = nodes[node.connectedNodes[0]];
        PathNode connectedNode2 = nodes[node.connectedNodes[1]];
        Direction direction1 = RelativeDirection(node, connectedNode1);
        Direction direction2 = RelativeDirection(node, connectedNode2);
        if (AreOppositeDirections(direction1, direction2)) {
            DrawStraightSegment(node, direction1);
        } else {
            DrawTurnSegment(node, direction1, direction2);
        }
    }
   
   private void DrawStraightSegment(PathNode node, Direction direction) {
        float x = translateRow(node.coords.x);
        float y = translateColumn(node.coords.y);
        GameObject straightRoadObject = Instantiate(straightRoadTile, new Vector3(x, y, 0f),
            Quaternion.AngleAxis((int)direction, new Vector3(0, 0, 1))) as GameObject;
        straightRoadObject.transform.SetParent(boardHolder);
    }
   
   private void DrawTurnSegment(PathNode node, Direction direction1, Direction direction2) {
        float x = translateRow(node.coords.x);
        float y = translateColumn(node.coords.y);
        int rotationAngle = GetRotationAngleForTurnRoad(direction1, direction2);
        GameObject turnRoadObject = Instantiate(turnRoadTile, new Vector3(x, y, 0f),
        Quaternion.AngleAxis(rotationAngle, new Vector3(0, 0, 1))) as GameObject;
        turnRoadObject.transform.SetParent(boardHolder);
    }
    
    private int GetRotationAngleForTurnRoad(Direction direction1, Direction direction2) {
        HashSet<Direction> directions = new HashSet<Direction>();
        directions.Add(direction1);
        directions.Add(direction2);
        int rotationAngle = 0;
        if (directions.Contains(Direction.North) && directions.Contains(Direction.East)) {
            rotationAngle = (int)Direction.North;
        } else if (directions.Contains(Direction.South) && directions.Contains(Direction.East)) {
            rotationAngle = (int)Direction.East;
        } else if (directions.Contains(Direction.North) && directions.Contains(Direction.West)) {
            rotationAngle = (int)Direction.West;
        } else if (directions.Contains(Direction.South) && directions.Contains(Direction.West)) {
            rotationAngle = (int)Direction.South;
        }
        return rotationAngle;
    }
    
    private void DrawTJunctionSegment(PathNode[] nodes, PathNode node) {
        float x = translateRow(node.coords.x);
        float y = translateColumn(node.coords.y);
        Direction direction1 = RelativeDirection(node, nodes[node.connectedNodes[0]]);
        Direction direction2 = RelativeDirection(node, nodes[node.connectedNodes[1]]);
        Direction direction3 = RelativeDirection(node, nodes[node.connectedNodes[2]]);
        HashSet<Direction> directions = new HashSet<Direction>();
        directions.Add(direction1);
        directions.Add(direction2);
        directions.Add(direction3);
        if (directions.Contains(Direction.North) && directions.Contains(Direction.South)) {
            directions.Remove(Direction.North);
            directions.Remove(Direction.South);
        } else if (directions.Contains(Direction.West) && directions.Contains(Direction.East)) {
            directions.Remove(Direction.West);
            directions.Remove(Direction.East);
        }
        Direction direction = Direction.North;
        foreach (Direction directionValue in directions)
        {
            direction = directionValue;
        }
        GameObject tJunctionObject = Instantiate(tJunctionTile, new Vector3(x, y, 0f),
        Quaternion.AngleAxis((int)direction, new Vector3(0, 0, 1))) as GameObject;
        tJunctionObject.transform.SetParent(boardHolder);
    }
    
    private void DrawCrossRoadSegment(PathNode node) {
        float x = translateRow(node.coords.x);
        float y = translateColumn(node.coords.y);
        GameObject crossRoadObject = Instantiate(crossRoadTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
        crossRoadObject.transform.SetParent(boardHolder);
    }
    
    private bool AreOppositeDirections(Direction direction1, Direction direction2) {
        HashSet<Direction> directions = new HashSet<Direction>();
        directions.Add(direction1);
        directions.Add(direction2);
        return (directions.Contains(Direction.North) && directions.Contains(Direction.South)) ||
            (directions.Contains(Direction.West) && directions.Contains(Direction.East));
    }
    
    private Direction RelativeDirection(PathNode subjectNode, PathNode otherNode) {
        if (subjectNode.coords.x < otherNode.coords.x) {
            return Direction.West;
        } else if (subjectNode.coords.x > otherNode.coords.x) {
            return Direction.East;
        } else if (subjectNode.coords.y < otherNode.coords.y) {
            return Direction.North;
        } else if (subjectNode.coords.y > otherNode.coords.y) {
            return Direction.South;
        }
        // should never get here
        return Direction.North;
    }

    private Direction StringToDirection(String directionString)
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

    public void SetupScene(int level)
    {
        halfColumns = columns / 2f;
        halfRows = rows / 2f;
        InitialiseList();
        boardHolder = new GameObject("Board").transform;
        SetupLevel();
    }


    // Use this for initialization
    public Level LoadJSONFile()
    {
        TextAsset levelTextAsset = (TextAsset)Resources.Load("Levels/1", typeof(TextAsset));
        string levelText = levelTextAsset.text;
        return JsonUtility.FromJson<Level>(levelText);
    }


}
