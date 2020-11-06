using System;
using UnityEngine ;

public class Line
{
    public float _co_x;
    public float _co_y;
    public float _const;
    public float slope { get { return _co_x == 0 ? Mathf.Infinity : _co_y / _co_x; } } 
    public Line(float _co_x, float _co_y, float _const)
    {
        this._co_x = _co_x;
        this._co_y = _co_y;
        this._const = _const;
    }
    public Line() : this(0, 1, 0) { }
    public static float Angle(Line line1 , Line line2)
    {
        float slope1 = line1.slope;
        float slope2 = line2.slope;
        return Mathf.Rad2Deg * Mathf.Atan(Mathf.Abs((slope1 - slope2) / (1 + slope2 * slope2))); 
    }
    public static Vector2 Intersection(Line line1, Line line2)
    { 
        float _nominator_y =line1._co_x * line2._const - line2._co_x * line1._const;
        float _denominator = line2._co_x * line1._co_y - line1._co_x * line2._co_y;
        float y = _nominator_y / _denominator;
        float _nominator_x = line2._const * line1._co_y - line2._co_y * line1._const;
        float x = -_nominator_x / _denominator;
        return new Vector2(x, y); 
    }
    public static Line CreateWithPoints(Vector2 point1, Vector2 point2)
    {
        float __co_y = point1.x - point2.x;
        float __co_x = -(point1.y - point2.y);
        float __const = point2.x * (-__co_x) - point2.y * (__co_y);
        return new Line(__co_x, __co_y, __const); 
    }
    public static Line CreateWithPointDir(Vector2 point, Vector2 direction)
    {
        float __co_x = -direction.y;
        float __co_y = direction.x;
        float __const = -direction.x * point.y + point.x * direction.y;
        return new Line(__co_x, __co_y, __const);
    }
}