using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid : MonoBehaviour
{
    [SerializeField]
    private int xSize = 4;
    [SerializeField]
    private int ySize = 4;
    [SerializeField]
    private bool ShowGrid = false;
    [SerializeField]
    private int pixelsPerUnit = 200;
    [SerializeField]
    private int YOffsetPixel = 5;
    [SerializeField]
    private int XOffsetPixel = 5; 
    [SerializeField]
    private float zOffset = -1.0f;
    [SerializeField]
    private Color StartColor = Color.green;
    [SerializeField]
    private Color EndColor = Color.blue;
    [SerializeField]
    private Color GridColor = Color.black;
    public bool isGridConstructed { get; private set; }

    private int tempXsize;
    private int tempYsize;
    private float cameraSize;
    private float tempzOffset;

    private bool isUpdate = false;
    private GameObject[,] tiles;
    private float hight;
    private float width;
    private float xOffset;
    private float yOffset;
    private new Camera camera;
    private Vector3[] corners;

    Vector3[] leftHeightPoints;
    Vector3[] rightHeightPoints;
    Vector3[] bottomWidthPoints;
    Vector3[] topWidthPoints;
    void Awake() { isGridConstructed = false; }
    void Start()
    {
        corners = new Vector3[4];
        camera = GetComponent<Camera>();
    }
    void Update()
    {

        if (tempXsize != xSize || tempYsize != ySize || cameraSize != camera.orthographicSize
            || tempzOffset != zOffset) { isUpdate = false; Debug.Log("Updating"); isGridConstructed = false; }
        if (isUpdate && ShowGrid)
        {
            UpdateCornerPoints(xSize, ySize);
            DrawGrid(xSize, ySize);
        }
        if (!isUpdate)
        {
            ClearTiles();
            tempXsize = xSize;
            tempYsize = ySize;
            cameraSize = camera.orthographicSize;
            tempzOffset = zOffset;
            isUpdate = true;
            RecalculateData();
            ReCreateTiles();
            isGridConstructed = true;
        }
    }
    public void DrawGrid(int x_size, int y_size)
    {
        for (int i = 0; i < y_size - 1; i++)
        {
            Debug.DrawLine(leftHeightPoints[i], rightHeightPoints[i], GridColor);
        }
        for (int i = 0; i < x_size - 1; i++)
        {
            Debug.DrawLine(bottomWidthPoints[i], topWidthPoints[i], GridColor);
        }
    }
    private void UpdateCornerPoints(int x_size, int y_size)
    {
        leftHeightPoints = new Vector3[y_size - 1];
        rightHeightPoints = new Vector3[y_size - 1];
        bottomWidthPoints = new Vector3[x_size - 1];
        topWidthPoints = new Vector3[x_size - 1];

        for (int i = 0; i < y_size - 1; i++)
        {
            leftHeightPoints[i] = camera.transform.TransformPoint(new Vector3(-width * 0.5f, yOffset * (i + 1) - hight * 0.5f, corners[0].z));
            rightHeightPoints[i] = camera.transform.TransformPoint(new Vector3(width * 0.5f, yOffset * (i + 1) - hight * 0.5f, corners[0].z));
        }
        for (int i = 0; i < x_size - 1; i++)
        {
            topWidthPoints[i] = camera.transform.TransformPoint(new Vector3(xOffset * (i + 1) - width * 0.5f, hight * 0.5f, corners[0].z));
            bottomWidthPoints[i] = camera.transform.TransformPoint(new Vector3(xOffset * (i + 1) - width * 0.5f, -hight * 0.5f, corners[0].z));
        }
    }
    private void ReCreateTiles()
    {
        int X;
        int Y;
        Y = (int)(yOffset * pixelsPerUnit) + YOffsetPixel;
        X = (int)(xOffset * pixelsPerUnit) + XOffsetPixel;
        Texture2D texture = new Texture2D(X, Y);
        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                texture.SetPixel(i, j, new Color(1, 1, 1, 1));
            }
        }
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, X, Y), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        tiles = new GameObject[xSize, ySize];
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                tiles[i, j] = new GameObject(string.Format("{0},{1}", i + 1, j + 1));
                tiles[i, j].AddComponent<SpriteRenderer>().sprite = sprite;
                tiles[i, j].transform.position = camera.transform.TransformVector(new Vector3(-width * 0.5f + xOffset * 0.5f + xOffset * i, -hight * 0.5f + yOffset * 0.5f + yOffset * j, corners[0].z));
                tiles[i, j].transform.SetParent(this.transform);
                Tile tile = tiles[i, j].AddComponent<Tile>();
                tile.SetUntoggleColor(Color.Lerp(StartColor, EndColor, (float)(i + j + 2) / (xSize + ySize)));
                tile.Off(); 
            }
        }
    }
    private void ClearTiles()
    {
        for (int i = 0; i < tempXsize; i++)
        {
            for (int j = 0; j < tempYsize; j++)
                Destroy(tiles[i, j]);
        }
    }
    private void RecalculateData()
    {
        camera.CalculateFrustumCorners(camera.rect, camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, corners);

        for (int i = 0; i < 4; i++)
        {
            corners[i] = corners[i] - new Vector3(0, 0, camera.farClipPlane + zOffset);
        }

        hight = Mathf.Abs(corners[1].y - corners[0].y);
        width = Mathf.Abs(corners[1].x - corners[2].x);
        xOffset = width / xSize;
        yOffset = hight / ySize;
    }

    public Tile getRefTile(int x, int y)
    {
        if (x > xSize - 1 || y > ySize - 1) return null;
        return tiles[x, y].GetComponent<Tile>();
    }
    public Tile GetTileInRange(Vector2 point)
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if (Check(point, tiles[i, j]))
                {
                    return tiles[i, j].GetComponent<Tile>();
                }
            }
        }
        Debug.Log("<color=red>No Tile is found</color>");
        return null;
    }
    private bool Check(Vector2 point, GameObject obj)
    {
        Sprite sprite = obj.GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;

        float width = texture.width / sprite.pixelsPerUnit * obj.transform.localScale.x;
        float height = texture.height / sprite.pixelsPerUnit * obj.transform.localScale.y;

        Vector2 point1 = new Vector2(-width * 0.5f, -height * 0.5f) + (Vector2)obj.transform.position;
        Vector2 point2 = new Vector2(-width * 0.5f, height * 0.5f) + (Vector2)obj.transform.position;
        Vector2 point3 = new Vector2(width * 0.5f, height * 0.5f) + (Vector2)obj.transform.position;
        Vector2 point4 = new Vector2(width * 0.5f, -height * 0.5f) + (Vector2)obj.transform.position;


        if (point.x > point1.x && point.x < point3.x && point.y < point2.y && point.y > point4.y)
            return true;
        else return false; 
    } 

    private void DrawDebugBorder(GameObject obj)
    {
        Sprite sprite = obj.GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;

        float width = texture.width / sprite.pixelsPerUnit * obj.transform.localScale.x;
        float height = texture.height / sprite.pixelsPerUnit * obj.transform.localScale.y;

        Vector2 point1 = new Vector2(-width * 0.5f, -height * 0.5f) + (Vector2)obj.transform.position;
        Vector2 point2 = new Vector2(-width * 0.5f, height * 0.5f) + (Vector2)obj.transform.position;
        Vector2 point3 = new Vector2(width * 0.5f, height * 0.5f) + (Vector2)obj.transform.position;
        Vector2 point4 = new Vector2(width * 0.5f, -height * 0.5f) + (Vector2)obj.transform.position;

        Debug.DrawLine(point1, point2, Color.yellow);
        Debug.DrawLine(point2, point3, Color.yellow);
        Debug.DrawLine(point3, point4, Color.yellow);
        Debug.DrawLine(point4, point1, Color.yellow);
    }
}
