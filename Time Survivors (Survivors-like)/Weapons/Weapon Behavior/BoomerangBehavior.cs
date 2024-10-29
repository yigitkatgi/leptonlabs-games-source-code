using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoomerangBehavior : ProjectileWeaponBehavior
{
    public float aimRadius;
    public float returnTime = 0.5f;
    public float selfRotationSpeed = 360f; // Speed at which the boomerang rotates around itself

    private float returnSpeed;
    private PlayerStats player;
    private bool isReturning = false;
    private Vector3 travelDirection;
    private Vector3 initialScale;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        returnSpeed = weaponData.Speed;
        player = FindObjectOfType<PlayerStats>();

        if (player == null)
        {
            Debug.LogError("PlayerStats not found!");
        }

        // Save the initial scale
        initialScale = transform.localScale;

        // Determine the initial travel direction towards the nearest enemy
        Transform nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            travelDirection = GetAutoAimDirection(nearestEnemy);
        }
        else
        {
            travelDirection = player.transform.right; // Default direction if no enemy is found
        }

        StartCoroutine(WaitForReturn());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReturning)
        {
            // Move in the initial travel direction
            transform.position += travelDirection * currentSpeed * Time.deltaTime;
        }
        else
        {
            ReturnToPlayer();
        }

        // Ensure the scale remains consistent
        transform.localScale = initialScale;

        // Rotate the boomerang around itself
        SelfRotate();
    }

    void SelfRotate()
    {
        transform.Rotate(Vector3.forward, selfRotationSpeed * Time.deltaTime);
    }

    Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] props = GameObject.FindGameObjectsWithTag("Prop");

        GameObject[] combined = enemies.Concat(props).ToArray();

        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in combined)
        {
            float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= aimRadius)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    Vector3 GetAutoAimDirection(Transform nearestEnemy)
    {
        Vector3 direction = (nearestEnemy.position - transform.position).normalized;
        return direction;
    }

    void ReturnToPlayer()
    {
        // Move the boomerang towards the player
        Vector3 direction = (player.transform.position - transform.position).normalized;
        float step = returnSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);

        // Check if the boomerang has reached the player
        if (Vector3.Distance(transform.position, player.transform.position) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator WaitForReturn()
    {
        yield return new WaitForSeconds(returnTime);
        isReturning = true; // Start returning to the player
    }
}
