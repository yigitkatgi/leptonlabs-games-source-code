using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    private Vector2 direction;

    public void Initialize(Vector2 initialDirection)
    {
        direction = initialDirection.normalized;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
    }
}
