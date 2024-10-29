using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    Vector3 noTerrainPosition;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    PlayerMovementScript pms;

    // For optimization (removing chunks at a distance from the player
    public List<GameObject> spawnedChunks;
    GameObject latestChunk;
    public float maxOpDist; // Must be greater than the width and length of the tilemap
    float opDist;
    float optimizerCooldown;
    public float optimizerCooldownDuration;



    // Start is called before the first frame update
    void Start()
    {
        pms = FindObjectOfType<PlayerMovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }

        if (pms.moveDirection.x > 0 && pms.moveDirection.y == 0) // Right
        {
            if(!Physics2D.OverlapCircle(currentChunk.transform.Find("Right").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Right").position;
                ChunkSpawn();
            }
        }
        else if (pms.moveDirection.x < 0 && pms.moveDirection.y == 0) // Left
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Left").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Left").position;
                ChunkSpawn();
            }
        }
        else if (pms.moveDirection.x == 0 && pms.moveDirection.y > 0) // Up
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Up").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Up").position;
                ChunkSpawn();
            }
        }
        else if (pms.moveDirection.x == 0 && pms.moveDirection.y < 0) // Down
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Down").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Down").position;
                ChunkSpawn();
            }
        }
        else if (pms.moveDirection.x > 0 && pms.moveDirection.y > 0) // Right Up
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Right Up").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Right Up").position;
                ChunkSpawn();
            }
        }
        else if (pms.moveDirection.x > 0 && pms.moveDirection.y < 0) // Right Down
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Right Down").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Right Down").position;
                ChunkSpawn();
            }
        }
        else if (pms.moveDirection.x < 0 && pms.moveDirection.y > 0) // Left Up
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Left Up").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Left Up").position;
                ChunkSpawn();
            }
        }
        else if (pms.moveDirection.x < 0 && pms.moveDirection.y < 0) // Left Down
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Left Down").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Left Down").position;
                ChunkSpawn();
            }
        }
    }

    void ChunkSpawn()
    {
        int rand = Random.Range(0,terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], noTerrainPosition, Quaternion.identity);
        spawnedChunks.Add(latestChunk);
    }

    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;
        if (optimizerCooldown <= 0f)
        {
            optimizerCooldown = optimizerCooldownDuration;
        }
        else
        {
            return;
        }
        foreach (GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);  
            }
        }
    }
}
