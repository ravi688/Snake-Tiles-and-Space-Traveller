using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript3 : MonoBehaviour {

    public GameObject prefab;
    [SerializeField]
    private float width = 1.0f;
    private Vector2[] positions;

    int num_childs; 
    private void Awake()
    {
         num_childs = transform.childCount;
        positions = new Vector2[num_childs];
        for (int i = 0; i < num_childs; i++)
        {
            positions[i] = transform.GetChild(i).position;
        }
    }
    private void Start()
    {
        for (int i = 0; i < num_childs - 1; i++)
        {
            Point p1 = new Point(positions[i + 1]); 
            Point p2 = new Point(positions[i]); 
            Point p3 = Point.RotatePoint(p1 , p2 , -90) ; 
            Instantiate(prefab , p3.ToVector(), Quaternion.identity); 
        }
    }

}
