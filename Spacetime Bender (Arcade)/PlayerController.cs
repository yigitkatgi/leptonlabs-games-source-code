using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveFactor = 0.8f;
    public float rotateSpeed = 300f;
    public float minTimeScale = 0.3f;
    public float minSizeScale = 0.1f;
    public float scaleLerpSpeed = 0.1f;  // Increased to make size adjust faster
    public float timeScaleFactor = 0.3f; // Lowered to make time scale adjust faster
    public float speedThreshold = 2.5f;
    public float speedDamping = 0.95f; // Reduced to make speed drop quicker
    public float movementSmoothness = 0.15f;
    public float momentumDecay = 0.1f;  // Lowered to make momentum decay faster
    public Camera mainCamera;
    public float buffer = 0.1f;

    private Vector2 initialTouchPosition;
    private Vector2 lastTouchPosition;
    private Vector3 originalScale;
    private float currentSpeed;
    private float smoothedSpeed = 0f;
    private float momentumSpeed = 0f; // Smoothed momentum-based speed
    private Vector3 targetPosition;

    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        originalScale = transform.localScale;
        targetPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                initialTouchPosition = touchPosition;
                lastTouchPosition = touchPosition;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = touchPosition - lastTouchPosition;
                lastTouchPosition = touchPosition;
                currentSpeed = touchDelta.magnitude / Time.unscaledDeltaTime;

                smoothedSpeed = Mathf.Lerp(smoothedSpeed, currentSpeed, 1f - speedDamping);

                // Update momentum-based speed
                momentumSpeed = Mathf.Lerp(momentumSpeed, smoothedSpeed, momentumDecay);

                if (smoothedSpeed > speedThreshold)
                {
                    targetPosition += new Vector3(touchDelta.x, touchDelta.y, 0) * moveFactor;
                }
            }
            else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                smoothedSpeed *= speedDamping;
                momentumSpeed *= momentumDecay; // Directly reduce momentumSpeed
                currentSpeed = 0f;
            }
        }
        else
        {
            smoothedSpeed *= speedDamping;
            momentumSpeed *= momentumDecay; // Decay momentum speed faster when no touch input
            currentSpeed = 0f;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, movementSmoothness);

        UpdateSize();
        UpdateTimeScale();

        // Do not let player out of the boundaries
        KeepPlayerWithinBounds();
    }

    void UpdateSize()
    {
        // Use momentumSpeed for smoother size adjustments
        float targetScaleFactor = momentumSpeed > 0 ? Mathf.Clamp(originalScale.x / momentumSpeed, minSizeScale, originalScale.x) : originalScale.x;
        Vector3 targetScale = new Vector3(1, 1, 1) * targetScaleFactor;

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleLerpSpeed);
    }

    void UpdateTimeScale()
    {
        // Use momentumSpeed for smoother time scale adjustments
        Time.timeScale = Mathf.Clamp(momentumSpeed / timeScaleFactor, minTimeScale, 1f);
    }

    private void KeepPlayerWithinBounds()
    {
        Vector3 screenPos = mainCamera.WorldToViewportPoint(transform.position);

        // Clamp the position within the viewport, considering the buffer
        screenPos.x = Mathf.Clamp(screenPos.x, buffer, 1f - buffer);
        screenPos.y = Mathf.Clamp(screenPos.y, buffer, 1f - buffer);

        // Convert the clamped position back to world space
        transform.position = mainCamera.ViewportToWorldPoint(screenPos);
    }
}
