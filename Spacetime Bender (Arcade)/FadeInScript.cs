using UnityEngine;
using TMPro; // Add this namespace for TextMeshPro
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeInScript : MonoBehaviour
{
    public Image logoImage;
    public TMP_Text firstText; // Use TMP_Text for TextMeshPro components
    public TMP_Text secondText; // Use TMP_Text for TextMeshPro components
    public float fadeInDuration = 2f;
    public float secondTextDelay = 1f; // Delay before the second text starts fading in
    public float blinkInterval = 1f; // Time between blinks

    void Start()
    {
        // Ensure the image and texts start fully transparent
        logoImage.color = new Color(logoImage.color.r, logoImage.color.g, logoImage.color.b, 0);
        firstText.color = new Color(firstText.color.r, firstText.color.g, firstText.color.b, 0);
        secondText.color = new Color(secondText.color.r, secondText.color.g, secondText.color.b, 0);

        // Start the fade-in process
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color logoColor = logoImage.color;
        Color firstTextColor = firstText.color;
        Color secondTextColor = secondText.color;

        // Fade in the logo and the first text simultaneously
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
            logoImage.color = new Color(logoColor.r, logoColor.g, logoColor.b, alpha);
            firstText.color = new Color(firstTextColor.r, firstTextColor.g, firstTextColor.b, alpha);
            yield return null;
        }

        // Ensure the logo and first text are fully opaque
        logoImage.color = new Color(logoColor.r, logoColor.g, logoColor.b, 1);
        firstText.color = new Color(firstTextColor.r, firstTextColor.g, firstTextColor.b, 1);

        // Wait for the specified delay before starting to fade in the second text
        yield return new WaitForSeconds(secondTextDelay);

        // Fade in the second text
        elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
            secondText.color = new Color(secondTextColor.r, secondTextColor.g, secondTextColor.b, alpha);
            yield return null;
        }

        // Ensure the second text is fully opaque
        secondText.color = new Color(secondTextColor.r, secondTextColor.g, secondTextColor.b, 1);

        // Start the blinking effect for the second text
        StartCoroutine(BlinkText());
    }

    private IEnumerator BlinkText()
    {
        while (true) // Infinite loop to keep blinking
        {
            // Fade out the text
            float elapsedTime = 0f;
            Color secondTextColor = secondText.color;

            while (elapsedTime < blinkInterval / 2)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / (blinkInterval / 2));
                secondText.color = new Color(secondTextColor.r, secondTextColor.g, secondTextColor.b, alpha);
                yield return null;
            }

            // Fade in the text
            elapsedTime = 0f;
            while (elapsedTime < blinkInterval / 2)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsedTime / (blinkInterval / 2));
                secondText.color = new Color(secondTextColor.r, secondTextColor.g, secondTextColor.b, alpha);
                yield return null;
            }
        }
    }

    void Update()
    {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Game"); // Replace with your main game scene name
        }
    }
}
