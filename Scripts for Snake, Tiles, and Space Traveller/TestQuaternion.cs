using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public float x;
    public float y;
    public float z;

    public Point(float x, float y, float z) { this.x = x; this.y = y; this.z = z; } 
    public Point() : this(0 , 0 ,0)  { }
    public Point(Vector3 vector)  { this.x = vector.x ;this.y = vector.y ; this.z = vector.z ;  } 
    public Vector3 ToVector() { return new Vector3(x, y, z); }
    public Point Subtract(Point point) { x -= point.x; y -= point.y; return this;  }
    public Point Add(Point point) { x += point.x; y += point.y; return this; }
    public static Point RotatePoint(Point point, Point pivot, float angle)
    {
        Quaternion rotor = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector3 vector = rotor * (point.Subtract(pivot)).ToVector();
        Point new_point = (new Point(vector)).Add(pivot);
        return new_point;
    }
}
public class TestQuaternion : MonoBehaviour {

    public Transform pivotTransform;
    public float angularSpeed = 30; 

    private void Update()
    {
        if (Input.GetKey(KeyCode.Y))
        {
            Point point = Point.RotatePoint(new Point(transform.position), new Point(pivotTransform.position), angularSpeed * Time.deltaTime);
            transform.position = point.ToVector(); 
        }
    }
}
