
using UnityEngine;

public static class CLogger
{
    public static void log(string str) { Debug.Log(str); }
    public static void err(string str) { Debug.Log("<color=red>" + str + "</color>"); }
    public static void wrn(string str) { Debug.Log("<color=yellow>" + str + "</color>"); }
}

public static class CDebug
{
    private static int DIVISIONS = 20; 
    private static float deltaAngle = Mathf.PI * 2 / DIVISIONS;
    public static void DrawCircle2D(Vector2 center , float radius , Color color)
    {
        float angle = 0 ;
        for (int i = 0; i < DIVISIONS; i++, angle += deltaAngle)
        {
            Vector2 point1 = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
            Vector2 point2 = new Vector2(radius * Mathf.Cos(angle + deltaAngle), radius * Mathf.Sin(angle + deltaAngle));
            point1 += center;
            point2 += center;  
            Debug.DrawLine(point1, point2); 
        }
        angle = 0; 
    }
    public static void DrawLine(Line line)
    {
        if (line._co_y == 0 && line._co_x != 0)
        {
            //y_intercept = = infinity ;
             Vector2 x_intercept = new Vector2(line._const / line._co_x , -45) ;
             Debug.DrawLine(x_intercept, new Vector2(x_intercept.x , 45)); 
        }
        else if (line._co_x == 0 && line._co_y != 0)
        {
            Vector2 y_intercept = new Vector2(-45, line._const / line._co_y);
            Debug.DrawLine(y_intercept, new Vector2(45, y_intercept.y));
        }
        else
        {
            Vector2 x_intercept = new Vector2(line._const / line._co_x, 0);
            Vector2 y_intercept = new Vector2(0, line._const / line._co_y);
            Debug.DrawLine(x_intercept, y_intercept, Color.white);
        } 
    }
    public static void DrawBounds(Bounds bounds, Vector3 center, Color color)
    {
        Vector3[] points = new Vector3[8];
        points[0] = new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z) + center;
        points[1] = new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z) + center;
        points[2] = new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z) + center;
        points[3] = new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z) + center;
        points[4] = new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z) + center;
        points[5] = new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z) + center;
        points[6] = new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z) + center;
        points[7] = new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z) + center;

        Debug.DrawLine(points[0], points[1], color);
        Debug.DrawLine(points[1], points[2], color);
        Debug.DrawLine(points[2], points[3], color);
        Debug.DrawLine(points[3], points[0], color);
        Debug.DrawLine(points[4], points[5], color);
        Debug.DrawLine(points[5], points[6], color);
        Debug.DrawLine(points[6], points[7], color);
        Debug.DrawLine(points[7], points[4], color);
        Debug.DrawLine(points[0], points[4], color);
        Debug.DrawLine(points[1], points[5], color);
        Debug.DrawLine(points[2], points[6], color);
        Debug.DrawLine(points[3], points[7], color);
    }
}
