using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberBehavior : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    public float rotationSpeed = 100f; // Rotation speed in degrees per second
    public float destroyAfterSeconds = 3f; // Set a default value or override in the prefab
    public float initialRadius = 1f; // Initial radius of the circular path around the player
    public float spiralOutRate = 0.5f; // Rate at which the radius increases per second
    public float returnTime = 0.5f; // Time before destruction when the lightsaber returns to the player
    public float returnSpeed = 5f; // Speed at which the lightsaber returns to the player

    private Transform playerTransform;
    private float currentDamage;
    private float currentAngle = 0f;
    private float currentRadius;
    private bool isReturning = false;

    void Awake()
    {
        currentDamage = weaponData.Damage;
    }

    void Start()
    {
        playerTransform = FindObjectOfType<PlayerMovementScript>().transform; // Get the player transform
        currentRadius = initialRadius; // Initialize the current radius
        StartCoroutine(DestroyAfterTime());
    }

    void Update()
    {
        if (isReturning)
        {
            ReturnToPlayer();
        }
        else
        {
            RotateAndSpiralOut();
        }
    }

    void RotateAndSpiralOut()
    {
        // Update the angle for rotation
        currentAngle += rotationSpeed * Time.deltaTime;
        if (currentAngle >= 360f)
        {
            currentAngle -= 360f;
        }

        // Increase the radius to create the spiral out effect
        currentRadius += spiralOutRate * Time.deltaTime;

        // Calculate the new position
        Vector3 offset = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad)) * currentRadius;
        transform.position = playerTransform.position + offset;

        // Make the lightsaber handle always point towards the player
        Vector3 direction = playerTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90)); // Adjust angle if necessary
    }

    void ReturnToPlayer()
    {
        // Move the lightsaber towards the player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        float step = returnSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, step);

        // Check if the lightsaber has reached the player
        if (Vector3.Distance(transform.position, playerTransform.position) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(GetCurrentDamage(), transform.position);
            }
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
            }
        }
        else if (col.CompareTag("Player") && isReturning)
        {
            Destroy(gameObject); // Destroy the lightsaber when it collides with the player
        }
    }

    private float GetCurrentDamage()
    {
        return currentDamage * FindObjectOfType<PlayerStats>().CurrentMight;
    }

    private IEnumerator DestroyAfterTime()
    {
        float timeToReturn = destroyAfterSeconds - returnTime;
        yield return new WaitForSeconds(timeToReturn);
        isReturning = true; // Start returning to the player
        yield return new WaitForSeconds(returnTime);
        Destroy(gameObject);
    }
}
