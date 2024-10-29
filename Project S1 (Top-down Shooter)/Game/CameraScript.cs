using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float rotationLerpSpeed = 5f; // Speed at which the camera rotates
    public float sensitivity = 1f; // Sensitivity for aiming

    private WeaponBase weapon;
    private float currentAngle;
    private float aimAngle;
    private float targetAngle;

    private void Update()
    {
        // Get the current weapon attached to the player
        weapon = player.GetComponentInChildren<WeaponBase>();

        if (weapon != null && weapon.HasAimInput())
        {
            // Make the camera rotate according to aim input only if there is input
            currentAngle = transform.eulerAngles.z; // Correct way to get the current Z angle

            aimAngle = weapon.aimAngle;
            targetAngle = Mathf.LerpAngle(currentAngle, aimAngle - 90, rotationLerpSpeed * Time.unscaledDeltaTime * sensitivity);

            transform.eulerAngles = new Vector3(0, 0, targetAngle);
        }

        // Calculate the rotated offset
        Quaternion cameraRotation = Quaternion.Euler(0, 0, transform.eulerAngles.z);
        Vector3 rotatedOffset = cameraRotation * offset;

        // Make the camera follow the player with the rotated offset
        transform.position = player.position + rotatedOffset;
    }
}
