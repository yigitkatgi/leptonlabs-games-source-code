using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ShopManager;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class Car
    {
        public string name;
        public int price;
        public bool isUnlocked;
        public Sprite skin;
        public Button unlockButton;
        public Button selectButton;
        public TMP_Text priceText;
        public TMP_Text nameText;
    }

    public List<Car> cars = new List<Car>();
    public GameObject watchAdPanel; // Reference to the ad panel
    public int adRewardAmount = 300; // Coins rewarded for watching an ad
    public Sprite defaultSkin;
    public SpriteRenderer sr; // Reference to the player sprite renderer

    private GameManager gm;
    private PlayerMovementScript pm;
    private Sprite currentlyChosenSkin;

    void Awake()
    {
        gm = FindObjectOfType<GameManager>(); // Reference GameManager safely
        pm = sr.gameObject.GetComponent<PlayerMovementScript>();

        LoadData(); // Load saved data first
        InitializeCarSkin(); // Set the player's current skin
        UpdateUI(); // Update UI based on loaded data
    }

    private void LateUpdate()
    {
        if (!pm.isMonsterMode) sr.sprite = currentlyChosenSkin;
    }

    private void InitializeCarSkin()
    {
        // Set the current skin of the player
        string carName = PlayerPrefs.GetString("currentSkin", "none");

        if (carName == "none")
        {
            currentlyChosenSkin = defaultSkin; // Default skin if no selection
            Debug.Log("No PlayerPref found. Assigning the default skin.");
        }
        else
        {
            Car currentCar = cars.Find(s => s.name == carName);
            if (currentCar != null)
            {
                currentlyChosenSkin = currentCar.skin;
                Debug.Log($"Assigned found skin {sr.sprite}");
            }
        }
    }

    public void UnlockSkin(string carName)
    {
        Car car = cars.Find(s => s.name == carName);
        if (car != null && !car.isUnlocked)
        {
            if (gm.gold >= car.price)
            {
                gm.AddGold(-car.price);
                car.isUnlocked = true;
                SaveData(); // Save the state after unlocking
                UpdateUI();
            }
            else
            {
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
        watchAdPanel.SetActive(false);
    }

    public void RelockSkin(string carName)
    {
        Car car = cars.Find(s => s.name == carName);
        if (car != null)
        {
            car.isUnlocked = false;
            SaveData();
            UpdateUI();
        }
    }

    public void OnUnlockButtonClick(string carName)
    {
        UnlockSkin(carName);
    }

    public void OnSelectButtonClick(string carName)
    {
        // Find the associated car
        Car car = cars.Find(s => s.name == carName);

        if (car != null)
        {
            Debug.Log($"Car found: {car.name}. Changing sprite to: {car.skin.name}");

            // Set the sprite renderer to that car's skin
            currentlyChosenSkin = car.skin;

            // Save the skin choice to PlayerPrefs
            PlayerPrefs.SetString("currentSkin", car.name);
            PlayerPrefs.Save(); // Save immediately to ensure it's stored

            Debug.Log($"Sprite changed to: {sr.sprite.name}");
        }
        else
        {
            Debug.LogError($"Car with name {carName} not found.");
        }
    }


    void SaveData()
    {
        foreach (var car in cars)
        {
            PlayerPrefs.SetInt(car.name, car.isUnlocked ? 1 : 0);
        }
        PlayerPrefs.SetInt("Gold", gm.gold);
        PlayerPrefs.Save();
    }

    void LoadData()
    {
        foreach (var car in cars)
        {
            car.isUnlocked = PlayerPrefs.GetInt(car.name, 0) == 1;
        }
        gm.gold = PlayerPrefs.GetInt("Gold", 0);
    }

    void UpdateUI()
    {
        foreach (var car in cars)
        {
            car.unlockButton.gameObject.SetActive(!car.isUnlocked);
            car.selectButton.interactable = car.isUnlocked;
            car.priceText.text = car.isUnlocked ? "" : car.price.ToString();
            car.nameText.enabled = car.isUnlocked;

            // Assign unlock button functionality
            car.unlockButton.onClick.RemoveAllListeners();
            car.unlockButton.onClick.AddListener(() => OnUnlockButtonClick(car.name));

            // Assign select button functionality
            car.selectButton.onClick.RemoveAllListeners();
            car.selectButton.onClick.AddListener(() => OnSelectButtonClick(car.name));
        }
    }

    public void ChooseDefaultSkin()
    {
        // Set the sprite renderer to that car's skin
        currentlyChosenSkin = defaultSkin;

        // Save the skin choice to PlayerPrefs
        PlayerPrefs.SetString("currentSkin", "none");
        PlayerPrefs.Save(); // Save immediately to ensure it's stored
    }
}
