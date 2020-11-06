using System;
using System.Collections.Generic;
using UnityEngine;


public class Path
{
    public Vector2[] wayPoints;
    public int num_threats;
    public int num_powerUps;
    public float length;
    public Path() { }
}

public class Obstacle
{
    public RaycastHit2D UpperTangentInfo;
    public RaycastHit2D LowerTangentInfo;
    public Vector2 UppetPointOfTangency;
    public Vector2 LowerPonitOfTangency;
    public Collider2D collider;

    public Obstacle() { }
}
public class VectorUtility
{
    private VectorUtility() { }
    public static float GetSignedAngle(Vector2 from, Vector2 to, bool isAntiClwPstv = true)
    {
        float angle = Vector2.Angle(from, to);
        Vector3 _crp_ = Vector3.Cross(from, to);
        angle *= Mathf.Sign(Vector3.Dot(Vector3.forward, _crp_)) * (isAntiClwPstv ? 1 : -1);
        return angle;
    }
    public static Vector2 Rotate(Vector2 vector, float angle)      //+ve for anticlockwise , -ve for clockwise
    {
        angle *= Mathf.Deg2Rad;
        float _x = Mathf.Cos(angle) * vector.x + Mathf.Sin(angle) * vector.y;
        float _y = -Mathf.Sin(angle) * vector.x + Mathf.Cos(angle) * vector.y;
        Vector2 rotated_vector = new Vector2(_x, _y);
        return rotated_vector;
    }
}

public class SnakeAI : MonoBehaviour
{
    [SerializeField]
    private float obstacle_detection_range = 5;
    [SerializeField]
    private bool isDebug = true;
    [SerializeField]
    private int precision = 50;
    [SerializeField]
    private float avoid_angle_offset = 2.0f;
    [SerializeField]
    private float target_reached_distance = 2f;
    [SerializeField]
    private float snake_width = 1.0f;

    public new bool isActive = false;
    public Transform target;


    Snake _controller;
    Collider2D current_obstacle;

    bool isObstacleDetected = false;
    bool isAvoidingObstacleInProcess = false;
    bool isTargetReached = false;

    float angle;
    float delta_angle;

    RaycastHit2D detection_info;
    RaycastHit2D previous_detection_info;
    RaycastHit2D ray_cast_info_path1;
    RaycastHit2D ray_cast_info_path2;
    Vector2 lower_tangent_dir;
    Vector2 ray_cast_dir;
    Vector2 ray_cast_dir_path2;
    Vector2 ray_cast_dir_path1;
    Vector2 previous_target_pos;
    Vector2 target_point;
    Obstacle obs;

    List<Obstacle> obstacles;
    List<Path> paths; 

    private void Awake()
    {
        _controller = GetComponent<Snake>();
        if (isActive && _controller.isControlledByUserInput) { _controller.isControlledByUserInput = false; }
        Physics2D.queriesStartInColliders = false;
        obstacles = new List<Obstacle>();
        delta_angle = 360.0f / precision;
        obs = new Obstacle();
        paths = new List<Path>(); 
    }
    private void Start()
    {
        if (isActive) _controller.ToggleMoveForward(true);
    }
    private void Update()
    {
        if (!isActive) return;

        SetTargetPoint(target.position);

        if (!isObstacleDetected)
            DetectObstacle();
        if (isObstacleDetected)
            GoArroundObstacle(current_obstacle);
        else if (!isTargetReached) Follow(target_point);

        // HandleStopMovement();

        if (isDebug)
        {
            Vector2 center = _controller.WorldHeadEndPos + _controller.SnakeMovingData.WorldHeadDirection.normalized * 0.5f;
            DrawFieldDetection(center, _controller.SnakeMovingData.WorldHeadDirection, 45, 5.0f);
        }
    }
    private void DrawPath(Path path)
    {
        Vector2[] waypoints = path.wayPoints;
        int length = waypoints.Length;
        for (int i = 0; i < length - 1; i++)
            Debug.DrawLine(waypoints[i], waypoints[i + 1]); 
    }
    public void SetTargetPoint(Vector2 point) { target_point = point; }
    private void HandleStopMovement()
    {
        float Sqrdistance_remained = ((Vector2)target.transform.position - _controller.WorldHeadEndPos).sqrMagnitude;
        if (!isTargetReached)
            if (Sqrdistance_remained <= Mathf.Pow(target_reached_distance, 2))
            {
                isTargetReached = true;
                _controller.StopMovement();
            }
    }
    private void DrawFieldDetection(Vector2 center, Vector2 head_direction, float _fieldOfScan, float _detection_radius)
    {
        Vector2 dir1 = VectorUtility.Rotate(head_direction, _fieldOfScan * 0.5f);
        Vector2 dir2 = VectorUtility.Rotate(head_direction, -_fieldOfScan * 0.5f);

        Debug.DrawRay(center, dir1.normalized * _detection_radius, Color.cyan);
        Debug.DrawRay(center, dir2.normalized * _detection_radius, Color.cyan);
        CDebug.DrawCircle2D(center, _detection_radius, Color.white);
        Debug.DrawRay(center, head_direction.normalized * _detection_radius, Color.white);
    }
    //Field Of Scan is in degrees
    private void DetectAllObstaclesInFront(out Obstacle[] out_obstacles, Vector2 center, Vector2 head_direction, float _fieldOfScan, float _detection_radius)
    {
        bool isScanComplete = true;
        delta_angle = _fieldOfScan / precision;
        angle = -_fieldOfScan * 0.5f;
        obstacles = new List<Obstacle>(); 
        for (int i = 0; i < precision; i++, angle += delta_angle)
        {
            ray_cast_dir = VectorUtility.Rotate(head_direction, angle);
            detection_info = Physics2D.Raycast(center, ray_cast_dir.normalized, _detection_radius);
            //This block is called when the scanning obstacle begin
            if (detection_info.collider && detection_info.collider.CompareTag("Obstacle") && isScanComplete)
            {
                isScanComplete = false;
                current_obstacle = detection_info.collider;
                lower_tangent_dir = ray_cast_dir;
                previous_detection_info = detection_info;
            }
            bool isOtherThanObstacle = current_obstacle != null && (!detection_info.collider || !detection_info.collider.CompareTag(current_obstacle.tag));
            bool isSameTagObstacleContinues = current_obstacle != null && (detection_info.collider && (detection_info.collider.name != current_obstacle.name));
            //This block is called when the scanning obstacle ended
            if (isOtherThanObstacle && !isScanComplete || isSameTagObstacleContinues)
            {
                obs.LowerPonitOfTangency = lower_tangent_dir;
                obs.UppetPointOfTangency = ray_cast_dir;
                obs.LowerTangentInfo = previous_detection_info;
                obs.UpperTangentInfo = detection_info;
                obs.collider = current_obstacle;
                obstacles.Add(obs);
                isScanComplete = true;
            }
        }
        if (current_obstacle != null && !isScanComplete)
        {
            obs.LowerPonitOfTangency = lower_tangent_dir;
            obs.UppetPointOfTangency = ray_cast_dir;
            obs.LowerTangentInfo = previous_detection_info;
            obs.UpperTangentInfo = detection_info;
            obs.collider = current_obstacle;
            obstacles.Add(obs);
            isScanComplete = true;
        }
        out_obstacles = obstacles.ToArray();
    }

    private float GetAcuteAngle_RIGHTTRI(Vector2 ANGLE_REQUIRED_POINT, Vector2 RIGHTANGLED_POINT, Vector2 B)
    {
        float w = (B - RIGHTANGLED_POINT).sqrMagnitude;
        float d = (ANGLE_REQUIRED_POINT - RIGHTANGLED_POINT).sqrMagnitude;
        return Mathf.Acos(1 - 2 * w / (d + w)) * Mathf.Rad2Deg;
    }
    //
    //                  /|          w ,d = squared magnitude
    //                 / |
    //                /  |   \/w
    //               /)  |
    //              -----|
    //                \/d
    private float GetAcuteAngle_RIGHTTRI(float _base, float _height)
    {
        return Mathf.Acos(1 - 2 * _height / (_base + _height)) * Mathf.Rad2Deg; 
    }
    private float CalculateSqrPathLength(Vector2[] Path_points)
    {
        int length = Path_points.Length;
        float _path_length = 0;
        for (int i = 0; i < length - 1; i++)
            _path_length += (Path_points[i + 1] - Path_points[i]).sqrMagnitude;
        return _path_length;
    }
    private void Follow(Vector2 _target)
    {
        if (!_controller.isSmoothTurn)
        {
            float angle = VectorUtility.GetSignedAngle(_controller.SnakeMovingData.WorldHeadDirection, _target - _controller.WorldNeckPos);
            _controller.ToggleSmoothTurn(angle, SnakeOrgan.Head);
            previous_target_pos = _target;
        }
    }
    private void GoArroundObstacle(Collider2D obstacle)
    {
        if (!isAvoidingObstacleInProcess)
        {
            angle = 0;
            Vector2 _target_disp_vect = (Vector2)current_obstacle.transform.position - _controller.WorldHeadEndPos;
            //Scan the whole Obstacle from left to right
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            for (int i = 0; i < precision; i++, angle += delta_angle)
            {
                ray_cast_dir_path1 = VectorUtility.Rotate(_target_disp_vect, angle);
                ray_cast_info_path1 = Physics2D.Raycast(_controller.WorldHeadEndPos, ray_cast_dir_path1.normalized, obstacle_detection_range);
                if (!ray_cast_info_path1.collider || !ray_cast_info_path1.collider.CompareTag("Obstacle"))
                    break;
            }
            angle = 0;
            for (int i = 0; i < precision; i++, angle -= delta_angle)
            {
                ray_cast_dir_path2 = VectorUtility.Rotate(_target_disp_vect, -angle);
                ray_cast_info_path2 = Physics2D.Raycast(_controller.WorldHeadEndPos, ray_cast_dir_path2.normalized, obstacle_detection_range);
                if (!ray_cast_info_path2.collider || !ray_cast_info_path2.collider.CompareTag("Obstacle"))
                    break;
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //Calculates the Path length for the two paths for a single Obstacle
            float sqr_dist_path1 = (ray_cast_info_path1.point - _controller.WorldHeadEndPos).sqrMagnitude
                + ((Vector2)target.position - ray_cast_info_path1.point).sqrMagnitude;
            float sqr_dist_path2 = (ray_cast_info_path2.point - _controller.WorldHeadEndPos).sqrMagnitude
                + ((Vector2)target.position - ray_cast_info_path2.point).sqrMagnitude;

            //Take the dicision which path left or right to take based upon shortest path
            float goArroundAngle = 0;
            float _avoidAngleOffset = 0;

            if (sqr_dist_path2 <= sqr_dist_path1)
            {
                _avoidAngleOffset = GetAcuteAngle_RIGHTTRI((_controller.WorldHeadEndPos - ray_cast_info_path2.point).sqrMagnitude, snake_width);
                goArroundAngle = VectorUtility.GetSignedAngle(_controller.SnakeMovingData.WorldHeadDirection, ray_cast_dir_path2);
            }
            else
            {
                _avoidAngleOffset = GetAcuteAngle_RIGHTTRI((_controller.WorldHeadEndPos - ray_cast_info_path1.point).sqrMagnitude, snake_width);
                goArroundAngle = VectorUtility.GetSignedAngle(_controller.SnakeMovingData.WorldHeadDirection, ray_cast_dir_path1);
            }
            goArroundAngle += Mathf.Sign(goArroundAngle) * _avoidAngleOffset;
            goArroundAngle += avoid_angle_offset;
            //Take the Action (Turn to Calculated Angle)
            _controller.ToggleSmoothTurn(goArroundAngle, SnakeOrgan.Head);
            isAvoidingObstacleInProcess = true;
        }
        if (!_controller.isSmoothTurn)
        {
            isAvoidingObstacleInProcess = false;
            isObstacleDetected = false;
            return;
        }
    }
    private void DetectObstacle()
    {
        Vector2 rayDirection = ((Vector2)target.transform.position - _controller.WorldHeadEndPos).normalized;
        Vector2 origin = _controller.WorldHeadEndPos;
        RaycastHit2D hit = Physics2D.Raycast(origin, rayDirection, obstacle_detection_range);
        if (isDebug) Debug.DrawRay(origin, rayDirection * obstacle_detection_range);
        if (hit.collider && hit.collider.CompareTag("Obstacle"))
        {
            current_obstacle = hit.collider;
            isObstacleDetected = true;
        }
        else
        {
            current_obstacle = null;
            isObstacleDetected = false;
        }
    }
}
