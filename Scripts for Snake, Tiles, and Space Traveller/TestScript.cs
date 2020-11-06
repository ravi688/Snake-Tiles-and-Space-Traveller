using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticalUtility
{
    public static void DrawRect(Rect rect)
    {
        float width = rect.width;
        float height = rect.height;
        Vector2 center = rect.center;
        Vector2 point1 = new Vector2(width * 0.5f, height * 0.5f) + center;
        Vector2 point2 = new Vector2(width * 0.5f, -height * 0.5f) + center;
        Vector2 point3 = new Vector2(-width * 0.5f, -height * 0.5f) + center;
        Vector2 point4 = new Vector2(-width * 0.5f, height * 0.5f) + center;

        Debug.DrawLine(point1, point2);
        Debug.DrawLine(point2, point3);
        Debug.DrawLine(point3, point4);
        Debug.DrawLine(point4, point1);
        Debug.Log(center); 
    }
}


public class TestScript : MonoBehaviour
{




}
