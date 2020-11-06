using UnityEngine;

[System.Serializable]
public class SnakeConstructionData
{
    public int numberOfTurningPoints = 60;
    public int moveVerticesPerSecond = 50;                        //How many vertices to add per second
    public int lengthGrowVerticesPerSecond = 5;                   //No of vertices to add per second
    public int headPivotIndex = 55;
    public int tailPivotIndex = 5;
    public float length = 10;
    public float smoothTurnTime = 0.5f;
    public float headTurnAngularSpeed = 60;
    public float tailTurnAngularSpeed = 60;
    public float colliderWidth = 0.4f;
    public float headColRadius = 0.4f;
}