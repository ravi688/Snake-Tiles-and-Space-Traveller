using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Snake))]
public class WayPointFollower : MonoBehaviour
{
    public bool isActive = true;
    public bool isDebug = true;
    public bool isRaw = false;
    public bool isStartFollow = false;
    public bool isStopOnComplete = false;
    public Vector2[] wayPoints;

    [SerializeField]
    float wayPointOffset = 0.6f;

    bool isCompleted = false;
    bool isFollowingCurrentWayPoint = false;
    bool isMovingForward = false;
    int currentWayPointIndex = 0;
    int num_WayPoints;
    float sqrDistanceRemained;
    float sgn_angle;

    SnakeAI _AI;
    Snake _controller;
    Vector2 currentWayPoint;

    void ToggleStartFollow(bool is_start) { isStartFollow = is_start; }
    void SetWayPoints(Vector2[] _way_points) { wayPoints = _way_points; }

    private void Awake()
    {
        _controller = GetComponent<Snake>();
        _AI = GetComponent<SnakeAI>();
        num_WayPoints = wayPoints.Length;
        currentWayPointIndex = 0;
    }
    private void Update()
    {
        if (!isActive) return;
        HandleMovement();
        if (isDebug) DrawWayPoints();
        if (isStartFollow && !isFollowingCurrentWayPoint)
        {
            currentWayPoint = wayPoints[currentWayPointIndex];
            isFollowingCurrentWayPoint = true;
            if (!isRaw)
            {
                if (_AI.isActive)
                    _AI.SetTargetPoint(currentWayPoint);
                else
                {
                    _controller.ToggleSmoothTurn(CalculatePath(currentWayPoint), SnakeOrgan.Head);
                    Debug.LogWarning("The AI attached to this Snake is InActive hence Fall back to Raw Following");
                }
            }
            else
                _controller.ToggleSmoothTurn(CalculatePath(currentWayPoint), SnakeOrgan.Head);
        }
        float DistanceRemained = ((_controller.WorldHeadEndPos - currentWayPoint).sqrMagnitude + 1) * 0.5f;
        if (isFollowingCurrentWayPoint && DistanceRemained <= (0.1f + wayPointOffset))
        {
            isFollowingCurrentWayPoint = false;
            currentWayPointIndex++;
            if (isStopOnComplete && currentWayPointIndex == num_WayPoints)
                isStartFollow = false;
        }
    }
    private void DrawWayPoints()
    {
        for (int i = 0; i < num_WayPoints; i++)
            CDebug.DrawCircle2D(wayPoints[i], 0.3f, Color.yellow);
    }

    private void HandleMovement()
    {
        if (!isMovingForward && isStartFollow) { isMovingForward = true; _controller.ToggleMoveForward(true); }
        if (!isStartFollow && isMovingForward) { isMovingForward = false; _controller.ToggleMoveForward(false); }
    }
    private float CalculatePath(Vector2 _wayPoint)
    {
        float sgn_angle = VectorUtility.GetSignedAngle(_controller.SnakeMovingData.WorldHeadDirection, _wayPoint - _controller.WorldHeadEndPos);
        return sgn_angle;
    }
}
