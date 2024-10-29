using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class LoadInterstitial : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public string androidAdUnitId;
    public string iosAdUnitId;
    public Action OnAdClosed;

    private string adUnitId;
    private bool isAdLoading = false;

    void Awake()
    {
#if UNITY_IOS
        adUnitId = iosAdUnitId;
#elif UNITY_ANDROID
        adUnitId = androidAdUnitId;
#endif
    }

    public void LoadAd()
    {
        if (isAdLoading) return;

        print("Loading interstitial!!");
        Advertisement.Load(adUnitId, this);
        isAdLoading = true;
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId == adUnitId)
        {
            print("Interstitial loaded!!");
            ShowAd();
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        if (placementId == adUnitId)
        {
            print("Interstitial failed to load");
            isAdLoading = false;
        }
    }

    public void ShowAd()
    {
        print("Showing ad!!");
        Advertisement.Show(adUnitId, this);
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        if (placementId == adUnitId)
        {
            print("Interstitial clicked");
        }
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        // Ensure this callback only responds to the interstitial ad and not banners or other ads
        if (placementId == adUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            print("Interstitial show complete");
            isAdLoading = false;
            OnAdClosed.Invoke();
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        if (placementId == adUnitId)
        {
            print("Interstitial show failure");
            isAdLoading = false;
            OnAdClosed.Invoke();
        }
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        if (placementId == adUnitId)
        {
            print("Interstitial show start");
        }
    }
}
