using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private PlayerMovementScript pm;
    public float minTimeScale = 0.1f;
    public float timeScaleLerpSpeed = 0.05f;
    public float actionTime = 0.05f;

    public bool isActionTimeActive;

    void Start()
    {
        Time.timeScale = minTimeScale;
        pm = FindObjectOfType<PlayerMovementScript>();
        if (pm == null)
        {
            Debug.LogError("PlayerMovementScript not found in the scene.");
        }
    }

    void Update()
    {
        float targetTimeScale = pm.IsMoving || isActionTimeActive ? 1f : minTimeScale;
        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, timeScaleLerpSpeed);
    }

    public void TriggerActionPassTime()
    {
        if (!isActionTimeActive)
        {
            StartCoroutine(ActionPassTime(actionTime));
        }
    }

    IEnumerator ActionPassTime(float time)
    {
        isActionTimeActive = true;
        yield return new WaitForSecondsRealtime(time);
        isActionTimeActive = false;
    }
}
