using UnityEngine;

public class LogoScript : MonoBehaviour
{
    public float targetScale = 0.4f;  // The target scale to reach
    public float scaleSpeed = 0.1f;   // The speed at which the object scales

    private Vector3 initialScale;     // The initial scale of the object
    private Vector3 targetVectorScale; // The target scale as a Vector3

    void Start()
    {
        // Store the initial scale set in the Inspector
        initialScale = transform.localScale;

        // Set the target scale as a Vector3 (uniform scaling)
        targetVectorScale = new Vector3(targetScale, targetScale, targetScale);
    }

    void Update()
    {
        // Gradually scale the object towards the target scale
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetVectorScale, scaleSpeed * Time.deltaTime);
    }
}
