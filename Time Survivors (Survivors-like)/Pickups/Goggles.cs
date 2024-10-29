using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goggles : Pickups, ICollectible
{
    CameraScript cameraS;
    public float sizeAdder;
    public float effectDuration;

    private void Start()
    {
        cameraS = FindObjectOfType<CameraScript>();
        if (cameraS == null)
        {
            Debug.LogError("CameraScript not found in the scene!");
        }
    }

    public void Collect()
    {
        if (cameraS == null)
        {
            return;
        }

        Camera mainCamera = cameraS.mainCamera;
        if (mainCamera == null)
        {
            return;
        }

        mainCamera.orthographicSize += sizeAdder;
        cameraS.StartCoroutine(RemoveSizeAfterDelay(mainCamera, effectDuration));
    }

    private IEnumerator RemoveSizeAfterDelay(Camera mainCamera, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (mainCamera != null)
        {
            mainCamera.orthographicSize -= sizeAdder;
        }
    }
}
