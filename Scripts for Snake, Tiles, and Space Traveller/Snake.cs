using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SnakeOrgan
{
    Head,
    Tail
}
public enum Direction
{
    front,
    back,
    left,
    right
}
[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Snake : MonoBehaviour
{
    [SerializeField]
    private SnakeConstructionData SnakeData;
    public bool isControlledByUserInput = true;
    public SnakeRuntimeData SnakeMovingData { get; private set; }
    public Vector2 WorldHeadEndPos { get { return this.transform.TransformPoint(runtime_positions.GetValue(runtime_positions.length - 1)); } }
    public Vector2 WorldTailEndPos { get { return this.transform.TransformPoint(runtime_positions.GetValue(0)); } }
    public Vector2 WorldNeckPos { get { return this.transform.TransformPoint(runtime_positions.GetValue(runtime_positions.length - 2)); } }
    public Direction currentToggleHeadDirection { get; private set; }
    public Direction currentToggleTailDirection { get; private set; }
    [HideInInspector]
    public CQueue<Vector2> runtime_positions;
    public bool isDebugDraw = false;
    public bool isMoveAtAwake = true;
    [HideInInspector]
    public float offset;
    [HideInInspector]
    public Vector2[] static_positions;

    //Variable Declarations which are not accessible in Outer Scripts
    private new LineRenderer renderer;
    private int growLengthSmoothVertices;
    private int numGrownSmoothVertices;
    private float moveTiming;
    private float lengthGrowTiming;
    private float eachPointCreateTime;
    private float eachVertexGrowTime;
    private bool isDataInitialized = false;
    public bool isSmoothTurn { get; private set; }
    private bool isRecontruct = true;
    private bool isGrowLength = false;
    private bool isToggleMoveForward = false;
    private bool isToggleMoveBackward = false;
    private Direction HeadDirToToggle = Direction.front;
    private Direction TailDirToToggle = Direction.front;
    private Direction LengthGrowDirection = Direction.front;
    private Vector2 initialHeadBodyDirection;
    private Vector2 initialTailBodyDirection;
    private float timing;
    private float smoothTurnAngle;
    private SnakeOrgan smoothTurnOrgan;
    private EdgeCollider2D Collider;
    private float previous_angle_rotated;

    private Vector2[] col_vertices;
    private int length;
    private CircleCollider2D head_collider;

    private void Awake()
    {
        renderer = GetComponent<LineRenderer>();
        Collider = GetComponent<EdgeCollider2D>();
        SnakeMovingData = new SnakeRuntimeData(this);
        runtime_positions = new CQueue<Vector2>(SnakeData.numberOfTurningPoints);
        currentToggleHeadDirection = Direction.front;
        currentToggleTailDirection = Direction.front;
        ContructSnake();
        UpdateColliderData();
        UpdateSnakeDynamics();
        if (isMoveAtAwake)
            ToggleMoveForward(true);
    }
    private void OnValidate()
    {
        if (SnakeData.numberOfTurningPoints < 4)
        {
            CLogger.err("The number of turning points in Snake can't be below 4");
            SnakeData.numberOfTurningPoints = 4;
        }
        if (SnakeData.tailPivotIndex == 0)
        {
            CLogger.err("The TailPivotIndex can't be vanished");
            SnakeData.tailPivotIndex = 1;
        }
        if (SnakeData.headPivotIndex == SnakeData.numberOfTurningPoints - 1)
        {
            CLogger.err("The HeadPivotIndex can't be at extreme");
            SnakeData.headPivotIndex = SnakeData.numberOfTurningPoints - 2;
        }
    }
    private void Update()
    {
        // Debug.Log("Snake Controller is called");
        if (isRecontruct) { ContructSnake(); return; }
        if (isDebugDraw)
            DebugDraw();
        if (currentToggleHeadDirection != HeadDirToToggle)
            ChangeDirection(HeadDirToToggle, SnakeOrgan.Head);
        if (currentToggleTailDirection != TailDirToToggle)
            ChangeDirection(TailDirToToggle, SnakeOrgan.Tail);
        if (isSmoothTurn)
            SmoothTurn();
        if (isControlledByUserInput)
            HandleUserInputMovement();
        if (isGrowLength)
            GrowLengthSmooth();

        if (isToggleMoveForward)
            MoveForward();
        else
            if (isToggleMoveBackward)
                MoveBackward();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //These functions are accessible in Outer Scripts
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //Function for Turning the organ AntiClockwise by Time , Smoothly
    public void RotateOrganAntiClockwise(SnakeOrgan organ)
    {
        float angle = 0;
        switch (organ)
        {
            case SnakeOrgan.Tail:
                angle = SnakeData.tailTurnAngularSpeed * Time.deltaTime;
                TurnSnakeOrgan(angle, SnakeOrgan.Tail);
                break;
            case SnakeOrgan.Head:
                angle = -SnakeData.headTurnAngularSpeed * Time.deltaTime;
                TurnSnakeOrgan(angle, SnakeOrgan.Head);
                break;
        }
    }
    //Function for Turning the organ Clockwise by Time , Smoothly
    public void RotateOrganClockwise(SnakeOrgan organ)
    {
        float angle = 0;
        switch (organ)
        {
            case SnakeOrgan.Head:
                angle = SnakeData.headTurnAngularSpeed * Time.deltaTime;
                TurnSnakeOrgan(angle, SnakeOrgan.Head);
                break;
            case SnakeOrgan.Tail:
                angle = -SnakeData.tailTurnAngularSpeed * Time.deltaTime;
                TurnSnakeOrgan(angle, SnakeOrgan.Tail);
                break;
        }
    }
    //Function for Turning the organ by some angle in a single Frame
    public void RotateOrganRaw(float angle, SnakeOrgan organ)      //in degrees , -ve => Clockwise , +ve => AntiClockwise
    {
        float deltaAngle = 0;
        switch (organ)
        {
            case SnakeOrgan.Head:
                deltaAngle = angle / (SnakeData.numberOfTurningPoints - (SnakeData.headPivotIndex + 1));
                break;
            case SnakeOrgan.Tail:
                deltaAngle = angle / SnakeData.tailPivotIndex;
                break;
        }
        TurnSnakeOrgan(deltaAngle, organ);
    }
    //Function for Growing the Length of the Snake by some new_vertices in a single Frame
    public void GrowLengthRaw(int numNewVertices, Direction dir = Direction.front)
    {
        switch (dir)
        {
            case Direction.front:
                int previous_size = runtime_positions.length;
                runtime_positions.growTop(numNewVertices);
                int new_size = runtime_positions.length;
                for (int i = previous_size; i < new_size; i++)
                {
                    Vector2 new_vertex = SnakeMovingData.LocalHeadDirection.normalized * offset * (i - previous_size + 1)
                        + runtime_positions.GetValue(previous_size - 1);
                    runtime_positions.SetValue(i, new_vertex);
                }
                break;
            case Direction.back:
                runtime_positions.growBottom(numNewVertices);
                for (int i = (numNewVertices - 1), j = 1; i >= 0; i--, j++)
                {
                    Vector2 new_vertex = SnakeMovingData.LocalTailDirection.normalized * offset * j +
                        runtime_positions.GetValue(numNewVertices);
                    runtime_positions.SetValue(i, new_vertex);
                }
                break;
            default:
                CLogger.err("Not Defined direction Exception");
                break;
        }
        SnakeData.headPivotIndex += numNewVertices;
        SnakeData.numberOfTurningPoints = runtime_positions.length;
        SnakeData.length = (runtime_positions.length - 1) * offset;
        UpdateColliderData();
        UpdateSnakeDynamics();
    }

    //Function for Outer Scripts , for Moving the Snake backward , must be called each frame
    public void MoveBackward()
    {
        if (!isDataInitialized)
        {
            Debug.Log("<color=red>MovementData isn't Initialized please Call the function InitializeMovementData()</color>");
            return;
        }
        if (Time.time - moveTiming >= eachPointCreateTime)
        {
            MoveOnePointBackward();
            moveTiming = Time.time;
        }
    }
    //Function for Outer Scripts , for Moving the Snake forward , must be called each frame
    public void MoveForward()
    {
        if (!isDataInitialized)
        {
            Debug.Log("<color=red>MovementData isn't Initialized please Call the function InitializeMovementData()</color>");
            return;
        }
        if (Time.time - moveTiming >= eachPointCreateTime)
        {
            MoveOnePointForward();
            moveTiming = Time.time;
        }
    }
    //Function for Outer Scripts , Intializeing the Movement Data when the Input for Movement is Pressed , must be called
    public void InitailizeMovementData()
    {
        moveTiming = Time.time;
        eachPointCreateTime = (float)1 / (float)SnakeData.moveVerticesPerSecond;
        isDataInitialized = true;
    }
    //Function for Outer Scritps ,DeInitializing the Movement Data  When the Input for Movement is Released , must be called 
    public void DeInitializeMovementData()
    {
        isDataInitialized = false;
    }
    //Function for Outer Scripts to access , and Toggling the SmoothGrowlength
    public void ToggleSmoothGrowLength(int numNewVertces, Direction dir = Direction.front)
    {
        growLengthSmoothVertices = numNewVertces;
        LengthGrowDirection = dir;
        isGrowLength = true;
        lengthGrowTiming = Time.time;
        numGrownSmoothVertices = 0;
        eachVertexGrowTime = 1 / SnakeData.lengthGrowVerticesPerSecond;
    }
    //Function for Outer Scripts to access , and Toggling to Change the Direction of a organ left , right or front
    public void ToggleDirection(Direction dir, SnakeOrgan organ)
    {
        switch (organ)
        {
            case SnakeOrgan.Head:
                HeadDirToToggle = dir;
                initialHeadBodyDirection = SnakeMovingData.LocalHeadBodyDirection;
                break;
            case SnakeOrgan.Tail:
                TailDirToToggle = dir;
                initialTailBodyDirection = SnakeMovingData.LocalTailBodyDirection;
                break;
        }
    }
    //Function for Outer Scripts to access , and Toggling Smooth Turn a SnakeOrgan by some Angle Smoothly
    public void ToggleSmoothTurn(float angle, SnakeOrgan organ)
    {
        smoothTurnAngle = angle;
        previous_angle_rotated = 0;
        timing = Time.time;
        smoothTurnOrgan = organ;
        isSmoothTurn = true;
    }
    //Function for Outer Scripts to access , and Toggling the Forward Movement of the Snake , such as Automated Controls
    public void ToggleMoveForward(bool isToggle)
    {
        if (isToggle)
        {
            isToggleMoveForward = true;
            InitailizeMovementData();
            if (isToggleMoveBackward) isToggleMoveBackward = false;
        }
        else
        {
            isToggleMoveForward = false;
            DeInitializeMovementData();
        }
    }
    //Function for Outer Scripts to access , and Toggling the Backward Movement of the Snake , such as Automated Controls
    public void ToggleMoveBackward(bool isToggle)
    {
        if (isToggle)
        {
            isToggleMoveBackward = true;
            InitailizeMovementData();
            if (isToggleMoveForward) isToggleMoveForward = false;
        }
        else
        {
            isToggleMoveBackward = false;
            DeInitializeMovementData();
        }
    }
    public void StopMovement()
    {
        isToggleMoveBackward = false;
        isToggleMoveForward = false;
        DeInitializeMovementData();
    }

    //Function for Loading the Contruction Data  , useful for creating many different Type of Snakes for different Levels
    public void LoadContructionData(SnakeConstructionData data) { SnakeData = data; }
    //Function  , Must be called just after loading the contruction data to recontruct the snake based on this data.
    public void ReContructSnake() { isRecontruct = true; }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //These functions are inaccessible in Outer Scripts
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //Function for Updating the Collider , Renderer , And Other Data
    //Must be called in this Script whenever any change in configuration of Snake Occurs
    private void UpdateSnakeDynamics()
    {
        UpdateSnakeRenderer();
        UpdateRuntimeData();
        UpdateColliderMovementData();
    }
    //Function responsible for Growing the length of the snake by some given vertices smoothly by time
    private void GrowLengthSmooth()
    {
        if (Time.time - lengthGrowTiming > eachVertexGrowTime)
        {
            GrowLengthRaw(1, LengthGrowDirection);
            lengthGrowTiming = Time.time;
        }
        numGrownSmoothVertices++;
        if (numGrownSmoothVertices >= growLengthSmoothVertices) { isGrowLength = false; }
    }
    //Function responsible for Turning  a particular SnakeOrgan defined as the Global Varialbe by some given angle smoothly by time
    private void SmoothTurn()
    {
        float _01t = (Time.time - timing) / SnakeData.smoothTurnTime;
        float current_angle_rotated = Mathf.Lerp(0, smoothTurnAngle, Mathf.Clamp01(_01t));
        float deltaAngle = current_angle_rotated - previous_angle_rotated;
        previous_angle_rotated = current_angle_rotated;
        RotateOrganRaw(deltaAngle, smoothTurnOrgan);
        if (_01t >= 1)
        { isSmoothTurn = false; }

    }
    //Function responsible for debugging the Snake vertices and Colliders
    private void DebugDraw()
    {
        for (int i = SnakeData.headPivotIndex; i < runtime_positions.length - 1; i++)
        {
            Debug.DrawLine(transform.TransformPoint(runtime_positions.GetValue(i)), transform.TransformPoint(runtime_positions.GetValue(i + 1)), Color.red);
        }
        for (int i = SnakeData.headPivotIndex - 1; i > SnakeData.tailPivotIndex; i--)
        {
            Debug.DrawLine(transform.TransformPoint(runtime_positions.GetValue(i)), transform.TransformPoint(runtime_positions.GetValue(i - 1)), Color.white);
        }
        for (int i = SnakeData.tailPivotIndex; i > 1; i--)
        {
            Debug.DrawLine(transform.TransformPoint(runtime_positions.GetValue(i)), transform.TransformPoint(runtime_positions.GetValue(i - 1)), Color.red);
        }
    }
    //Function responsible for Turning the Direction of the SnakeOrgan organ  left , right or front 
    //It is more automatic  , and useful we are developing for not a anolog controls
    private void ChangeDirection(Direction direction, SnakeOrgan organ)
    {
        switch (direction)
        {
            case Direction.left:
                RotateOrganClockwise(organ);
                break;
            case Direction.right:
                RotateOrganAntiClockwise(organ);
                break;
            case Direction.front:
                switch (organ)
                {
                    case SnakeOrgan.Head:
                        if (currentToggleHeadDirection == Direction.left)           //we Have to Rotate the Head ClockWise
                            RotateOrganAntiClockwise(SnakeOrgan.Head);
                        else if (currentToggleHeadDirection == Direction.right)
                            RotateOrganClockwise(SnakeOrgan.Head);
                        break;
                    case SnakeOrgan.Tail:
                        if (currentToggleTailDirection == Direction.left)           //we Have to Rotate the Head ClockWise
                            RotateOrganAntiClockwise(SnakeOrgan.Tail);
                        else if (currentToggleTailDirection == Direction.right)
                            RotateOrganClockwise(SnakeOrgan.Tail);
                        break;
                }
                break;
        }

        if (direction == Direction.left || direction == Direction.right)
        {
            float angle = 0;
            switch (organ)
            {
                case SnakeOrgan.Head:
                    angle = Vector2.Angle(initialHeadBodyDirection, SnakeMovingData.LocalHeadDirection);
                    if (angle >= 90.0f)
                    {
                        currentToggleHeadDirection = direction;
                        float deltaAngle = angle - 90.0f;
                        switch (direction)
                        {
                            case Direction.left:
                                RotateOrganRaw(-deltaAngle, SnakeOrgan.Head);
                                break;
                            case Direction.right:
                                RotateOrganRaw(deltaAngle, SnakeOrgan.Head);
                                break;
                        }
                    }
                    break;
                case SnakeOrgan.Tail:
                    angle = Vector2.Angle(initialTailBodyDirection, SnakeMovingData.LocalTailDirection);
                    if (angle >= 90.0f)
                        currentToggleTailDirection = direction;
                    break;
            }
        }
        if (direction == Direction.front)
        {
            Vector3 crp;
            switch (organ)
            {
                case SnakeOrgan.Head:
                    crp = Vector3.Cross(SnakeMovingData.LocalHeadBodyDirection, SnakeMovingData.LocalHeadDirection);
                    switch (currentToggleHeadDirection)
                    {
                        case Direction.left:
                            if (crp.z <= 0) currentToggleHeadDirection = direction;
                            break;
                        case Direction.right:
                            if (crp.z >= 0) currentToggleHeadDirection = direction;
                            break;
                    }
                    break;
                case SnakeOrgan.Tail:
                    crp = Vector3.Cross(SnakeMovingData.LocalTailBodyDirection, SnakeMovingData.LocalTailDirection);
                    switch (currentToggleTailDirection)
                    {
                        case Direction.right:
                            if (crp.z <= 0) currentToggleTailDirection = direction;
                            break;
                        case Direction.left:
                            if (crp.z >= 0) currentToggleTailDirection = direction;
                            break;
                    }
                    break;
            }
        }
    }
    //Function responsible for Handling the Use Inputs for Moving the Snake but it works when 
    //The public variable "isControlledByUser" is set to true
    private void HandleUserInputMovement()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.RightControl))
                RotateOrganAntiClockwise(SnakeOrgan.Tail);
            else
                RotateOrganAntiClockwise(SnakeOrgan.Head);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (Input.GetKey(KeyCode.RightControl))
                RotateOrganClockwise(SnakeOrgan.Tail);
            else
                RotateOrganClockwise(SnakeOrgan.Head);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            InitailizeMovementData();
        if (Input.GetKey(KeyCode.UpArrow))
            MoveForward();
        if (Input.GetKey(KeyCode.DownArrow))
            MoveBackward();
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            DeInitializeMovementData();

        if (Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.E))
                ToggleDirection(Direction.right, SnakeOrgan.Tail);
            if (Input.GetKeyDown(KeyCode.Q))
                ToggleDirection(Direction.left, SnakeOrgan.Tail);
            if (Input.GetKeyDown(KeyCode.W))
                ToggleDirection(Direction.front, SnakeOrgan.Tail);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
                ToggleDirection(Direction.right, SnakeOrgan.Head);
            if (Input.GetKeyDown(KeyCode.Q))
                ToggleDirection(Direction.left, SnakeOrgan.Head);
            if (Input.GetKeyDown(KeyCode.W))
                ToggleDirection(Direction.front, SnakeOrgan.Head);
        }
        if (Input.GetKeyDown(KeyCode.P))
            ToggleSmoothTurn(-45, SnakeOrgan.Head);
        if (Input.GetKeyDown(KeyCode.O))
            ToggleSmoothTurn(45, SnakeOrgan.Head);

        if (Input.GetKeyDown(KeyCode.U) && Input.GetKey(KeyCode.RightControl))
            ToggleSmoothGrowLength(10, Direction.back);
        else if (Input.GetKeyDown(KeyCode.U))
            ToggleSmoothGrowLength(10, Direction.front);

    }
    //Function responsible for Moving the Line Renderer one vertex forward by adding the vertex on the head and removing from tail
    private void MoveOnePointForward()
    {
        Vector3 newPoint = SnakeMovingData.LocalHeadDirection.normalized * offset +
            runtime_positions.GetValue(runtime_positions.length - 1);
        runtime_positions.enqueTop(newPoint);
        UpdateSnakeDynamics();
    }
    //Function responsible for Moving the Line Renderer one vertex backward by adding the vertex on the tail and removign form head
    private void MoveOnePointBackward()
    {
        Vector3 newPoint = SnakeMovingData.LocalTailDirection.normalized * offset
            + runtime_positions.GetValue(0);
        runtime_positions.enqueBottom(newPoint);
        UpdateSnakeDynamics();
    }
    //Function responsible for Updating the Snake Runtime Data for Accessign the Runtime Information to the oursize Scripts
    private void UpdateRuntimeData()
    {
        SnakeMovingData.SetLocalHeadDirection(runtime_positions.GetValue(runtime_positions.length - 1) - runtime_positions.GetValue(runtime_positions.length - 2));
        SnakeMovingData.SetLocalTailDirection(runtime_positions.GetValue(0) - runtime_positions.GetValue(1));
        SnakeMovingData.SetLocalHeadPivotBody(runtime_positions.GetValue(SnakeData.headPivotIndex) - runtime_positions.GetValue(SnakeData.headPivotIndex - 1));
        SnakeMovingData.SetLocalTailPivotBody(runtime_positions.GetValue(SnakeData.tailPivotIndex) - runtime_positions.GetValue(SnakeData.tailPivotIndex + 1));
        //if sign == -1  , that means we have rotated clockwise w.r.t local transform
        Vector3 headTemp = Vector3.Cross(SnakeMovingData.LocalHeadBodyDirection, SnakeMovingData.LocalHeadDirection);
        Vector3 tailTemp = Vector3.Cross(SnakeMovingData.LocalTailBodyDirection, SnakeMovingData.LocalTailDirection);
        float signOfAngle_head = Mathf.Sign(Vector3.Dot(headTemp, Vector3.forward));
        float signOfAngle_tail = Mathf.Sign(Vector3.Dot(tailTemp, Vector3.forward));
        SnakeMovingData.SetTailRotatedAngle(Vector2.Angle(SnakeMovingData.LocalTailBodyDirection, SnakeMovingData.LocalTailDirection) * signOfAngle_tail);
        SnakeMovingData.SetHeadRotatedAngle(Vector2.Angle(SnakeMovingData.LocalHeadBodyDirection, SnakeMovingData.LocalHeadDirection) * signOfAngle_head);
        if (currentToggleHeadDirection != Direction.front && Mathf.Abs(headTemp.z) == 0)
        {
            currentToggleHeadDirection = Direction.front;
            HeadDirToToggle = currentToggleHeadDirection;
        }
        if (currentToggleTailDirection != Direction.front && Mathf.Abs(tailTemp.z) == 0)
        {
            currentToggleTailDirection = Direction.front;
            TailDirToToggle = currentToggleTailDirection;
        }

    }
    //Function responsible for Turning a particular organ of the snake such as Head , or Tail
    private void TurnSnakeOrgan(float angle, SnakeOrgan organ = SnakeOrgan.Head)
    {
        int pivotIndex;
        switch (organ)
        {
            case SnakeOrgan.Head:
                pivotIndex = SnakeData.headPivotIndex;
                int length = runtime_positions.ToArray().Length;
                for (int j = pivotIndex; j < length; j++)
                    Turn(j, angle, isBackward: false);
                break;
            case SnakeOrgan.Tail:
                pivotIndex = SnakeData.tailPivotIndex;
                for (int j = pivotIndex; j > 0; j--)
                    Turn(j, angle, isBackward: true);
                break;
            default:
                pivotIndex = runtime_positions.length / 2;
                Debug.LogError("Unvalid Snake Organ is Passed");
                break;
        }
        UpdateSnakeDynamics();
    }
    //Function to turn a par t of the snake by taking a pivotIndex 
    private void Turn(int pivotIndex, float _angle, bool isBackward = false)
    {
        Point pivot = new Point(runtime_positions.GetValue(pivotIndex));
        if (!isBackward)
        {
            int length = runtime_positions.length;
            for (int i = pivotIndex + 1; i < length; i++)
            {
                Point newPoint = Point.RotatePoint(new Point(runtime_positions.GetValue(i)), pivot, _angle);
                runtime_positions.SetValue(i, newPoint.ToVector());
            }
        }
        else
        {
            for (int i = pivotIndex - 1; i >= 0; i--)
            {
                Point newPoint = Point.RotatePoint(new Point(runtime_positions.GetValue(i)), pivot, _angle);
                runtime_positions.SetValue(i, newPoint.ToVector());
            }
        }
    }
    //Function responsible for Setting the Updated Queue positions to the line renderer
    private void UpdateSnakeRenderer()
    {
        int temp = renderer.numPositions = runtime_positions.length;
        for (int i = 0; i < temp; i++)
            renderer.SetPosition(i, runtime_positions.GetValue(i));
    }
    //Function responsible for Creating the Snake Data and pose based on initial Snake Data
    private void ContructSnake()
    {
        offset = SnakeData.length / SnakeData.numberOfTurningPoints;
        renderer.numPositions = SnakeData.numberOfTurningPoints;
        for (int i = 0; i < SnakeData.numberOfTurningPoints; i++)
        {
            runtime_positions.SetValue(i, new Vector2(0, -offset * SnakeData.numberOfTurningPoints / 2 + offset * i));
        }
        if (head_collider != null)
            Destroy(head_collider.gameObject);
        GameObject obj = new GameObject("Head Collider");
        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = runtime_positions.GetValue(runtime_positions.length - 1);
        head_collider = obj.AddComponent<CircleCollider2D>();
        head_collider.radius = SnakeData.headColRadius;
        isRecontruct = false;
    }

    //Functions responsible for updating and creating the collider data
    private void UpdateColliderMovementData()
    {
        int i = 0;
        for (; i < length - 1; i++)
        {
            Point p1 = new Point(static_positions[i]);
            Point p2 = new Point(static_positions[i + 1]);
            Point p3 = p2.Subtract(p1);
            Vector2 offsetVector = Point.RotatePoint(p3, new Point(0, 0, 0), -90).ToVector().normalized * Mathf.Lerp(0, SnakeData.colliderWidth, (float)(i + 1) / length);
            col_vertices[i] = offsetVector + (Vector2)p1.ToVector();
            col_vertices[2 * length - 1 - i] = (Vector2)p1.ToVector() - offsetVector;
        }
        Vector2 offset = (col_vertices[length - 2] - static_positions[length - 2]).normalized * SnakeData.colliderWidth;
        Vector2 radius_vector = SnakeMovingData.LocalHeadDirection + static_positions[length - 2];
        col_vertices[length - 1] = offset + radius_vector;
        col_vertices[length] = -offset + radius_vector;
        Collider.points = col_vertices;
        head_collider.transform.localPosition = runtime_positions.GetValue(runtime_positions.length - 1);
    }
    private void UpdateColliderData()
    {
        static_positions = runtime_positions.ToArray();
        length = static_positions.Length;
        col_vertices = new Vector2[length * 2];
    }
}
