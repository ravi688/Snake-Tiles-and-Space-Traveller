using System;
using UnityEngine; 
namespace CMath
{
    public class GraphTransform
    {
        public static float Concave(float value)
        {
            return value * value;
        }
        public static float Concave(float value, float concaveOrder)
        {
            if (concaveOrder < 1.0) Debug.LogWarning("Concave Order is Below 1.0 High Performance Loss!"); 
            return Mathf.Pow(value, Mathf.Clamp(concaveOrder , 1, Mathf.Infinity));
        }
        public static float Convex(float value)
        {
            return Mathf.Sqrt(value); 
        }
        public static float Convex(float value, float convexOrder)
        {
            return Mathf.Pow(value, 1 / (convexOrder == 0 ? 1 : convexOrder)); 
        }
        public static Vector2 Convex(Vector2 value, float convexOrder)
        {
            return new Vector2(Convex(value.x, convexOrder), Convex(value.y, convexOrder)); 
        }
        public static Vector2 Convex(Vector2 value)
        {
            return new Vector2(Convex(value.x), Convex(value.y));
        }
    }
} 
