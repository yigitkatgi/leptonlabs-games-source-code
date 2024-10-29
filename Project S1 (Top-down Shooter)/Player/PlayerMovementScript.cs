using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public float speed = 5f;  // Speed factor
    public GameObject joystickPrefab;  // Prefab for the joystick
    public float maxJoystickDistance = 100f;  // Maximum distance the inner part can move
    public float maxSpeed = 10f;  // Maximum speed of the player

    private GameObject joystickInstance;  // Instance of the joystick
    private RectTransform joystickBackground;  // Joystick background transform
    private RectTransform joystickHandle;  // Joystick handle transform
    private Vector2 startPos;  // Start position of the touch
    private Vector2 direction;  // Direction of movement
    public Vector2 lastDirection;
    private Canvas canvas;  // Reference to the Canvas
    private Rigidbody2D rb;  // Reference to the Rigidbody2D
    private TimeManager tm;

    public bool IsMoving;

    private Camera mainCamera; // Reference to the main camera

    private void Start()
    {
        // Find the Canvas in the scene
        canvas = FindObjectOfType<Canvas>();
        tm = FindObjectOfType<TimeManager>();

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

        // Get the main camera
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleTouch();
        FacePlayer();
    }

    private void FixedUpdate()
    {
        MovePlayer();

    }

    private void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.position.x < Screen.width / 2)
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
                                lastDirection = direction;

                                // Limit the joystick handle movement
                                float distance = Mathf.Min(direction.magnitude, maxJoystickDistance);
                                Vector2 limitedDirection = direction.normalized * distance;

                                // Move the joystick handle
                                joystickHandle.anchoredPosition = limitedDirection;

                                // Set IsMoving to true if there is significant input
                                if (distance > 2f) // Adjust this threshold as needed
                                {
                                    IsMoving = true;
                                }
                            }
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            // Destroy joystick instance
                            Destroy(joystickInstance);
                            direction = Vector2.zero;
                            IsMoving = false;
                            break;
                    }
                }
            }
        }
    }

    private void MovePlayer()
    {
        if (direction != Vector2.zero)
        {
            // Convert direction to world space relative to the camera
            Vector3 screenDirection = new Vector3(direction.x, direction.y, 0);
            Vector3 worldDirection = mainCamera.transform.TransformDirection(screenDirection);
            worldDirection.z = 0; // Ignore any depth axis

            Vector2 movement = worldDirection.normalized * speed * (direction.magnitude / maxJoystickDistance);

            // Clamp the player's speed to maxSpeed
            movement = Vector2.ClampMagnitude(movement, maxSpeed);

            // Apply movement to the Rigidbody2D
            rb.velocity = movement;
        }
        else
        {
            // Stop the player's movement when there is no direction input
            rb.velocity = Vector2.zero;
        }
    }

    private void FacePlayer()
    {
        transform.rotation = mainCamera.transform.rotation;
    }
}
