using System;



[Serializable]
public class Node
{
    public Coordinate coords
    {
        get
        {
            return new Coordinate(this.coordinate[0], this.coordinate[1]);
        }
    }

    public int[] coordinate;
}

[Serializable]
public class PathNode : Node
{
    public int[] connectedNodes;
}

[Serializable]
public class OriginNode : Node
{
    public String direction;
}
