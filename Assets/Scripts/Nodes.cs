using System;

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

public class PathNode : Node
{
    public int[] connectedNodes;
}

public class OriginNode : Node
{
    public String direction;
}
