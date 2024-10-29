using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float rotationSpeed = 10f; // Rotation speed of the camera
    public int rotationWay = 1;

    private void Start()
    {
        int random = Random.Range(0, 2);
        if (random == 0) rotationWay = -1;
        else if (random == 1) rotationWay = 1;
    }

    void Update()
    {
        // Rotate the camera around the Z axis at the specified speed
        transform.Rotate(0, 0, rotationWay * rotationSpeed * Time.deltaTime);
    }
} 
