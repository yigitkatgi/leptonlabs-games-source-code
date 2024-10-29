using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public float speed = 5f;  // Speed factor
    public GameObject joystickPrefab;  // Prefab for the joystick
    public float maxJoystickDistance = 100f;  // Maximum distance the inner part can move
    public float maxSpeed = 10f;  // Maximum speed of the player
    public float dashSpeed = 20f;  // Speed for dashing
    public float jumpForce = 15f;  // Force applied for jumping
    public float dashDuration = 0.5f;  // Duration of the dash
    public float dashCooldown = 1f;  // Cooldown time for the dash

    private SpriteRenderer sr;
    private GameObject joystickInstance;  // Instance of the joystick
    private RectTransform joystickBackground;  // Joystick background transform
    private RectTransform joystickHandle;  // Joystick handle transform
    private Vector2 startPos;  // Start position of the touch
    private Vector2 direction;  // Direction of movement
    public int lastDirection;
    private Canvas canvas;  // Reference to the Canvas
    private Rigidbody2D rb;  // Reference to the Rigidbody2D
    private PlayerCombatScript pc;

    public bool isMoving;
    public bool isGrounded = true;  // Track whether the player is on the ground
    public bool isDashing = false;  // Track if the player is dashing
    private float dashTime;  // Time left for the dash
    private float dashCooldownTime;  // Time left for dash cooldown

    private void Start()
    {
        // Find the Canvas in the scene
        canvas = FindObjectOfType<Canvas>();
        sr = GetComponent<SpriteRenderer>();
        pc = GetComponent<PlayerCombatScript>();
        lastDirection = +1;

        if (canvas == null)
        {
            Debug.LogError("No Canvas found in the scene.");
        }

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D found on the player.");
        }
    }

    private void Update()
    {
        HandleTouch();
        FacePlayer();
        if (!pc.isAttacking && !isDashing) MovePlayer();
        if (isDashing) HandleDash();
        if (dashCooldownTime > 0) dashCooldownTime -= Time.deltaTime;
    }

    private void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.position.x < Screen.width / 2 && !isDashing)
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            // Spawn joystick at touch position
                            startPos = touch.position;
                            joystickInstance = Instantiate(joystickPrefab, canvas.transform);
                            joystickInstance.transform.position = startPos;

                            // Get references to the background and handle
                            joystickBackground = joystickInstance.transform.GetChild(0).GetComponent<RectTransform>();
                            joystickHandle = joystickInstance.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
                            break;

                        case TouchPhase.Moved:
                            if (joystickInstance)
                            {
                                // Calculate direction and move joystick handle
                                Vector2 currentPos = touch.position;
                                direction = currentPos - startPos;
                                lastDirection = (int)Mathf.Sign(direction.x);

                                // Limit the joystick handle movement
                                float distance = Mathf.Min(direction.magnitude, maxJoystickDistance);
                                Vector2 limitedDirection = direction.normalized * distance;

                                // Move the joystick handle
                                joystickHandle.anchoredPosition = limitedDirection;

                                // Set IsMoving to true if there is significant input
                                if (distance > 2f) // Adjust this threshold as needed
                                {
                                    isMoving = true;
                                }
                            }
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            // Destroy joystick instance
                            if (joystickInstance)
                            {
                                Destroy(joystickInstance);
                                joystickInstance = null;
                            }
                            direction = Vector2.zero;
                            isMoving = false;
                            break;
                    }
                }
                else if (touch.position.x >= Screen.width / 2)
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            // Record the start position for swipes on the right side
                            startPos = touch.position;
                            break;

                        case TouchPhase.Moved:
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            // Handle dashing or jumping based on swipe direction
                            Vector2 swipeDirection = touch.position - startPos;
                            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                            {
                                // Horizontal swipe - dash
                                if (dashCooldownTime <= 0)
                                {
                                    Dash((int)Mathf.Sign(swipeDirection.x));
                                }
                            }
                            else if (swipeDirection.y > 0 && isGrounded && !isDashing)
                            {
                                // Vertical swipe upwards - jump
                                Jump();
                            }
                            break;
                    }
                }
            }
        }
        else
        {
            // Ensure joystick is destroyed if no touches are present
            if (joystickInstance)
            {
                Destroy(joystickInstance);
                joystickInstance = null;
            }
            direction = Vector2.zero;
            isMoving = false;
        }
    }

    private void MovePlayer()
    {
        if (direction != Vector2.zero)
        {
            // Only consider the horizontal component of the direction
            float horizontalInput = direction.x;

            // Calculate movement vector based on horizontal input
            Vector2 movement = new Vector2(horizontalInput, 0).normalized * speed * (Mathf.Abs(horizontalInput) / maxJoystickDistance);

            // Clamp the player's speed to maxSpeed
            movement = Vector2.ClampMagnitude(movement, maxSpeed);

            // Apply movement to the Rigidbody2D
            rb.velocity = new Vector2(movement.x, rb.velocity.y);
        }
        else
        {
            // Stop the player's horizontal movement when there is no direction input
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void FacePlayer()
    {
        if (lastDirection > 0)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }
    }

    private void Dash(int direction)
    {
        // Apply dash force in the specified direction
        rb.velocity = new Vector2(direction * dashSpeed, rb.velocity.y);
        isDashing = true;
        dashTime = dashDuration;
        dashCooldownTime = dashCooldown;  // Set cooldown after dashing
        lastDirection = direction;  // Update the last direction based on the dash
    }

    private void HandleDash()
    {
        if (dashTime > 0)
        {
            dashTime -= Time.deltaTime;
        }
        else
        {
            isDashing = false;
        }
    }

    private void Jump()
    {
        // Apply jump force
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Set isGrounded to true when the player touches the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
