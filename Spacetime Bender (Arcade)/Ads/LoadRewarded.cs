using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class LoadRewarded : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public string androidAdUnitId;
    public string iosAdUnitId;

    private string adUnitId;
    private int rewardType; // Variable to track which reward to give
    private GameManager gm;
    private PlayerCollisionScript pcm;

    void Awake()
    {
#if UNITY_IOS
        adUnitId = iosAdUnitId;
#elif UNITY_ANDROID
        adUnitId = androidAdUnitId;
#endif
    }

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        pcm = FindObjectOfType<PlayerCollisionScript>();
    }

    public void LoadAd(int rewardType)
    {
        this.rewardType = rewardType; // Set the reward type based on the button pressed
        print($"Loading Rewarded Ad for reward type {rewardType}!!");
        Advertisement.Load(adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId.Equals(adUnitId))
        {
            print("Rewarded ad loaded!!");
            ShowAd();
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        print($"Rewarded ad failed to load: {message}");
    }

    public void ShowAd()
    {
        print("Showing rewarded ad!!");
        Advertisement.Show(adUnitId, this);
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        print("Rewarded ad clicked");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(adUnitId) && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            print("Rewarded ad show complete. Distributing the reward.");
            DistributeReward();
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        print($"Rewarded ad show failure: {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        print("Rewarded ad show start");
    }

    private void DistributeReward()
    {
        // Distribute rewards based on the rewardType
        switch (rewardType)
        {
            case 1:
                // Reward for button 1
                print("Reward for Button 1: Granting 100 Coins");
                // Implement reward logic here (e.g., add coins)
                gm.AddGold(30);
                break;
            case 2:
                // Reward for button 2
                print("Reward for Button 2: Granting Extra Life");
                // Implement reward logic here (e.g., grant extra life)
                pcm.OnAdWatched();
                break;
            default:
                print("Unknown reward type");
                break;
        }
    }
}
