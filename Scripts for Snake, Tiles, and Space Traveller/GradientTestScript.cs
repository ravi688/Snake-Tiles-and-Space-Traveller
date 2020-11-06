using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientTestScript : MonoBehaviour {

    public Gradient grad;
    public GradientAlphaKey[] alpha_keys;
    public GradientColorKey[] color_keys;

    private LineRenderer rend; 

    private void Start()
    {
        rend = GetComponent<LineRenderer>() ; 
        CreateGradient();
        rend.colorGradient = grad; 
    }

    private void CreateGradient()
    {
        alpha_keys = new GradientAlphaKey[2];
        color_keys = new GradientColorKey[2];
        alpha_keys[0].alpha = 1;
        alpha_keys[0].time = 0; 
        alpha_keys[1].alpha = 1;
        alpha_keys[1].time = 1;

        color_keys[0].color = Color.green;
        color_keys[0].time = 0; 
        color_keys[1].color = Color.red;
        color_keys[1].time = 1;
        grad.SetKeys(color_keys, alpha_keys); 
    }
}
