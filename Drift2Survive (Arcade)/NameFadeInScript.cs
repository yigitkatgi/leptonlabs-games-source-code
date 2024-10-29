using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NameFadeInScript : MonoBehaviour
{
    public TMP_Text logoImage;
    public float fadeInDuration = 2f;
    public float delayText = 1.5f;
    public float skipAfterTime = 4f;
    private bool waitOver = false;
    private bool played = false;


    void Start()
    {
        // Ensure the image starts fully transparent
        logoImage.color = new Color(logoImage.color.r, logoImage.color.g, logoImage.color.b, 0);
        StartCoroutine(wait());
        StartCoroutine(SkipToNextScene());
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(delayText);
        waitOver = true;
    }
    
    private IEnumerator SkipToNextScene()
    {
        yield return new WaitForSeconds(skipAfterTime);
        SceneManager.LoadScene("Game");
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color logoColor = logoImage.color;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
            logoImage.color = new Color(logoColor.r, logoColor.g, logoColor.b, alpha);
            yield return null;
        }

        // Ensure the image is fully opaque 
        logoImage.color = new Color(logoColor.r, logoColor.g, logoColor.b, 1);
    }

    void Update()
    {
        if (waitOver == true && played == false) { StartCoroutine(FadeIn()); played = true; }

    }
}
