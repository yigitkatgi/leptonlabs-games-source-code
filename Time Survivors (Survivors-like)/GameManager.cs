using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState {GamePlay, Paused, GameOver, LevelUp}; // Define the different states of the game

    public GameState currentState; // Store the current state of the game
    public GameState previousState; // Store the previous state of the game

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject gameplayScreen;
    public GameObject levelUpScreen;

    [Header("Current Stat Displays")]
    // Current stat displays
    public Text currentHealthDisplay;
    public Text currentRecoveryDisplay;
    public Text currentMoveSpeedDisplay;
    public Text currentMightDisplay;
    public Text currentProjectileSpeedDisplay;
    public Text currentMagnetDisplay;

    [Header("Results Screen Display")]
    public Image chosenCharacterImage;
    public Text chosenCharacterName;
    public Text levelReachedDisplay;
    public Text timeSurvivedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(6);
    public List<Image> chosenPassiveItemsUI = new List<Image>(6);

    public Text stopwatchDisplay;

    public bool isGameOver = false;
    public bool isChoosingLevel = false;

    public GameObject playerObject;

    void Awake()
    {
        // Warning check to see if there is another singleton of this kind in the game
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Extra " + this + "deleted");
            Destroy(gameObject);
        }

        DisableScreens();
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.GamePlay:
                // Code for the gameplay state
                gameplayScreen.SetActive(true);
                CheckForPauseAndResume();
                stopwatchDisplay.text = Stopwatch(Time.timeSinceLevelLoad);
                break;

            case GameState.Paused:
                // Code for the paused state
                gameplayScreen.SetActive(false);
                CheckForPauseAndResume();
                break;

            case GameState.GameOver:
                // Code for the gameover state
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0;
                    Debug.Log("Game is over");
                    gameplayScreen.SetActive(false );
                    DisplayResults();
                }

                break;

            case GameState.LevelUp:
                // Code for the leveling up state
                if (!isChoosingLevel)
                {
                    isChoosingLevel = true;
                    Time.timeScale = 0;
                    Debug.Log("Choosing level-up");
                    levelUpScreen.SetActive(true);
                }
                break;

            default:
                Debug.LogWarning("State does not exist");
                break;
        }
    }

    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        // If the canvas is not set, end the function so we don't generate any text
        if (!instance.damageTextCanvas) return;

        // Find relevant camera that we can use to convert the world position to screen position
        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(text, target, duration, speed));
    }

    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        // Start generating the floating text
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();

        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        if (textFont) tmPro.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        // Destroy after duration
        Destroy(textObj, duration);

        // Parent the generated text object to the canvas
        textObj.transform.SetParent(instance.damageTextCanvas.transform);

        // Pan the text upward and fade it away over time
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        while (t < duration)
        {
            // Wait for a frame and update the time
            yield return w;
            t += Time.deltaTime;

            // Fade the text to the right alpha value
            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            // Pan the text upward
            yOffset += speed * Time.deltaTime;
            if(rect && target) rect.position = referenceCamera.WorldToScreenPoint(target.position + new Vector3(0, yOffset));
        }
    }

    // Method to change the state of the game
    public void ChangeState (GameState newState)
    {
        currentState = newState;
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; // Stop the game
            pauseScreen.SetActive(true);
            Debug.Log("Game has been paused");
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f; // Resume the game
            pauseScreen.SetActive(false);
            Debug.Log("Game has been resumed");
        }
    }

    // Define the method that checks the pause or resume inputs
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState==GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }

        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    public void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssignChosenCharacterUI(CharacterScriptableObject chosenCharacterData) 
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterName.text = chosenCharacterData.Name;
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public string Stopwatch(float time)
    {
        float seconds = time % 60;
        float minutes = (time - seconds) / 60;

        float roundedSeconds = Mathf.RoundToInt(seconds);
        float roundedMinutes = Mathf.RoundToInt(minutes);

        string handledSeconds = roundedSeconds.ToString();
        string handledMinutes = roundedMinutes.ToString();

        if (roundedSeconds < 10)
        {
            handledSeconds = "0" + roundedSeconds.ToString();
        }

        if (roundedMinutes < 10)
        {
            handledMinutes = "0" + roundedMinutes.ToString();
        }


        return handledMinutes + ":" + handledSeconds;
    }

    public void AssignTimeSurvivedUI(float timeSurvivedData)
    {

        timeSurvivedDisplay.text = Stopwatch(timeSurvivedData);
    }

    public void AssignChosenInventoryUI(List<Image> chosenWeaponsData, List<Image> chosenPassiveItemsData)
    {
        if (chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassiveItemsData.Count != chosenPassiveItemsUI.Count)
        {
            Debug.Log("Chosen weapons and passive items data lists have different lenghts");
            return;
        }

        // Assign weapons UI
        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            if (chosenWeaponsData[i].sprite)
            {
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite;
            }
            else
            {
                chosenWeaponsUI[i].enabled = false;
            }
        }

        // Assign passive items UI
        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            if (chosenPassiveItemsData[i].sprite)
            {
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].sprite;
            }
            else
            {
                chosenPassiveItemsUI[i].enabled = false;
            }
        }
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    public void EndLevelUp()
    {
        isChoosingLevel = false;
        Time.timeScale = 1;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.GamePlay);
    }
}