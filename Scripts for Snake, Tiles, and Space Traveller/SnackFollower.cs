using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnackFollower : MonoBehaviour {

    [SerializeField]
    private float followSpeed = 1.0f;
    public Snake snake;

    private Vector3 targetPoint;
    void LateUpdate()
    {
        targetPoint = (snake.WorldHeadEndPos + snake.WorldTailEndPos) * 0.5f;
       transform.position = Vector3.Lerp(transform.position, new Vector3(targetPoint.x , targetPoint.y , transform.position.z) , Time.deltaTime * followSpeed); 
    }
}
