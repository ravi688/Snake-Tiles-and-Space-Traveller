using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTestScript : MonoBehaviour {


    void OnGUI()
    {
        GUIContent content = new GUIContent("World Pos = " + transform.position.ToString() + 
            "\n" + "Local Pos = " + transform.localPosition.ToString()
             , "The position of the child");  
        GUI.Label(new Rect(10, 50, 200, 100), content); 
 
       
    }

    private void Update()
    {

    }
}
