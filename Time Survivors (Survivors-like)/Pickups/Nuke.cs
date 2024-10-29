using UnityEngine;

public class Nuke : Pickups, ICollectible
{
    private ScreenFlash screenFlash; // Reference to the ScreenFlash script

    private void Start()
    {
        // Find the ScreenFlash script in the scene
        screenFlash = FindObjectOfType<ScreenFlash>();

        if (screenFlash == null)
        {
            Debug.LogError("ScreenFlash not found in the scene!");
        }
    }

    public void Collect()
    {
        NukeManager.Instance.ExecuteNuke(screenFlash, screenFlash.flashDuration, DestroyAllEnemies);
    }

    private void DestroyAllEnemies()
    {
        Debug.Log("Destroying all enemies");
        // Find all game objects with the tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Loop through each enemy and destroy it
        foreach (GameObject enemy in enemies)
        {
            Debug.Log("Destroying enemy: " + enemy.name);
            Destroy(enemy);
        }
    }
}
