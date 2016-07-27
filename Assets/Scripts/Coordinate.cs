using System;
using UnityEngine;

[Serializable]
public class Coordinate
{
    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector3 vector {
        get {
            return new Vector3(x, y, 0);
        }
    }
}