using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Tile : MonoBehaviour {

    public UnityAction OnTileOn;
    public UnityAction OnTileOff; 
   
    private new BoxCollider2D collider;
    private new SpriteRenderer renderer;
    private Color untoggledColor;
    private Color toggleColor = Color.white;

    public void SetUntoggleColor(Color color) { untoggledColor = color; } 
    public void SetToggleColor(Color color) { toggleColor = color; }

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        if (!GetComponent<BoxCollider2D>())
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            float width = renderer.sprite.texture.width * renderer.sprite.pixelsPerUnit * transform.localScale.x;
            float height = renderer.sprite.texture.height * renderer.sprite.pixelsPerUnit * transform.localScale.y;
            collider.size = new Vector2(width, height);
            collider.offset = Vector2.zero; 
        }
    }
    public void On()
    {
        collider.enabled = true;
        renderer.color = toggleColor;
        if(OnTileOn != null) OnTileOn(); 
    }
    public void Off()
    {
        collider.enabled = false;
        renderer.color = untoggledColor;
        if(OnTileOff != null) OnTileOff(); 
    }

}
