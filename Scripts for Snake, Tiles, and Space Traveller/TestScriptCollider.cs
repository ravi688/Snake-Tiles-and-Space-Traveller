using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptCollider : MonoBehaviour {


    new BoxCollider2D collider;
    Vector3 position;
    Color white; 
    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        position = transform.position;
        white = Color.white; 
    }

    private void Update()
    {
        CDebug.DrawBounds(collider.bounds , position , white); 
    }

}

