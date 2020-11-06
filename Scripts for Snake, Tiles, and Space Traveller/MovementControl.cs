
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementControl : MonoBehaviour
{

    public float speed = 1.0f;

    private Rigidbody2D body2d;
    private Vector2 move_left_pos;
    private Vector2 move_right_pos;
    private Vector2 move_up_pos;
    private Vector2 move_down_pos;

    private static readonly Vector2 nullvector = Vector2.zero;

    private void Awake()
    {
        body2d = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            move_up_pos =   Vector2.up;
        else move_up_pos = nullvector;
        if (Input.GetKey(KeyCode.DownArrow))
            move_down_pos =   Vector2.down;
        else move_down_pos = nullvector;
        if (Input.GetKey(KeyCode.LeftArrow))
            move_left_pos =  Vector2.left;
        else move_left_pos = nullvector;
        if (Input.GetKey(KeyCode.RightArrow))
            move_right_pos =  Vector2.right;
        else move_right_pos = nullvector;
    }
    private void OnCollisionStay2D(Collision2D colInfo)
    {
        ContactPoint2D[] contacts = colInfo.contacts;
        int length = contacts.Length;
        for (int i = 0; i < length; i++)
        {
            CDebug.DrawCircle2D(colInfo.contacts[i].point, 0.3f, Color.white);
        } 
    }
    private void FixedUpdate()
    {
        body2d.MovePosition((move_right_pos + move_left_pos + move_up_pos + move_down_pos).normalized * Time.deltaTime * speed + (Vector2)transform.position);
    }
}
