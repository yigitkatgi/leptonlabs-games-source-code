using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class Skin
    {
        public string name;
        public int price;
        public bool isUnlocked;
        public Button unlockButton;
        public Button selectButton;
        public TMP_Text priceText;
        public TMP_Text nameText;
    }

    public List<Skin> ballSkins = new List<Skin>();
    public List<Skin> obstacleSkins = new List<Skin>();
    public GameObject watchAdPanel; // Reference to the ad panel
    public int adRewardAmount = 10; // Coins rewarded for watching an ad

    private GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        LoadData();
        UpdateUI();

        foreach (var skin in ballSkins)
        {
            skin.nameText.text = skin.name;
        }
    }

    public void UnlockSkin(string skinName)
    {
        Skin skin = ballSkins.Find(s => s.name == skinName) ?? obstacleSkins.Find(s => s.name == skinName);
        if (skin != null && !skin.isUnlocked)
        {
            if (gm.gold >= skin.price)
            {
                Debug.Log($"Attempting to unlock skin: {skinName}, Price: {skin.price}, Current Gold: {gm.gold}");
                gm.AddGold(-skin.price);
                Debug.Log($"Gold after deduction: {gm.gold}");
                skin.isUnlocked = true;
                UpdateUI();
                SaveData();
            }
            else
            {
                // Not enough gold, show the ad panel
                ShowWatchAdPanel();
            }
        }
    }

    public void ShowWatchAdPanel()
    {
        watchAdPanel.SetActive(true);
    }

    public void GoBackFromWatchAdPanel()
    {
        watchAdPanel.SetActive(false); 
    }

    public void WatchAd()
    {
        gm.AddGold(adRewardAmount);

        // Hide the panel after rewarding
        watchAdPanel.SetActive(false);

        // Ensure game is still paused
        gm.PauseGame();
    }

    public void RelockSkin(string skinName)
    {
        Skin skin = ballSkins.Find(s => s.name == skinName) ?? obstacleSkins.Find(s => s.name == skinName);
        if (skin != null)
        {
            skin.isUnlocked = false;
            SaveData();
            UpdateUI();
        }
    }

    public void OnUnlockButtonClick(string skinName)
    {
        UnlockSkin(skinName);
    }

    void SaveData()
    {
        foreach (var skin in ballSkins)
        {
            PlayerPrefs.SetInt(skin.name, skin.isUnlocked ? 1 : 0);
        }
        foreach (var skin in obstacleSkins)
        {
            PlayerPrefs.SetInt(skin.name, skin.isUnlocked ? 1 : 0);
        }
        PlayerPrefs.SetInt("Gold", gm.gold);
        PlayerPrefs.Save(); // Ensure data is saved immediately
    }

    void LoadData()
    {
        foreach (var skin in ballSkins)
        {
            skin.isUnlocked = PlayerPrefs.GetInt(skin.name, 0) == 1;
        }
        foreach (var skin in obstacleSkins)
        {
            skin.isUnlocked = PlayerPrefs.GetInt(skin.name, 0) == 1;
        }
        gm.gold = PlayerPrefs.GetInt("Gold", 0);
    }

    void UpdateUI()
    {
        foreach (var skin in ballSkins)
        {
            skin.unlockButton.gameObject.SetActive(!skin.isUnlocked);
            skin.selectButton.interactable = skin.isUnlocked;
            skin.priceText.text = skin.isUnlocked ? "" : skin.price.ToString();
            skin.nameText.enabled = skin.isUnlocked;

            // Set the button click event
            skin.unlockButton.onClick.RemoveAllListeners();
            skin.unlockButton.onClick.AddListener(() => OnUnlockButtonClick(skin.name));
        }
        foreach (var skin in obstacleSkins)
        {
            skin.unlockButton.gameObject.SetActive(!skin.isUnlocked);
            skin.selectButton.interactable = skin.isUnlocked;
            skin.priceText.text = skin.isUnlocked ? "" : skin.price.ToString();

            // Set the button click event
            skin.unlockButton.onClick.RemoveAllListeners();
            skin.unlockButton.onClick.AddListener(() => OnUnlockButtonClick(skin.name));
        }
    }
}
