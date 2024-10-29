using System.Collections;
using UnityEngine;

public class SupremeMagnet : Pickups, ICollectible
{
    PlayerStats player;
    public float effectDuration;
    private float originalMagnet;

    public void Collect()
    {
        player = FindObjectOfType<PlayerStats>();
        if (player == null)
        {
            Debug.LogError("PlayerStats not found!");
            return;
        }

        // Store the original magnet value
        originalMagnet = player.CurrentMagnet;

        // Set the magnet value to a very high number
        player.CurrentMagnet = 9999;

        // Start the coroutine to revert the magnet value after some time on the player object
        player.StartCoroutine(RevertMagnetAfterDelay(effectDuration));
    }

    private IEnumerator RevertMagnetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Revert the magnet value to the original value
        if (player != null)
        {
            player.CurrentMagnet = originalMagnet;
        }
        else
        {
            Debug.LogError("PlayerStats reference lost before reverting magnet value!");
        }
    }
}
