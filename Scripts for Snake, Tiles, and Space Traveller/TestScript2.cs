using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript2 : MonoBehaviour {

    private new SpriteRenderer renderer;

    private Sprite sprite;
    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        sprite = renderer.sprite;
    }

    void Update()
    {
      
    }

    private void DrawDebugBorder(GameObject obj)
    {
        Sprite sprite = obj.GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture; 

        float width = texture.width / sprite.pixelsPerUnit * transform.localScale.x;
        float height = texture.height / sprite.pixelsPerUnit * transform.localScale.y;

        Vector2 point1 = new Vector2(-width * 0.5f, -height * 0.5f) + (Vector2)transform.position;
        Vector2 point2 = new Vector2(-width * 0.5f, height * 0.5f) + (Vector2)transform.position;
        Vector2 point3 = new Vector2(width * 0.5f, height * 0.5f) + (Vector2)transform.position;
        Vector2 point4 = new Vector2(width * 0.5f, -height * 0.5f) + (Vector2)transform.position;

        Debug.DrawLine(point1, point2);
        Debug.DrawLine(point2, point3);
        Debug.DrawLine(point3, point4);
        Debug.DrawLine(point4, point1);
    }
}
