using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{
    public float strenght;                          //Strenght Range [ 0 , 1]
    public Vector2 point;

    public Damage(float strenght, Vector2 point)
    {
        this.strenght = strenght;
        this.point = point;
    }
    public Damage() : this(0, Vector2.zero) { }
}

[RequireComponent(typeof(Snake))]
public class SnakeDamageController : MonoBehaviour
{
    private Snake _snake_controller;
    private int num_vertices;
    private LineRenderer _snake_renderer;
    private Vector2[] vertices;
    private float total_snake_length;

    private ColorBand band;
    private Color MAX_DAMAGE_COLOR;
    private Color MIN_DAMAGE_COLOR;
    private Gradient DAMAGE_MAP;
    private List<Damage> damages;
    GradientAlphaKey[] a_keys;
    GradientColorKey[] c_keys;
    Gradient damage_map;

    private void Awake()
    {
        _snake_controller = GetComponent<Snake>();
        _snake_renderer = GetComponent<LineRenderer>();
        MIN_DAMAGE_COLOR = Color.black;
        MAX_DAMAGE_COLOR = Color.red;
        damages = new List<Damage>();
        damage_map = new Gradient();

        a_keys = new GradientAlphaKey[2];
        a_keys[0].alpha = 1;
        a_keys[1].alpha = 1;
        a_keys[0].time = 0;
        a_keys[1].time = 1;
    }
    private void OnCollisionEnter2D(Collision2D colInfo)
    {
        num_vertices = _snake_controller.runtime_positions.length;
        vertices = _snake_controller.runtime_positions.ToArray();
        total_snake_length = Mathf.RoundToInt((num_vertices - 1) * _snake_controller.offset);


        TakeDamage(new Damage(1.0f, colInfo.contacts[0].point));
    }
    private void TakeDamage(Damage damage)
    {
        DrawDamage(damage);
    }
    private void DrawDamage(Damage damage)
    {
        damages.Add(damage);
        int NUM_KEYS = 2 + damages.Count * 3;
        c_keys = new GradientColorKey[NUM_KEYS];      //3 - keys for each damage
        Damage[] _static_array = damages.ToArray(); 
        for (int i = 1; i <= damages.Count; i++)
        {
            int main_damage_key_index = i * 3 - 1;
            float normalized_coordinate = GetNormalizedCoordinate(_static_array[i - 1].point);
            float strenght_01 = _static_array[i -1].strenght;
            c_keys[main_damage_key_index - 1].color = MIN_DAMAGE_COLOR;
            c_keys[main_damage_key_index].color = GetDamageColor(strenght_01);
            c_keys[main_damage_key_index + 1].color = MIN_DAMAGE_COLOR;
            c_keys[main_damage_key_index - 1].time = normalized_coordinate - 0.1f;
            c_keys[main_damage_key_index].time = normalized_coordinate;
            c_keys[main_damage_key_index + 1].time = normalized_coordinate + 0.1f;
        }

        c_keys[0].color = MIN_DAMAGE_COLOR;
        c_keys[0].time = 0;
        c_keys[NUM_KEYS - 1].time = 1;
        c_keys[NUM_KEYS - 1].color = MIN_DAMAGE_COLOR;

        damage_map.SetKeys(c_keys, a_keys);
        _snake_renderer.colorGradient = damage_map;
    }
    private Color GetDamageColor(float strength_01)
    {
        return Color.Lerp(MIN_DAMAGE_COLOR, MAX_DAMAGE_COLOR, strength_01);
    }
    private float GetNormalizedCoordinate(Vector2 hit_point)
    {
        int closest_index = GetClosestPointIndexTo(hit_point);
        float upper_length = (num_vertices - closest_index - 1) * _snake_controller.offset;
        float lower_length = closest_index * _snake_controller.offset;
        return 1 - upper_length / total_snake_length;
    }
    private int GetClosestPointIndexTo(Vector2 hit_point)
    {
        float[] sqr_distances = new float[num_vertices];
        for (int i = 0; i < num_vertices; i++)
            sqr_distances[i] = (hit_point - (Vector2)transform.TransformPoint(vertices[i])).sqrMagnitude;

        float _distance_0 = sqr_distances[0];
        int _index_0 = 0;
        for (int i = 0; i < num_vertices; i++)
            if (_distance_0 > sqr_distances[i])
            {
                _distance_0 = sqr_distances[i];
                _index_0 = i;
            }
        return _index_0;
    }
}

