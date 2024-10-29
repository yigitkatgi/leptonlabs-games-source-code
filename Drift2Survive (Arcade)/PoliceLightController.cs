using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PoliceLightController : MonoBehaviour
{
    public float shineTime = 0.5f; // Duration for each light to stay on
    public GameObject leftLightObject;
    public GameObject rightLightObject;

    private Light2D leftLight;
    private Light2D rightLight;
    private float timer;

    private void Start()
    {
        leftLight = leftLightObject.GetComponent<Light2D>();
        rightLight = rightLightObject.GetComponent<Light2D>();

        // Initially turn off right light
        rightLight.enabled = false;
        leftLight.enabled = true;

        timer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= shineTime)
        {
            // Toggle the lights
            leftLight.enabled = !leftLight.enabled;
            rightLight.enabled = !rightLight.enabled;

            // Reset the timer
            timer = 0;
        }
    }
}
