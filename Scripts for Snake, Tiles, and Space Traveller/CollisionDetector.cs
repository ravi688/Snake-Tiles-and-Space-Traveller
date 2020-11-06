using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{


    private void Start()
    {

    }

    private void OnCollisionEnter2D(Collision2D colliderinfo)
    {
        if (colliderinfo.collider.name == "EdgeCollider")
            Debug.Log("Collided with the edge collider");
        Debug.Log("Hitted");
    }
}