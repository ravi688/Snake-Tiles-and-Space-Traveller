using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Star : MonoBehaviour {

    private float phase;
    private float maxBrightness;
    private float frequencyFactor; 
    private SpriteRenderer[] rend;
    public static Vector2 ScreenWorldCoordinates; 
    private void Awake()
    {
        phase = Random.Range(0, Mathf.PI * 2);
        maxBrightness = Random.Range(0.6f, 0.9f);
        frequencyFactor = Random.Range(5.0f, 10.0f); 
        rend = GetComponentsInChildren<SpriteRenderer>(true); 
    }

    private void Update()
    {
        foreach (SpriteRenderer _rend in rend)
        {
            _rend.color = new Color(_rend.color.r, _rend.color.g, _rend.color.b, Mathf.PingPong(phase + Time.time / frequencyFactor, maxBrightness));
        }
    }
}