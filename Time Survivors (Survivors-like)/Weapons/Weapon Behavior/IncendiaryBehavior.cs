using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncendiaryBehavior : ProjectileWeaponBehavior
{
    public GameObject fireAreaPrefab;
    public float explosionRadius;
    public float fireDuration;
    public float launchAngle = 45f; // Angle at which the grenade is launched
    public float gravity = 9.81f; // Simulated gravity value
    public float launchSpeed = 10f; // Initial launch speed

    private PlayerStats player;
    private Rigidbody2D rb;
    private Vector2 initialVelocity;

    protected override void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            Debug.LogError("PlayerStats not found!");
        }
        else if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found!");
        }
        else
        {
            LaunchTowardsNearestEnemy();
            StartCoroutine(ExplodeAfterDelay(destroyAfterSeconds));
        }
    }

    void LaunchTowardsNearestEnemy()
    {
        Transform nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector2 direction = (nearestEnemy.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, nearestEnemy.position);

            float radianAngle = launchAngle * Mathf.Deg2Rad;
            float vx = launchSpeed * Mathf.Cos(radianAngle);
            float vy = launchSpeed * Mathf.Sin(radianAngle);

            initialVelocity = new Vector2(vx * direction.x, vy);

            rb.velocity = initialVelocity;
        }
    }

    void FixedUpdate()
    {
        // Apply simulated gravity
        rb.velocity += new Vector2(0, -gravity * Time.fixedDeltaTime);
    }

    Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    void Explode()
    {
        Instantiate(fireAreaPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }
}
