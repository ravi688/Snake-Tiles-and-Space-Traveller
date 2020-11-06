using UnityEngine;
using System;

public class DestroyAble : MonoBehaviour
{
    public static float destroyDistance = 8.0f;

    void Update()
    {
        if ((Mathf.Abs(transform.position.x - Camera.main.transform.position.x)) > destroyDistance
           || (Mathf.Abs(transform.position.y - Camera.main.transform.position.x)) > destroyDistance)
        {
            Destroy(this.gameObject);
        }
    }
}