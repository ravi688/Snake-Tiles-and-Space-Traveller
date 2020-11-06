using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{

    [SerializeField]
    private float offset = 1.0f;
    [SerializeField]
    private Transform Darkner;
    [SerializeField]
    private SpriteRenderer _planet;
    [SerializeField]
    private float damping = 2.0f;

    private Vector2 comingPoint;

    void Awake()
    {
        comingPoint = Darkner.transform.localPosition = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * offset;
        _planet.color = new Color(Random.Range(0.5f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.2f, 1.0f), 1.0f);
        Darkner.transform.localScale = Darkner.transform.localScale * Random.Range(0.5f, 1.2f);
    }
    void Update()
    {
        Darkner.transform.localPosition = Vector2.Lerp(Darkner.transform.localPosition, -comingPoint, Time.deltaTime * 1 / (damping == 0 ? 1 : damping));
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.parent && col.transform.parent.name == "Rocket")
        {
            Rocket.RocketInstance.ExplodeRocket();
        }
    }

}
