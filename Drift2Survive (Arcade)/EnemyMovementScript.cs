using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementScript : MonoBehaviour
{
    public float moveSpeed;
    public float turnSpeed;
    public float frictionCoefficient = 0.1f;
    public Transform player;  // Reference to the player's transform
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        MoveTowardsPlayer();
        ApplyFriction();
        UpdateDirection();
    }

    void MoveTowardsPlayer()
    {
        // Calculate direction towards the player
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        // Calculate the force to apply
        Vector2 force = directionToPlayer * moveSpeed;

        // Apply the force
        rb.AddForce(force);
    }

    void ApplyFriction()
    {
        // Apply friction force opposite to the direction of velocity
        Vector2 frictionForce = -rb.velocity * frictionCoefficient;
        rb.AddForce(frictionForce);
    }

    void UpdateDirection()
    {
        Vector2 velocity = rb.velocity;
        if (velocity != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 270f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }
}
