using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour {
    [SerializeField]
    private GameObject PPlanet;
    [SerializeField]
    private GameObject Star;
    [SerializeField]
    private int numStars = 10;
    [SerializeField]
    private int numPlanets = 2;


    public static Vector2 ScreenWorldCoordinates;

    void OnDrawGizmos()
    {
        Vector2 size = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        Gizmos.DrawSphere(size, 0.5f);
    }
    void Awake()
    {
        ScreenWorldCoordinates = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width , Screen.height));
        CreateFrame(0, 0); 
    }
    int xCounter = 1;
    int yCounter = 1; 
    void Update()
    {
        if (Camera.main.transform.position.x > xCounter * ScreenWorldCoordinates.x * 0.5f)
        {
            CreateFrame(xCounter, 0);
            xCounter++;
        }
    }

    void CreateFrame(int xoffset ,int yoffset )
    {
        for (int i = 0; i < numStars; i++)
        {
            Vector2 pos = new Vector2(Random.Range(0, ScreenWorldCoordinates.x),
              Random.Range(0, ScreenWorldCoordinates.y *2)); 
            GameObject inst = Instantiate(Star, pos + (Vector2)Camera.main.transform.position 
                + new Vector2(pos.x  * xoffset , pos.y * yoffset), Quaternion.identity); 
        }
        for(int i =0; i < numPlanets; i++)
        {

            Vector2 pos = new Vector2(Random.Range(0, ScreenWorldCoordinates.x),
                Random.Range(0, ScreenWorldCoordinates.y));
            GameObject inst = Instantiate(PPlanet, pos + (Vector2)Camera.main.transform.position
                + new Vector2(pos.x * xoffset , pos.y  * yoffset), Quaternion.identity); 
        }
    }
}
