using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public Vector3 offset;    // Offset from the player
    public float smoothSpeed = 0.125f;  // Smoothing speed
    public float shakeDuration = 0.5f;  // Duration of the shake effect
    public float shakeMagnitude = 0.1f;  // Magnitude of the shake effect

    private float shakeTimeRemaining = 0f;  // Time remaining for the shake effect

    // Map boundaries
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    void LateUpdate()
    {
        if (player != null)
        {
            // Desired position is the player's position plus the offset
            Vector3 desiredPosition = player.position + offset;

            // If shaking, add random offset
            if (shakeTimeRemaining > 0)
            {
                desiredPosition += Random.insideUnitSphere * shakeMagnitude;
                shakeTimeRemaining -= Time.deltaTime;
            }

            // Clamp the camera's position within the map boundaries
            float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
            float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);

            Vector3 clampedPosition = new Vector3(clampedX, clampedY, desiredPosition.z);

            // Smoothly interpolate between the camera's current position and the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);

            // Update the camera's position
            transform.position = smoothedPosition;
        }
    }

    // Method to trigger the camera shake
    public void TriggerShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTimeRemaining = duration;
    }
}
