using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public GameObject astarGameObject; // Reference to the A* GameObject
    private List<EnemyAI> activeEnemies = new List<EnemyAI>(); // Track enemies that have detected the player

    private void Start()
    {
        if (astarGameObject != null)
        {
            astarGameObject.SetActive(false); // Start with A* disabled
        }
    }

    // Call this method when an enemy detects the player
    public void OnEnemyDetectsPlayer(EnemyAI enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
            if (activeEnemies.Count == 1)
            {
                EnableAstar();
            }
        }
    }

    // Call this method when an enemy is killed
    public void OnEnemyKilled(EnemyAI enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            if (activeEnemies.Count == 0)
            {
                DisableAstar();
            }
        }
    }

    private void EnableAstar()
    {
        if (astarGameObject != null)
        {
            astarGameObject.SetActive(true);
            Debug.Log("A* Enabled");
        }
    }

    private void DisableAstar()
    {
        if (astarGameObject != null)
        {
            astarGameObject.SetActive(false);
            Debug.Log("A* Disabled");
        }
    }
}
