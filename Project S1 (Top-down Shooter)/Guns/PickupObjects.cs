using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObjects : MonoBehaviour
{
    public bool hasCollided;
    public Vector3 collisionPoint;

    private void Start()
    {
        hasCollided = false;
        collisionPoint = Vector3.zero;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            hasCollided = true;
            collisionPoint = col.contacts[0].point; // Get the first contact point of collision
        }
    }
}
