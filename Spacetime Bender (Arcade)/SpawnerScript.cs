using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float obstacleCooldown = 1f; // Time between obstacle spawns
    public float obstacleSpeed = 5f; // Speed at which the obstacle moves
    public float spawnPositionPlusMinus = 1f;
    private float timer;
    private RewindManager rewindManager;
    private List<GameObject> obstacles = new List<GameObject>(); // List to track obstacles

    private void Start()
    {
        rewindManager = FindObjectOfType<RewindManager>();
        SpawnObstacle(); // Initial spawn

        if (rewindManager == null)
        {
            Debug.LogError("RewindManager not found! Make sure it is added to the scene.");
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Spawn new obstacles at intervals
        if (timer > obstacleCooldown)
        {
            SpawnObstacle();
            timer = 0;
        }

        // Move each obstacle downwards
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            GameObject obstacle = obstacles[i];
            if (obstacle != null)
            {
                obstacle.transform.position += Vector3.down * obstacleSpeed * Time.deltaTime;
            }
            else
            {
                // Remove null references from the list
                obstacles.RemoveAt(i);
            }
        }
    }

    private void SpawnObstacle()
    {
        Vector3 spawnPosition = new Vector3(
            transform.position.x + Random.Range(-spawnPositionPlusMinus, spawnPositionPlusMinus),
            transform.position.y,
            transform.position.z);

        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);

        // Add the new obstacle to the list
        obstacles.Add(obstacle);

        if (rewindManager != null)
        {
            // Notify RewindManager of the new obstacle
            rewindManager.RegisterObstacle(obstacle);
        }
        else
        {
            Debug.LogWarning("RewindManager is not set. Obstacle will not be registered for rewind.");
        }
    }

    public void AssignObstaclePrefab(GameObject prefab)
    {
        obstaclePrefab = prefab;
    }
}
