using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamageGradient : MonoBehaviour
{
    private Vector3[] points;
    private LineRenderer _line_renderer;
    private int num_vertices;
    private float offset = 1.0f;
    private Color MIN_COLOR = Color.black;
    private Color MAX_COLOR = Color.red;
    private float total_length; 
    Vector2 temp_vector; 
    private void Awake()
    {
        _line_renderer = GetComponent<LineRenderer>();
        num_vertices = _line_renderer.numPositions;
        points = new Vector3[num_vertices];
        _line_renderer.GetPositions(points);
        total_length = 1 * num_vertices; 
    }
    private void OnCollisionEnter2D(Collision2D col_info)
    {
        TakeDamage(col_info.contacts[0].point, 1);
    }
    private void TakeDamage(Vector2 hit_point, float strength_01)
    {
        Gradient grad = new Gradient();
        GradientColorKey[] damage_marks = new GradientColorKey[5];
        damage_marks[2].color = GetDamageColor(strength_01);
        damage_marks[2].time = GetNormalizedLocation(hit_point);
        damage_marks[0].color = MIN_COLOR;
        damage_marks[0].time = 0;
        damage_marks[1].time = damage_marks[2].time - 0.1f;
        damage_marks[1].color = MIN_COLOR;
        damage_marks[3].time = damage_marks[2].time + 0.1f;
        damage_marks[3].color = MIN_COLOR;
        damage_marks[4].color = MIN_COLOR;
        damage_marks[4].time = 1;
        GradientAlphaKey[] akeys = new GradientAlphaKey[2];
        akeys[0].time = 0;
        akeys[0].alpha = 1;
        akeys[1].time = 1;
        akeys[1].alpha = 1;
        grad.SetKeys(damage_marks, akeys);
        _line_renderer.colorGradient = grad;

    }
    private float GetNormalizedLocation(Vector2 hit_point)
    {
        int index;
        GetClosestPoint(hit_point, out index , out temp_vector);
        float upper_length = (num_vertices - index - 1) * offset;
        float lower_length = index * offset;
        return lower_length / total_length; 
    }
    private Color GetDamageColor(float interpolant)
    {
        return Color.Lerp(MIN_COLOR, MAX_COLOR, interpolant);
    }

    private void GetClosestPoint(Vector2 point, out int index , out Vector2 out_point)
    {
        index = 0;
        float[] distances = new float[num_vertices];
        for (int i = 0; i < num_vertices; i++) 
            distances[i]  = ((Vector2)transform.TransformPoint(points[i]) - point).sqrMagnitude;
        float _closest_distance = distances[0];
        for(int i =0;i  < num_vertices; i++)
        {
            if (_closest_distance > distances[i])
            {
                _closest_distance = distances[i];
                index = i; 
            }  
        } 
        out_point = points[index]; 
    }




}
