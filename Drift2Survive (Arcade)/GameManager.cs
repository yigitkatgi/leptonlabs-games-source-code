using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject gameOverScreen;
    public GameObject carSelectionScreen;
    public Joystick joystick;
    public TMP_Text goldText; // Text to display the player's gold
    public TMP_Text scoreText; // Text to display the current score
    public bool isGameRunning = false;
    public PlayerMovementScript pm;
    public GameObject gameRunningScreen;

    [Header("Summary stats")]
    public TMP_Text gameOverScoreText; // Text to display the score on the game over screen
    public TMP_Text highScoreText; // Text to display the high score on the game over screen
    public TMP_Text aliveDurationText;
    public TMP_Text driftDurationText;

    [HideInInspector]
    public int gold = 0; // Player's gold
    [HideInInspector]
    public int score = 0; // Current score
    private int highScore = 0; // High score
    private float accumulatedTime = 0f; // Accumulated time for score increment
    private float aliveDuration;
    

    void Start()
    {
        ShowStartScreen();
        gameRunningScreen.SetActive(false);
        // Freeze time at the start of the game
        Time.timeScale = 0;
        LoadPlayerPrefs();
        UpdateGoldText();
        UpdateScoreText();
        aliveDuration = 0f;
    }

    void LoadPlayerPrefs()
    {
        // Load the player's gold and high score from PlayerPrefs
        gold = PlayerPrefs.GetInt("Gold", 0);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void SavePlayerPrefs()
    {
        // Save the player's gold and high score to PlayerPrefs
        PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }

    public void ShowStartScreen()
    {
        startScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        carSelectionScreen.SetActive(false);
        isGameRunning = false;
    }

    public void ShowGameOverScreen()
    {
        startScreen.SetActive(false);
        gameRunningScreen.SetActive(false);
        gameOverScreen.SetActive(true);
        carSelectionScreen.SetActive(false);
        // Freeze time when showing the game over screen
        Time.timeScale = 0;
        isGameRunning = false;
        joystick.gameObject.SetActive(false);

        // Update the high score if the current score is higher
        if (score > highScore)
        {
            highScore = score;
        }

        // Save the player's gold and high score
        SavePlayerPrefs();

        // Display the score and high score on the game over screen
        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = "Score: " + score;
        }
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
        if (aliveDurationText != null)
        {
            int minutes = Mathf.FloorToInt(aliveDuration / 60);
            int seconds = Mathf.FloorToInt(aliveDuration % 60);
            aliveDurationText.text = "Survived for " + minutes.ToString("00") + ":" + seconds.ToString("00");
        }

        if (driftDurationText != null)
        {
            int driftMinutes = Mathf.FloorToInt(pm.driftDuration / 60);
            int driftSeconds = Mathf.FloorToInt(pm.driftDuration % 60);
            driftDurationText.text = "Drifted for " + driftMinutes.ToString("00") + ":" + driftSeconds.ToString("00");
        }

    }

    public void ShowCarSelectionScreen()
    {
        startScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        carSelectionScreen.SetActive(true);
        isGameRunning = false;
    }

    public void StartGame()
    {
        // Logic to start the game
        startScreen.SetActive(false);

        // Enable in game UI
        gameRunningScreen.SetActive(true);

        // Unfreeze time to start the game
        Time.timeScale = 1;
        isGameRunning = true;
        score = 0; // Reset score
        accumulatedTime = 0f; // Reset accumulated time
        UpdateGoldText();
        UpdateScoreText();
    }

    public void RetryGame()
    {
        // Unfreeze time before reloading the scene
        Time.timeScale = 1;
        // Reload the current scene to restart the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToPreviousScreen()
    {
        // Logic to go back to the previous screen
        if (carSelectionScreen.activeSelf)
        {
            // If currently in car selection screen, go back to game over screen
            ShowGameOverScreen();
        }
        else if (gameOverScreen.activeSelf)
        {
            // If currently in game over screen, reload the scene to reset everything
            RetryGame();
        }
        else if (startScreen.activeSelf)
        {
            // If currently in start screen, reset game state
            ShowStartScreen();
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
    }

    void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = gold.ToString();
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    void Update()
    {
        if (isGameRunning)
        {
            aliveDuration += Time.deltaTime;
            accumulatedTime += Time.deltaTime;
            if (accumulatedTime >= 1f)
            {
                score += Mathf.FloorToInt(accumulatedTime);
                accumulatedTime = accumulatedTime % 1f;
                UpdateScoreText();
            }
            scoreText.gameObject.SetActive(true);
        }
        else
        {
            scoreText.gameObject.SetActive(false);
        }
    }
}
