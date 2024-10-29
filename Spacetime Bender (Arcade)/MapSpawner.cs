using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public float spawnCooldown = 2f; // Time between spawns
    public float speed = 1f; // Speed at which the background moves down
    public GameObject backgroundPrefab;
    public float lifeTime = 8f; // Time after which the background is destroyed

    private float timer;
    private List<GameObject> backgrounds = new List<GameObject>(); // List to track backgrounds
    private GameObject initialBG;

    private void Awake()
    {
        initialBG = Instantiate(backgroundPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Destroy(initialBG, lifeTime);
    }

    private void Start()
    {
        timer = spawnCooldown;
    }

    private void Update()
    {

        if (initialBG) initialBG.transform.position += Vector3.down * speed * Time.deltaTime;

        timer += Time.deltaTime;

        // Spawn a new background if the cooldown is met
        if (timer >= spawnCooldown)
        {
            SpawnBackground();
            timer = 0;
        }

        // Move each background downwards
        for (int i = backgrounds.Count - 1; i >= 0; i--)
        {
            GameObject background = backgrounds[i];
            if (background != null)
            {
                background.transform.position += Vector3.down * speed * Time.deltaTime;
            }
            else
            {
                // Remove null references from the list
                backgrounds.RemoveAt(i);
            }
        }
    }

    private void SpawnBackground()
    {
        // Instantiate a new background and add it to the list
        GameObject backGround = Instantiate(backgroundPrefab, transform.position, Quaternion.identity);
        backgrounds.Add(backGround);

        // Start coroutine to destroy the background after its lifetime
        StartCoroutine(DestroyAfterLifetime(backGround, lifeTime));
    }

    private IEnumerator DestroyAfterLifetime(GameObject obj, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        if (obj != null)
        {
            backgrounds.Remove(obj); // Remove from the list before destroying
            Destroy(obj);
        }
    }
}
