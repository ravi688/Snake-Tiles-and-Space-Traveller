using UnityEngine;

public class SnakeRuntimeData
{
    private Snake snake;
    public Vector2 LocalHeadDirection { get; private set; }
    public Vector2 LocalTailDirection { get; private set; }
    public Vector2 LocalHeadBodyDirection { get; private set; }
    public Vector2 LocalTailBodyDirection { get; private set; }
    public float TailRotatedAngle { get; private set; }                      //With Respect to the sneck's body
    public float HeadRotatedAngle { get; private set; }                      //With Respect to the sneck's body
    public Direction currentToggleHeadDirection { get; private set; }
    public Direction currentToggleTailDirection { get; private set; }
    public Vector2 WorldHeadDirection { get { return snake.transform.TransformDirection(LocalHeadDirection); } }
    public Vector2 WorldTailDirection { get { return snake.transform.TransformDirection(LocalTailDirection); } }
    public Vector2 WorldTailBodyDirection { get { return snake.transform.TransformDirection(LocalTailBodyDirection); } }
    public Vector2 WorldHeadBodyDirection { get { return snake.transform.TransformDirection(LocalHeadBodyDirection); } }
    //Note : Here We can't Derive from Snake  , since by the definition of the Inheritance 
    //In Inheritance the blueprint of the base class is inhereted to the derived class 
    //Eventually You will get the NullReferenceException
    public SnakeRuntimeData(Snake snake) { this.snake = snake; }
    //Setters
    public void SetLocalHeadDirection(Vector2 dir) { LocalHeadDirection = dir; }
    public void SetLocalTailDirection(Vector2 dir) { LocalTailDirection = dir; }
    public void SetLocalHeadPivotBody(Vector2 dir) { LocalHeadBodyDirection = dir; }
    public void SetLocalTailPivotBody(Vector2 dir) { LocalTailBodyDirection = dir; }
    public void SetTailRotatedAngle(float angle) { TailRotatedAngle = angle; }
    public void SetHeadRotatedAngle(float angle) { HeadRotatedAngle = angle; }
    public void SetCurrentToggleHeadDirection(Direction enum_dir) { currentToggleHeadDirection = enum_dir; }
    public void SetCurrentToggleTailDirection(Direction enum_dir) { currentToggleTailDirection = enum_dir; }
}