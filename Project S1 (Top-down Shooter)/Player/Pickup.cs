using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float throwForce = 1f;
    public List<GameObject> gunControllers = new List<GameObject>(); // Ensure these are assigned in the Inspector
    public List<GameObject> possiblePickups = new List<GameObject>(); // Ensure these are assigned in the Inspector

    private GameObject currentGunController;
    private GameObject currentGunPickup;
    private GameObject gunHolder;
    private PlayerMovementScript pm;

    private void Start()
    {
        gunHolder = transform.GetChild(0).gameObject;
        currentGunController = gunHolder.transform.GetChild(0).gameObject;

        if (currentGunController != null)
        {
            SetCurrentPickup();
        }
        else
        {
            Debug.LogError("No initial gun controller found on the player.");
        }

        pm = GetComponent<PlayerMovementScript>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Gun Pickup"))
        {
            PickupWeapon(col.gameObject);
        }
    }

    void PickupWeapon(GameObject foundGunPickup)
    {
        // Drop the current weapon first
        DropWeapon();

        // Determine which weapon is picked up and instantiate the corresponding controller
        GameObject newGun = null;
        if (foundGunPickup.name.Contains("Pistol"))
        {
            newGun = Instantiate(gunControllers[0], gunHolder.transform.position, Quaternion.identity);
            currentGunPickup = possiblePickups[0];
        }
        else if (foundGunPickup.name.Contains("Shotgun"))
        {
            newGun = Instantiate(gunControllers[1], gunHolder.transform.position, Quaternion.identity);
            currentGunPickup = possiblePickups[1];
        }
        else if (foundGunPickup.name.Contains("Carbine"))
        {
            newGun = Instantiate(gunControllers[2], gunHolder.transform.position, Quaternion.identity);
            currentGunPickup = possiblePickups[2];
        }

        if (newGun != null)
        {
            if (currentGunController != null)
            {
                Destroy(currentGunController); // Ensure old gun controller is destroyed
            }
            currentGunController = newGun;
            currentGunController.transform.SetParent(gunHolder.transform);

            // Adjust the weapon's local rotation to match the player's current orientation
            currentGunController.transform.localRotation = possiblePickups[0].transform.rotation; // Reset local rotation
        }

        Destroy(foundGunPickup); // Remove the pickup after it's been used

        // Debugging
        Debug.Log($"Picked up: {newGun?.name}, Set current pickup to: {currentGunPickup?.name}");
    }

    void DropWeapon()
    {
        if (currentGunController != null)
        {
            Destroy(currentGunController); // Destroy current gun controller
        }

        // Instantiate the pickup version of the current gun
        GameObject currentGunDrop = Instantiate(currentGunPickup, transform.position, Quaternion.identity);

        // Start a coroutine to move the gun smoothly
        StartCoroutine(MoveGunSmoothly(currentGunDrop, pm.lastDirection.normalized, throwForce));

        // Temporarily ignore collision with the player to prevent immediate pickup
        Collider2D playerCollider = GetComponent<Collider2D>();
        Collider2D gunPickupCollider = currentGunDrop.GetComponent<Collider2D>();

        if (playerCollider != null && gunPickupCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, gunPickupCollider, true);
            StartCoroutine(ReenableCollision(playerCollider, gunPickupCollider, 0.5f)); // Adjust delay as needed
        }
    }

    IEnumerator MoveGunSmoothly(GameObject gun, Vector2 direction, float distance)
    {
        float duration = 0.5f; // Duration of the movement
        float elapsed = 0f;
        Vector3 startPosition = gun.transform.position;
        Vector3 targetPosition = startPosition + (Vector3)direction * distance;

        PickupObjects pickupObj = gun.GetComponent<PickupObjects>();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime; // Use unscaled delta time to ignore time scaling
            float t = elapsed / duration;

            // Smooth movement using Lerp
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t);

            // Check for collision
            if (pickupObj.hasCollided)
            {
                // Stop at collision point or just before
                gun.transform.position = pickupObj.collisionPoint - (Vector3)direction * 0.1f; // Slightly back from collision point
                yield break;
            }
            else
            {
                gun.transform.position = newPosition;
            }

            yield return null;
        }

        // Ensure final position if not collided
        gun.transform.position = targetPosition;
    }

    IEnumerator ReenableCollision(Collider2D playerCollider, Collider2D gunPickupCollider, float delay)
    {
        yield return new WaitForSeconds(delay);
        Physics2D.IgnoreCollision(playerCollider, gunPickupCollider, false);
    }

    private void SetCurrentPickup()
    {
        if (currentGunController.GetComponent<Pistol>() != null)
        {
            currentGunPickup = possiblePickups[0];
        }
        else if (currentGunController.GetComponent<Shotgun>() != null)
        {
            currentGunPickup = possiblePickups[1];
        }
        else if (currentGunController.GetComponent<Carbine>() != null)
        {
            currentGunPickup = possiblePickups[2];
        }
        else
        {
            Debug.LogError("No matching gun type found for current gun controller.");
        }
    }
}