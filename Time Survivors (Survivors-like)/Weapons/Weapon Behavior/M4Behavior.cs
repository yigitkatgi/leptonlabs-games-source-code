using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class M4Behavior : ProjectileWeaponBehavior
{
    PlayerStats player;
    public float aimRadius;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        player = FindObjectOfType<PlayerStats>();
        if (player == null)
        {
            Debug.LogError("PlayerStats not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Transform nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector3 autoAimDirection = GetAutoAimDirection(nearestEnemy);
            transform.position += autoAimDirection * currentSpeed * Time.deltaTime;
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
        Vector3 direction = (nearestEnemy.position - player.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        return direction;
    }
}
