using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RewindManager : MonoBehaviour
{
    public float recordInterval = 0.1f; // Time between position records
    public float rewindSpeed = 5f; // Global rewind speed
    public float skipDelay = 1f; // Delay before allowing skip
    public TMP_Text skipRewind;

    private List<Vector2> playerPositions = new List<Vector2>();
    private List<Quaternion> cameraRotations = new List<Quaternion>();
    private Dictionary<GameObject, List<Vector2>> obstaclePositions = new Dictionary<GameObject, List<Vector2>>();
    private bool isRewinding = false;
    private bool canSkip = false; // Flag to control when skipping is allowed

    private GameObject player;
    private List<GameObject> obstacles = new List<GameObject>();
    private SpawnerScript spawner;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        spawner = FindObjectOfType<SpawnerScript>();

        StartCoroutine(RecordPositions());
    }

    void Update()
    {
        if (isRewinding)
        {
            // Disable physics and collider for player during rewind
            player.GetComponent<Rigidbody2D>().simulated = false;
            player.GetComponent<Collider2D>().enabled = false;

            // Allow skipping after the delay
            if (canSkip && Input.touchCount > 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            foreach (var obstacle in obstacles)
            {
                if (obstacle != null)
                {
                    obstacle.GetComponent<Rigidbody2D>().simulated = false;
                }
            }
        }
    }

    private IEnumerator RecordPositions()
    {
        while (true)
        {
            if (!isRewinding)
            {
                playerPositions.Add(player.transform.position);
                cameraRotations.Add(Camera.main.transform.rotation);

                foreach (var obstacle in obstacles)
                {
                    if (obstacle != null)
                    {
                        if (!obstaclePositions.ContainsKey(obstacle))
                        {
                            obstaclePositions[obstacle] = new List<Vector2>();
                        }
                        obstaclePositions[obstacle].Add(obstacle.transform.position);
                    }
                }
            }
            yield return new WaitForSeconds(recordInterval);
        }
    }

    public void RegisterObstacle(GameObject obstacle)
    {
        obstacles.Add(obstacle);
        obstaclePositions[obstacle] = new List<Vector2>();
    }

    public void StartRewind()
    {
        isRewinding = true;
        skipRewind.gameObject.SetActive(true);
        StartCoroutine(Rewind());
        StartCoroutine(EnableSkipAfterDelay()); // Start the coroutine to enable skipping after the delay
    }

    private IEnumerator Rewind()
    {
        spawner.enabled = false;

        // Determine the maximum number of recorded frames
        int maxFrames = playerPositions.Count;

        for (int i = 0; i < maxFrames; i++)
        {
            if (i < playerPositions.Count)
            {
                player.transform.position = playerPositions[maxFrames - 1 - i];
            }

            if (i < cameraRotations.Count)
            {
                Camera.main.transform.rotation = cameraRotations[maxFrames - 1 - i];
            }

            foreach (var obstacle in obstacles)
            {
                if (obstacle != null && obstaclePositions.ContainsKey(obstacle))
                {
                    var obstacleFrames = obstaclePositions[obstacle];
                    if (i < obstacleFrames.Count)
                    {
                        obstacle.transform.position = obstacleFrames[obstacleFrames.Count - 1 - i];
                    }
                    else if (obstacleFrames.Count > 0)
                    {
                        // If there are no more recorded positions, use the initial position
                        obstacle.transform.position = obstacleFrames[0];
                    }
                }
            }

            yield return new WaitForSeconds(recordInterval / rewindSpeed);
        }

        // Once rewinding is done, reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator EnableSkipAfterDelay()
    {
        yield return new WaitForSeconds(skipDelay);
        canSkip = true; // Allow skipping after the delay
    }
}
