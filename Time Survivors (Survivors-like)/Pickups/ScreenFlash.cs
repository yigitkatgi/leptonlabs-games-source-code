using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    public Image flashImage;
    public float flashDuration = 0.5f; // Duration of the flash effect
    public Color flashColor = new Color(1f, 1f, 0.8f, 1f); // Color of the flash

    private void Start()
    {
        if (flashImage != null)
        {
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f); // Ensure initial alpha is 0
        }
    }

    public IEnumerator StartFlash(System.Action midpointAction)
    {
        Debug.Log("Flash start");

        // Fade in
        float halfDuration = flashDuration / 2f;
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / halfDuration;
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, Mathf.Lerp(0f, flashColor.a, normalizedTime));
            yield return null;
        }

        // Call the midpoint action (e.g., destroy enemies)
        midpointAction?.Invoke();

        // Ensure fully visible at peak
        flashImage.color = flashColor;

        // Fade out
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / halfDuration;
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, Mathf.Lerp(flashColor.a, 0f, normalizedTime));
            yield return null;
        }

        // Ensure fully transparent at end
        flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
        Debug.Log("Flash end");
    }
}
