using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeslaCoilBehavior : ProjectileWeaponBehavior
{
    public float aimRadius;
    private HashSet<Transform> zappedEnemies = new HashSet<Transform>();
    private Vector3 initialScale;

    // Start is called before the first frame update
    protected override void Start()
    {
        initialScale = transform.localScale; // Save the initial scale
        Debug.Log("Initial Scale: " +  initialScale);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = initialScale; // Ensure the scale remains consistent

        Transform nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector3 autoAimDirection = GetAutoAimDirection(nearestEnemy);
            transform.position += autoAimDirection * currentSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, nearestEnemy.position) < 0.1f)
            {
                zappedEnemies.Add(nearestEnemy);
            }
        }
        else
        {
            Destroy(gameObject);
        }
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
            Transform enemyTransform = enemy.transform;
            float distanceToEnemy = Vector3.Distance(currentPosition, enemyTransform.position);
            if (!zappedEnemies.Contains(enemyTransform) && distanceToEnemy < shortestDistance && distanceToEnemy <= aimRadius)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemyTransform;
            }
        }

        return nearestEnemy; 
    }

    Vector3 GetAutoAimDirection(Transform nearestEnemy)
    {
        Vector3 direction = (nearestEnemy.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        return direction;
    }
}
