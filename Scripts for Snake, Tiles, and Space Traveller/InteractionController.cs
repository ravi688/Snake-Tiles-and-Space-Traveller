using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
[RequireComponent(typeof(Camera))]
public class InteractionController : MonoBehaviour
{

    private Grid grid;
    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        if (Input.mousePresent)
        {
            Camera cam = GetComponent<Camera>();
            Vector2 mousepos = cam.ScreenToWorldPoint(Input.mousePosition);
           // DebugPoint(cam, mousepos);
            if (Input.GetMouseButton(0))
            {
                Tile tile = grid.GetTileInRange(mousepos);
                if (tile)
                    tile.On();
            }
        }
    }
    private void DebugPoint(Camera cam, Vector2 point)
    {
        Vector3 point1 = cam.transform.TransformPoint(new Vector3(point.x, 45, 0.5f));
        Vector3 point2 = cam.transform.TransformPoint(new Vector3(point.x, -45, 0.5f));
        Vector3 point3 = cam.transform.TransformPoint(new Vector3(45, point.y, 0.5f));
        Vector3 point4 = cam.transform.TransformPoint(new Vector3(-45, point.y, 0.5f));
        Debug.DrawLine(point1, point2);
        Debug.DrawLine(point3, point4);
    }


}
