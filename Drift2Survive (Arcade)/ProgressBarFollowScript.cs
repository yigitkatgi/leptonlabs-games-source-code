using UnityEngine;
using TMPro;  // Make sure to include this for handling TextMeshPro elements

public class ProgressBarFollowScript : MonoBehaviour
{
    public Transform playerTransform;  // Reference to the player's transform
    public Vector3 offset;  // Offset to adjust the position of the progress bar relative to the player
    public RectTransform progressBarRectTransform;  // The RectTransform of the progress bar UI element
    public TMP_Text speedometerText;  // Reference to the TextMeshPro UI element for the speedometer
    public Vector3 speedometerOffset;  // Offset to adjust the position of the speedometer relative to the player
    public float smoothSpeed = 0.2f;  // Smoothing speed for UI element movement

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Update the position of the progress bar with smoothing
            UpdateUIElementPosition(progressBarRectTransform, offset);

            // Update the position of the speedometer with smoothing
            UpdateUIElementPosition(speedometerText.rectTransform, speedometerOffset);
        }
    }

    void UpdateUIElementPosition(RectTransform uiElement, Vector3 elementOffset)
    {
        // Convert the player's world position to screen space
        Vector3 screenPos = mainCamera.WorldToScreenPoint(playerTransform.position + elementOffset);

        // Smoothly interpolate the UI element's position to the target screen position
        uiElement.position = Vector3.Lerp(uiElement.position, screenPos, smoothSpeed);
    }
}
