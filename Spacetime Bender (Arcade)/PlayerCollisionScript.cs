using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCollisionScript : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject confettiParticlesPrefab;
    public AudioClip successSound;
    public AudioClip failSound;
    public AudioClip goldSound;
    public float confettiDestroyAfterTime = 0.3f;
    public float particleSpeed;
    public List<GameObject> golds = new List<GameObject>();
    public GameObject continuePanel;
    public bool isDead = false;
    public GameObject tutorialPanel;
    public float tutorialInputDelay = 1f;
    public float adShowProbability = 0.33f;
    public Button sfxOnOffButton;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    private GoldSpawnerScript goldSpawner;
    private RewindManager rewindManager;
    private AudioSource audioSource;
    private GameObject currentObstacle;
    private PlayerController playerController;
    private float previousTimeScale;
    private int deathIndex;
    private MusicManager musicManager;
    private bool showingTutorial = false;
    private bool canSkipTutorial = false;
    private bool isSFXOn = true;
    private string sfxOnKey = "sfxOnKey";
    private LoadInterstitial interAd;
    private bool adShownLastTime = false; // Flag to check if an ad was shown last time

    void Start()
    {
        goldSpawner = FindObjectOfType<GoldSpawnerScript>();
        rewindManager = GetComponent<RewindManager>();
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerController>();
        musicManager = FindObjectOfType<MusicManager>();
        interAd = FindObjectOfType<LoadInterstitial>();

        if (interAd != null)
        {
            interAd.OnAdClosed = StartRewindAfterAd;
        }

        deathIndex = 0;
        isDead = false;

        // Load SFX state
        isSFXOn = PlayerPrefs.GetInt(sfxOnKey, 1) == 1;

        // Load whether an ad was shown last time
        adShownLastTime = PlayerPrefs.GetInt("AdShownLastTime", 0) == 1;

        if (PlayerPrefs.GetInt("FirstGame", 0) == 0)
        {
            showingTutorial = true;
            previousTimeScale = Time.timeScale;
            playerController.enabled = false;
            Time.timeScale = 0;
            tutorialPanel.SetActive(true);
            StartCoroutine(AllowTutorialSkipAfterDelay());
        }

        sfxOnOffButton.onClick.AddListener(ToggleSFX);
        UpdateSFXButtonSprite();
    }

    private IEnumerator AllowTutorialSkipAfterDelay()
    {
        yield return new WaitForSecondsRealtime(tutorialInputDelay);
        canSkipTutorial = true;
    }

    private void Update()
    {
        if (showingTutorial && canSkipTutorial)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                tutorialPanel.SetActive(false);
                Time.timeScale = previousTimeScale;
                playerController.enabled = true;
                showingTutorial = false;
            }
        }
        UpdateSFXSprite();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            isDead = true;
            PlayerPrefs.SetInt("FirstGame", 1);
            PlayDeathFeedback();
            deathIndex++;
            currentObstacle = collision.gameObject;

            if (deathIndex == 1)
            {
                previousTimeScale = Time.timeScale;
                playerController.enabled = false;
                Time.timeScale = 0f;
                musicManager.PauseMusic();
                continuePanel.SetActive(true);
            }
            else
            {
                HandleDeath();
                musicManager.PauseMusic();
            }
        }
        else if (collision.CompareTag("Score"))
        {
            HandleScore(collision.transform.position);
            collision.enabled = false;
        }
        else if (collision.CompareTag("Gold1"))
        {
            gameManager.AddGold(1);
            Destroy(collision.gameObject);
            RemoveFromGoldList(collision.gameObject);
            PlaySFX(goldSound);
        }
        else if (collision.CompareTag("Gold2"))
        {
            gameManager.AddGold(2);
            Destroy(collision.gameObject);
            RemoveFromGoldList(collision.gameObject);
            PlaySFX(goldSound);
        }
    }

    private void HandleScore(Vector2 position)
    {
        gameManager.IncrementScore();
        PlaySuccessFeedback(position);
    }

    private void HandleDeath()
    {
        goldSpawner.enabled = false;

        foreach (GameObject gold in golds)
        {
            Destroy(gold);
        }

        rewindManager.StartRewind();
        deathIndex = 0;
    }

    private void PlaySuccessFeedback(Vector2 position)
    {
        if (confettiParticlesPrefab != null)
        {
            var confetti = Instantiate(confettiParticlesPrefab, position, Quaternion.identity);
            Rigidbody2D confettiRb = confetti.GetComponent<Rigidbody2D>();
            if (confettiRb != null)
            {
                confettiRb.velocity = Vector2.down * particleSpeed;
            }
            Destroy(confetti, confettiDestroyAfterTime);
        }

        PlaySFX(successSound);
        Handheld.Vibrate();
    }

    private void PlayDeathFeedback()
    {
        PlaySFX(failSound);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (isSFXOn && clip != null)
        {
            audioSource.PlayOneShot(clip, 0.7f);
        }
    }

    private void ToggleSFX()
    {
        isSFXOn = !isSFXOn;
        PlayerPrefs.SetInt(sfxOnKey, isSFXOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateSFXButtonSprite();
    }

    private void UpdateSFXButtonSprite()
    {
        sfxOnOffButton.GetComponent<Image>().sprite = isSFXOn ? sfxOnSprite : sfxOffSprite;
    }

    public void AddToGoldList(GameObject gold)
    {
        golds.Add(gold);
    }

    public void RemoveFromGoldList(GameObject gold)
    {
        golds.Remove(gold);
    }

    public void OnAdWatched()
    {
        Destroy(currentObstacle);
        playerController.enabled = true;
        Time.timeScale = previousTimeScale;
        musicManager.ResumeMusic();
        continuePanel.SetActive(false);
    }

    public void OnCloseButtonClicked()
    {
        // Check if an ad was shown last time; if so, skip this round
        if (adShownLastTime)
        {
            Debug.Log("Skipping ad due to last ad being shown.");
            StartRewindAfterAd();
            adShownLastTime = false; // Reset the flag for the next session
            PlayerPrefs.SetInt("AdShownLastTime", 0);
            PlayerPrefs.Save();
            return;
        }

        System.Random random = new System.Random();
        float randomValue = (float)random.NextDouble();

        Debug.Log($"Random value generated: {randomValue}, Probability threshold: {adShowProbability}");

        // Check probability for showing the ad
        if (randomValue < adShowProbability)
        {
            Debug.Log("Showing the ad...");
            if (interAd != null)
            {
                interAd.LoadAd();
            }
            adShownLastTime = true; // Mark that an ad was shown
            PlayerPrefs.SetInt("AdShownLastTime", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Skipping the ad, proceeding to rewind immediately.");
            StartRewindAfterAd();
            adShownLastTime = false;
            PlayerPrefs.SetInt("AdShownLastTime", 0);
            PlayerPrefs.Save();
        }
    }

    private void StartRewindAfterAd()
    {
        continuePanel.SetActive(false);
        Time.timeScale = previousTimeScale;
        HandleDeath();
    }

    private void OnDestroy()
    {
        // Clean up or reset any required state
    }

    public void UpdateSFXSprite()
    {
        if (isSFXOn) sfxOnOffButton.image.sprite = sfxOnSprite;
        else sfxOnOffButton.image.sprite = sfxOffSprite;
    }
}
