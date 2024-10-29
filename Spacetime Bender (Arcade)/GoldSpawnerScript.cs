using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldSpawnerScript : MonoBehaviour
{
    public GameObject prefab1;
    public GameObject prefab2;
    public float weight1 = 0.7f; // Probability weight for prefab1
    public float weight2 = 0.3f; // Probability weight for prefab2
    public float spawnCooldown = 2f; // Time interval between spawns
    public float initialCooldown = 0.5f;
    public float goldSpeed = 7f;
    
    private PlayerCollisionScript playerCol;
    private Camera mainCamera;
    private float timer;
    private float initialTimer;

    void Start()
    {
        mainCamera = Camera.main;
        playerCol = FindObjectOfType<PlayerCollisionScript>();
        timer = spawnCooldown;
    }

    void Update()
    {
        initialTimer += Time.deltaTime;
        if (initialTimer < initialCooldown) return;

        timer += Time.deltaTime;
        if (timer >= spawnCooldown)
        {
            SpawnPrefab();
            timer = 0f;
        }
    }

    void SpawnPrefab()
    {
        // Ensure only one gold at a time
        //if (playerCol.golds.Count > 0) return;
        
        // Select prefab based on weighted probability
        GameObject selectedPrefab = SelectPrefabBasedOnWeight();

        // Calculate a random position within the camera's view
        Vector3 randomPosition = GetRandomPositionInView();

        // Instantiate the selected prefab at the random position
        GameObject instantiatedPrefab = Instantiate(selectedPrefab, randomPosition, Quaternion.identity);

        // Parent the instantiated prefab to the camera
        //instantiatedPrefab.transform.SetParent(mainCamera.transform);
        Rigidbody2D rb = instantiatedPrefab.GetComponent<Rigidbody2D>();

        rb.velocity = Vector3.down * goldSpeed;

        playerCol.AddToGoldList(instantiatedPrefab);

    }

    GameObject SelectPrefabBasedOnWeight()
    {
        float totalWeight = weight1 + weight2;
        float randomValue = Random.Range(0f, totalWeight);

        if (randomValue < weight1)
        {
            return prefab1;
        }
        else
        {
            return prefab2;
        }
    }

    Vector3 GetRandomPositionInView()
    {
        float y = 6f;
        float minX = -1.5f;
        float maxX = 1.5f;

        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), y, 0);

        return randomPosition;
    }
}
