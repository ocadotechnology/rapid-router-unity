using System;
using UnityEngine;

[Serializable]
public class Coordinate: IEquatable<Coordinate>
{
    public float x;
    public float y;

    public Coordinate(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public Coordinate(Vector3 vector) {
        this.x = (float)Math.Round(vector.x, 2);
        this.y = (float)Math.Round(vector.y, 2);
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
        return (int)(this.x * 100 + this.y * 100);
    }

    
    override public String ToString()
    {
        return "x: " + this.x + ", y: " + this.y;
    }
}