using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnerScript : MonoBehaviour
{
    public GameObject enemyPrefab;  // Prefab of the enemy car
    public Transform player;        // Reference to the player's transform
    public float baseSpawnInterval = 4f; // Base time interval between spawns for 0 stars
    public float spawnDistance = 10f; // Distance from the screen to spawn
    public float monsterModeIntervalMultiplier = 0.5f;
    public int maxStars = 5; // Maximum number of stars
    public float currentSpawnInterval;
    public List<Image> starImages = new List<Image>();

    [Header("Map boundaries")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private Camera mainCamera;
    private PlayerMovementScript pm;
    private float spawnTimer;
    private int starCount;
    private bool monsterModePreviouslyActive;

    void Start()
    {
        mainCamera = Camera.main;
        pm = player.gameObject.GetComponent<PlayerMovementScript>();
        spawnTimer = 0f;
        starCount = 0; // Initialize starCount
        monsterModePreviouslyActive = false; // Initialize the monster mode state tracker
    }

    void Update()
    {
        // Update the spawn timer based on the elapsed time
        spawnTimer += Time.deltaTime;

        // Determine the current spawn interval based on starCount
        currentSpawnInterval = GetSpawnInterval();

        // Check if the timer has reached the spawn interval
        if (spawnTimer >= currentSpawnInterval)
        {
            // Spawn an enemy and reset the timer
            SpawnEnemy();
            spawnTimer = 0f;
        }

        // Check if monster mode was just activated
        if (pm.isMonsterMode && !monsterModePreviouslyActive && starCount < maxStars)
        {
            starCount++; // Increase starCount only once when monster mode is activated
            starImages[-1 + starCount].gameObject.SetActive(true);
            monsterModePreviouslyActive = true; // Update the state tracker
        }
        else if (!pm.isMonsterMode)
        {
            monsterModePreviouslyActive = false; // Reset the state tracker when monster mode ends
        }
    }

    float GetSpawnInterval()
    {
        // Determine spawn interval based on starCount
        switch (starCount)
        {
            case 0:
                return baseSpawnInterval;
            case 1:
                return 2f;
            case 2:
                return 1f;
            case 3:
                return 0.5f;
            case 4:
                return 0.25f;
            case 5:
                return 0.1f;
            default:
                return baseSpawnInterval; // Default case if something goes wrong
        }
    }

    void SpawnEnemy()
    {
        // Calculate screen boundaries
        Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.transform.position.z));

        // Randomize the side to spawn from
        int side = Random.Range(0, 4);
        Vector3 spawnPosition = Vector3.zero;

        switch (side)
        {
            case 0: // Top
                spawnPosition = new Vector3(Random.Range(screenBottomLeft.x, screenTopRight.x), screenTopRight.y + spawnDistance, 0);
                break;
            case 1: // Bottom
                spawnPosition = new Vector3(Random.Range(screenBottomLeft.x, screenTopRight.x), screenBottomLeft.y - spawnDistance, 0);
                break;
            case 2: // Left
                spawnPosition = new Vector3(screenBottomLeft.x - spawnDistance, Random.Range(screenBottomLeft.y, screenTopRight.y), 0);
                break;
            case 3: // Right
                spawnPosition = new Vector3(screenTopRight.x + spawnDistance, Random.Range(screenBottomLeft.y, screenTopRight.y), 0);
                break;
        }

        if (minX < spawnPosition.x && spawnPosition.x < maxX && minY < spawnPosition.y && spawnPosition.y < maxY)
        {

            // Instantiate the enemy car at the spawn position
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            EnemyMovementScript enemyMovement = enemy.GetComponent<EnemyMovementScript>();
            enemyMovement.player = player; // Assign the player reference to the enemy
        }

    }
}