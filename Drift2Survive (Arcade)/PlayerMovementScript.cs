using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerMovementScript : MonoBehaviour
{
    public float moveSpeed;
    public float frictionCoefficient = 0.1f;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;
    public float driftThreshold = 0.5f; // Threshold to detect drifting
    public Vector2 moveDirection;
    public Animator animator;
    public int goldForEat = 10;
    [HideInInspector]
    public float driftDuration;

    [Header("GoldPopUp")]
    public GameObject goldPopUp;
    public Canvas canvas;

    [Header("MonsterMode")]
    public MonsterTrailController monsterTrailController;
    public Color monsterBarColor = Color.red;
    public ProgressBar progressBar; // Assign the progress bar script
    public Sprite monsterSprite; // Assign the monster car sprite
    public float monsterModeDuration = 10f; // Duration of monster mode
    public float increaseFillRate = 1f;
    public float decreaseFillRate = 0.5f;
    public float monsterColliderSideLength = 5.5f;
    public float monsterMovementSpeedMultiplier = 2f;


    [Header("Lights")]
    public GameObject leftHeadlightObject;
    public GameObject rightHeadlightObject;

    private Rigidbody2D rb;
    private Joystick joystick;
    private TrailRenderer rearLeftTrail;
    private TrailRenderer rearRightTrail;
    private Vector2 previousInputDirection;
    private Vector3 lastPosition;
    [HideInInspector]
    public bool isMonsterMode = false;
    private Sprite normalSprite; // Store the normal car sprite
    private GameManager gameManager; // Reference to the GameManager
    private Color originalBarColor; // Store the original color of the progress bar
    private Light2D leftLight;
    private Light2D rightLight;
    private Light2D monsterLight;
    private float currentDriftDuration;
    private BoxCollider2D col;
    private Vector2 originalColSize;
    private float originalMoveSpeed;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joystick = FindObjectOfType<Joystick>();
        monsterLight = GetComponent<Light2D>();
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        monsterLight.enabled = false;
        canvas = joystick.GetComponentInParent<Canvas>();

        leftLight = leftHeadlightObject.GetComponent<Light2D>();
        rightLight = rightHeadlightObject.GetComponent<Light2D>();

        rearLeftTrail = rearLeftWheel.GetComponent<TrailRenderer>();
        rearRightTrail = rearRightWheel.GetComponent<TrailRenderer>();

        gameManager = FindObjectOfType<GameManager>();

        // Initially disable the trails
        rearLeftTrail.emitting = false;
        rearRightTrail.emitting = false;
        previousInputDirection = Vector2.zero;

        // Initialize position tracking
        lastPosition = transform.position;

        // Capture the initial sprite as the normal sprite
        normalSprite = GetComponent<SpriteRenderer>().sprite;

        // Store the original color of the progress bar
        originalBarColor = progressBar.progressBar.color;

        driftDuration = 0f;
    }

    void FixedUpdate()
    {
        moveDirection = new Vector2(joystick.Horizontal(), joystick.Vertical()).normalized;
        Move(moveDirection);
        ApplyFriction();

        // Only detect drift and calculate drift distance when not in monster mode
        if (!isMonsterMode)
        {
            DetectDrift(moveDirection);
        }
    }

    void Move(Vector2 moveDirection)
    {
        Vector2 force = moveDirection * moveSpeed;
        rb.AddForce(force);

        if (force != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void ApplyFriction()
    {
        Vector2 frictionForce = -rb.velocity * frictionCoefficient;
        rb.AddForce(frictionForce);
    }

    void DetectDrift(Vector2 inputDirection)
    {
        if (inputDirection.magnitude > 0.1f)
        {
            float angleDifference = Vector2.Angle(inputDirection, rb.velocity.normalized);
            if (angleDifference > driftThreshold)
            {
                // Drifting detected
                rearLeftTrail.emitting = true;
                rearRightTrail.emitting = true;

                // Calculate drift time
                driftDuration += Time.deltaTime;
                currentDriftDuration += Time.deltaTime;

                // Update the progress bar based on drift time
                progressBar.UpdateProgressBar(currentDriftDuration * increaseFillRate);

                // Check if the drift time threshold is met to activate monster mode
                if (progressBar.IsBarFull() && !isMonsterMode)
                {
                    ActivateMonsterMode();
                    currentDriftDuration = 0;
                }
            }
            else
            {
                // Not drifting
                rearLeftTrail.emitting = false;
                rearRightTrail.emitting = false;

                // Decrease progress bar if not drifting
                DecreaseProgressWhileNotDrifting();
            }
        }
        else
        {
            // No significant input
            rearLeftTrail.emitting = false;
            rearRightTrail.emitting = false;

            // Decrease progress bar if no input
            DecreaseProgressWhileNotDrifting();
        }

        lastPosition = transform.position;
        previousInputDirection = inputDirection;
    }

    void DecreaseProgressWhileNotDrifting()
    {
        // Decrease the current drift duration
        currentDriftDuration -= Time.deltaTime * decreaseFillRate;

        // Ensure the current drift duration doesn't go below zero
        currentDriftDuration = Mathf.Max(currentDriftDuration, 0);

        // Update the progress bar based on the new current drift duration
        progressBar.UpdateProgressBar(currentDriftDuration);
    }

    void ActivateMonsterMode()
    {
        isMonsterMode = true;

        // Increase movement speed
        originalMoveSpeed = moveSpeed;
        moveSpeed = moveSpeed * monsterMovementSpeedMultiplier;

        // Increase size of collider
        originalColSize = col.size;
        col.size = new Vector2 (monsterColliderSideLength, monsterColliderSideLength);

        // Enable monster trail
        monsterTrailController.StartFireTrail();

        // Handle lights
        monsterLight.enabled = true;
        leftLight.enabled = false;
        rightLight.enabled = false;

        // Disable normal drift trail
        rearLeftTrail.enabled = false;
        rearRightTrail.enabled = false;

        animator.SetBool("isMonsterMode", isMonsterMode);

        // Store the current sprite as the normal sprite before changing to monster mode
        normalSprite = sr.sprite;

        // Switch to monster mode sprite
        sr.sprite = monsterSprite;

        // Flip the sprite 
        sr.transform.rotation = Quaternion.Euler(0, 0, 90);

        // Start the timer to gradually deplete the circular bar
        StartCoroutine(MonsterModeTimer());
    }

    IEnumerator MonsterModeTimer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < monsterModeDuration)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = Mathf.Clamp01(1f - (elapsedTime / monsterModeDuration));
            progressBar.progressBar.fillAmount = remainingTime; // Deplete the bar gradually

            // Change the color of the bar during monster mode
            progressBar.progressBar.color = monsterBarColor;

            yield return null;
        }

        DeactivateMonsterMode();
    }

    void DeactivateMonsterMode()
    {
        isMonsterMode = false;

        // Set move speed back
        moveSpeed = originalMoveSpeed;

        // Set collider size back
        col.size = originalColSize;

        // Enable monster trail
        monsterTrailController.EndFireTrail();

        // Handle lights
        monsterLight.enabled = false;
        leftLight.enabled = true;
        rightLight.enabled = true;

        // Enable normal drift trail
        rearLeftTrail.enabled = true;
        rearRightTrail.enabled = true;

        animator.SetBool("isMonsterMode", isMonsterMode);

        // Revert to the stored normal sprite
        sr.sprite = normalSprite;

        // Revert flip
        sr.transform.rotation = Quaternion.identity;

        // Reset the progress bar color to the original color
        progressBar.progressBar.color = originalBarColor;

        // Reset the progress bar and drift distance
        progressBar.ResetProgressBar();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isMonsterMode)
            {
                // Determine the direction of the police car relative to the player
                Vector2 collisionDirection = collision.transform.position - transform.position;

                // Calculate the angle in degrees
                float angle = Mathf.Atan2(collisionDirection.y, collisionDirection.x) * Mathf.Rad2Deg;

                // Apply the rotation to the sprite renderer
                sr.transform.rotation = Quaternion.Euler(0, 0, angle + 90);

                // Trigger the eating animation
                animator.SetTrigger("Eat");

                // Monster mode: Destroy the enemy and add gold
                Destroy(collision.gameObject);
                gameManager.AddGold(goldForEat);
                // Gold popup
                var goldPopup = Instantiate(goldPopUp, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, canvas.transform);
                goldPopup.GetComponent<TMP_Text>().text = goldForEat.ToString();
            }
            else
            {
                // Normal mode: Destroy both the player and the enemy
                Destroy(collision.gameObject);
                gameManager.ShowGameOverScreen();
            }
        }
    }
}
