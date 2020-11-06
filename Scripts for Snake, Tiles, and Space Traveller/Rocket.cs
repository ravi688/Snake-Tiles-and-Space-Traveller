using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private float FallAnglurForceFactor = 1.0f;
    [SerializeField]
    private float ThrustAngularForceFactor = 2.0f;
    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private GameObject ExplodeRocketParticle; 
    public static Rocket RocketInstance; 

    private Rigidbody2D body;
    private float temp_dot = 0;
    private void Awake()
    {
        if (!RocketInstance) { RocketInstance = this; } 
        body = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
            temp_dot = Vector3.Dot(Vector3.Cross(transform.right , Vector3.down) , Vector3.forward);
        if (Input.GetKey(KeyCode.UpArrow))
            ApplyThrust(); 
        else
            ApplyGravity();
        ApplyPositionUpdate(); 
    }
    private void ApplyPositionUpdate()
    {
        transform.position += speed * transform.right * Time.deltaTime; 
    }
    private void ApplyThrust()
    {
        transform.rotation = FromToRotation(transform.rotation, -ThrustAngularForceFactor * Mathf.Sign(temp_dot) * Time.deltaTime); 
    }
    private void ApplyGravity()
    {
        float dot = Vector3.Dot(Vector3.Cross(transform.TransformDirection(Vector3.right), Vector3.down), Vector3.forward);
        float angle = Vector2.Angle(Vector2.down, transform.TransformDirection(Vector3.right));
        float _angle = angle * FallAnglurForceFactor * Time.deltaTime * Mathf.Sign(dot);
        transform.rotation = FromToRotation(transform.rotation, _angle);
    }
    private Quaternion FromToRotation(Quaternion from, float angle)
    {
        Quaternion rotor = Quaternion.AngleAxis(angle, Vector3.forward);
        return from * rotor;
    }

    private Vector2 rotateVector(Vector2 vector, float angle)
    {
        Quaternion rotor = Quaternion.AngleAxis(angle * 2, Vector3.forward);
        Quaternion quat = new Quaternion(vector.x, vector.y, 0, 0) * rotor;
        return new Vector2(quat.x, quat.y);
    }
    public void ExplodeRocket()
    {
      GameObject obj =   Instantiate(ExplodeRocketParticle, transform.position, Quaternion.identity);
      Destroy(obj, 2.0f);
      Destroy(this.gameObject); 
    }
}
