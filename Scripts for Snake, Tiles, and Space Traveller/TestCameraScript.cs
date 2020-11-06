using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraScript : MonoBehaviour {

    [SerializeField]
    private float speed = 1.0f; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position  = (Vector2)transform.position +  Vector2.right * speed * Time.deltaTime; 
	}
}
