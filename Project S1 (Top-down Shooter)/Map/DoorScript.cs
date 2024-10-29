using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool opened = false;
    public float targetRotDeg = -120f;
    public float openingSpeed = 1f;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool hasCalledHalfwayScan = false; // Flag to check if halfway Scan has been called
    private bool hasCalledFinalScan = false; // Flag to check if final Scan has been called

    private void Start()
    {
        // Set the initial and target rotations based on the initial orientation
        initialRotation = transform.rotation;
        targetRotation = Quaternion.Euler(0, 0, targetRotDeg) * initialRotation;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            opened = true;
        }
    }

    private void Update()
    {
        if (opened)
        {
            // Rotate the door towards the target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, openingSpeed * Time.deltaTime);

            // Calculate the current angle from the initial position
            float currentAngle = Quaternion.Angle(initialRotation, transform.rotation);
            float totalAngle = Quaternion.Angle(initialRotation, targetRotation);

            // Check if the door is halfway open and hasn't called Scan yet
            if (currentAngle >= totalAngle / 2 && !hasCalledHalfwayScan)
            {
                AstarPath.active.Scan();
                hasCalledHalfwayScan = true;
            }

            // Check if the door has reached the target rotation and hasn't called final Scan yet
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.01f && !hasCalledFinalScan)
            {
                transform.rotation = targetRotation; // Snap to target rotation
                AstarPath.active.Scan();
                opened = false; // Stop further updates
                hasCalledFinalScan = true;
            }
        }
    }
}
