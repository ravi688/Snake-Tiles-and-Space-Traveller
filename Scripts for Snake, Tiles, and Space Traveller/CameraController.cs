using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    private float FollowSpeed = 1.0f;
    [SerializeField]
    private Transform target; 
    private Vector2 offset; 
	// Use this for initialization
	void Start () {
        offset = transform.position - target.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {

        if (!target) return;
       Vector2 pos = Vector2.Lerp((Vector2)transform.position,(Vector2) target.position + offset , Time.deltaTime * FollowSpeed );
       transform.position = new Vector3(pos.x, pos.y, transform.position.z);
	}
}
