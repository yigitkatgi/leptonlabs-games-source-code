using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image progressBar; // Assign the circular progress bar image
    public float maxDriftTime = 10f; // Time required to fill the bar

    private float currentDriftTime = 0f;

    private void Start()
    {
        progressBar.fillAmount = 0;
    }

    public void UpdateProgressBar(float driftTime)
    {
        currentDriftTime = driftTime;
        float fillAmount = currentDriftTime / maxDriftTime;
        progressBar.fillAmount = Mathf.Clamp01(fillAmount);
    }

    public void ResetProgressBar()
    {
        currentDriftTime = 0f;
        progressBar.fillAmount = 0f;
    }

    public bool IsBarFull()
    {
        return progressBar.fillAmount >= 1f;
    }

    public void HideProgressBar()
    {
        progressBar.enabled = false;
    }

    public void ShowProgressBar()
    {
        progressBar.enabled = true;
    }

    public void DecreaseProgressBar(float decreaseAmount)
    {
        // Decrease the current drift time
        currentDriftTime -= decreaseAmount;
        currentDriftTime = Mathf.Clamp(currentDriftTime, 0f, maxDriftTime);

        // Update the fill amount based on the new current drift time
        float fillAmount = currentDriftTime / maxDriftTime;
        progressBar.fillAmount = Mathf.Clamp01(fillAmount);
    }
}
