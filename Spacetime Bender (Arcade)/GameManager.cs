using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Sprite> playerSprites = new List<Sprite>();
    public List<GameObject> obstaclePrefabs = new List<GameObject>();
    public int score;
    public int highScore;
    public int gold;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text goldText;
    public GameObject pauseScreen;
    public GameObject adPanel;
    public Camera mainCamera;
    public float colorTransitionDuration = 1f;
    public SpawnerScript spawnerScript;
    public Button pauseButton;

    private Color[] colors = new Color[]
    {
        new Color32(51, 204, 186, 255),
        new Color32(0, 119, 221, 255),
        new Color32(255, 119, 33, 255),
        new Color32(0, 187, 220, 255),
        new Color32(255, 103, 154, 255)
    };
    private int currentColorIndex = 0;
    private bool isTransitioning = false;
    private float previousTimeScale;
    private GameObject player;
    private PlayerController playerController;
    private PlayerCollisionScript playerCollisionScript;
    void Start()
    {
        Debug.Log("GameManager Start: Initializing game state.");

        pauseScreen.SetActive(false);
        SelectCharacter(PlayerPrefs.GetInt("Sprite"));
        SelectObstacle(PlayerPrefs.GetInt("Prefab"));
        scoreText.text = score.ToString();
        highScore = PlayerPrefs.GetInt("HighScore");
        highScoreText.text = highScore.ToString();

        // Load gold from PlayerPrefs
        gold = PlayerPrefs.GetInt("Gold", 0);
        goldText.text = gold.ToString();
        Debug.Log("Loaded gold at start: " + gold);

        mainCamera.backgroundColor = colors[currentColorIndex]; // Initialize background color
        playerController = FindObjectOfType<PlayerController>();
        playerCollisionScript = FindObjectOfType<PlayerCollisionScript>();
    }

    public bool IncrementScore()
    {
        score++;
        scoreText.text = score.ToString();

        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = highScore.ToString();
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save(); // Save immediately
            Debug.Log("New high score: " + highScore);
        }

        if (score % 3 == 0)
        {
            ChangeBackgroundColor();
        }

        return true;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log("Adding gold: " + amount + ", New gold amount: " + gold);
        goldText.text = gold.ToString();
        PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.Save(); // Ensure the data is saved immediately
        Debug.Log("Gold after update and save: " + gold);
    }

    private void ChangeBackgroundColor()
    {
        if (isTransitioning) return; // Prevent multiple transitions at the same time
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        StartCoroutine(TransitionToColor(colors[currentColorIndex]));
    }

    private IEnumerator TransitionToColor(Color targetColor)
    {
        isTransitioning = true;
        Color startColor = mainCamera.backgroundColor;
        float elapsed = 0f;

        while (elapsed < colorTransitionDuration)
        {
            mainCamera.backgroundColor = Color.Lerp(startColor, targetColor, elapsed / colorTransitionDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.backgroundColor = targetColor; // Ensure it reaches the exact target color
        isTransitioning = false;
    }

    public void PauseGame()
    {
        previousTimeScale = Time.timeScale;
        playerController.enabled = false;
        Time.timeScale = 0f;
        adPanel.SetActive(false);
        pauseButton.gameObject.SetActive(false);
        highScoreText.enabled = false;
        pauseScreen.SetActive(true);
    }

    public void ResumeGame()
    {
        if (playerCollisionScript.isDead) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
        {
            pauseScreen.SetActive(false);
            pauseButton.gameObject.SetActive(true);
            highScoreText.enabled = true;
            playerController.enabled = true;
            Time.timeScale = previousTimeScale;
        }
    }

    public void SelectCharacter(int index)
    {
        PlayerPrefs.SetInt("Sprite", index);
        PlayerPrefs.Save(); // Ensure immediate save
        player = GameObject.FindWithTag("Player");
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        sr.sprite = playerSprites[index];
    }

    public void SelectObstacle(int index)
    {
        PlayerPrefs.SetInt("Prefab", index);
        PlayerPrefs.Save(); // Ensure immediate save
        spawnerScript.AssignObstaclePrefab(obstaclePrefabs[index]);
    }
}
