using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    /// <summary>
    /// Class to store all of the data about waves of enemies
    /// </summary>
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; // A list of groups of enemies to spawn in this wave
        public int waveQuota; // Total number of enemies to be spawned in this wave
        public float spawnInterval; // The interval at which to spawn enemies
        public int spawnCount; // The number of enemies already spawned in this wave
    }

    /// <summary>
    /// Class to store all of the data about a group of enemies in a wave of enemies
    /// </summary>
    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount; // Number of enemies to spawn in this wave
        public int spawnCount; // The number of enemies of this type already spawned in this wave
        public GameObject enemyPrefab;

    }

    public List<Wave> waves; // A list of all the waves in the game 
    public int currentWaveCount; // The index of the current wave (first wave has index 0)

    [Header("Spawner Attributes")]
    float spawnTimer; // Timer used to determine when to spawn the next enemy
    public int enemiesAlive;
    public int maxEnemiesAllowed; // The maximum number if enemies allowed on the map at once
    public bool maxEnemiesReached = false; // Flag indicating if the max number of enemies has been reached
    public float waveInterval; // The interval between each wave

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints; // A list to store all the relative spawn points of enemies

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the wave has ended and the next wave should start
        if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        // Check if its time to spawn the next enemy
        if (spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    /// <summary>
    /// Coroutine that waits for waveInterval seconds before increasing the wave index (progressing onto the next wave)
    /// </summary>
    IEnumerator BeginNextWave()
    {
        // Wait for "waveInterval" seconds before starting the next wave
        yield return new WaitForSeconds(waveInterval);

        // Check if there are more waves to start after this current wave, if not, coroutine ends
        if (currentWaveCount < waves.Count - 1)
        {
            // Progress onto the next wave
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
    }

    /// <summary>
    /// This method will stop spawning enemies if the amiunt of enemies on the map is maximum.
    /// This method will only spawn enemies in a particular wave until it is time for the next wave's enemies to be spawned.
    /// </summary>
    void SpawnEnemies()
    {
        // Check if the minimum number of enemies in the wave has been spawned
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            // Spawn each type of enemy until the quota is filled
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                // Check if the minimum number of enemies of this type have been spawned
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    // Limit the number of enemies that can be spawned at once
                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }

                    // Spawn the enemy at a random position close to the player
                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0,relativeSpawnPoints.Count)].position, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;
                }
            }
        }

        // Reset the maxEnemiesReached flag if the number of enemies alive has dropped below the maximum amount
        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }

    /// <summary>
    /// Call this function when an enemy is killed
    /// </summary>
    public void OnEnemyKilled()
    {
        // Decrement the number of enemies alive
        enemiesAlive--;
    } 
}
