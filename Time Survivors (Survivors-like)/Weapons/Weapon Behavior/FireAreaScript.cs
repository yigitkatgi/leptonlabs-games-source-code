using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAreaScript : MonoBehaviour
{
    public float damagePerSecond;
    public float duration;
    private HashSet<EnemyStats> enemiesInFireArea = new HashSet<EnemyStats>();

    void Start()
    {
        Destroy(gameObject, duration);
        StartCoroutine(DamageEnemiesPeriodically());
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemiesInFireArea.Add(enemy);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemiesInFireArea.Remove(enemy);
            }
        }
    }

    private IEnumerator DamageEnemiesPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second

            // Create a copy of the HashSet to avoid modifying it while iterating
            EnemyStats[] enemiesSnapshot = new EnemyStats[enemiesInFireArea.Count];
            enemiesInFireArea.CopyTo(enemiesSnapshot);

            foreach (var enemy in enemiesSnapshot)
            {
                if (enemy != null)
                {
                    enemy.TakeDamage(damagePerSecond, transform.position, 0 ,0);
                }
            }
        }
    }
}
