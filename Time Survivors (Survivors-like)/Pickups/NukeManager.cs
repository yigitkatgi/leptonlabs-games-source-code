using System.Collections;
using UnityEngine;

public class NukeManager : MonoBehaviour
{
    private static NukeManager instance;

    public static NukeManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("NukeManager");
                instance = go.AddComponent<NukeManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public void ExecuteNuke(ScreenFlash screenFlash, float flashDuration, System.Action midpointAction)
    {
        StartCoroutine(ExecuteNukeCoroutine(screenFlash, flashDuration, midpointAction));
    }

    private IEnumerator ExecuteNukeCoroutine(ScreenFlash screenFlash, float flashDuration, System.Action midpointAction)
    {
        if (screenFlash != null)
        {
            Debug.Log("Starting flash effect");
            yield return StartCoroutine(screenFlash.StartFlash(midpointAction));
            Debug.Log("Flash effect complete");
        }
        else
        {
            Debug.LogWarning("ScreenFlash reference is null!");
        }
    }
}
