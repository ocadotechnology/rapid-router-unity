using System;
using UnityEngine;

[Serializable]
public class Coordinate: IEquatable<Coordinate>
{
    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Coordinate(Vector3 vector) {
        this.x = (int)vector.x;
        this.y = (int)vector.y;
    }

    public Vector3 vector {
        get {
            return new Vector3(x, y, 0);
        }
    }

    public bool Equals(Coordinate other)
    {
        return this.x == other.x && this.y == other.y;
    }

    public override int GetHashCode() {
        return this.x * 100 + y;
    }

    
    override public String ToString()
    {
        return "x: " + this.x + ", y: " + this.y;
    }
}