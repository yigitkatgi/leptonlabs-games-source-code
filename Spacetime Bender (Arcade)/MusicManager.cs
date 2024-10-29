using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource; // Reference to the AudioSource component
    public AudioClip musicClip; // Reference to the music clip
    public float rewindSpeed = 1.5f; // Speed at which the music rewinds
    public float timePitchFactor = 0.3f; // Coefficient to adjust pitch based on time scale
    public float minPitch = 0.6f; // Minimum pitch value
    public float maxPitch = 1.3f; // Maximum pitch value
    public float pitchLerpSpeed = 5f; // Speed at which the pitch interpolates
    public float dampingStrength = 0.5f; // Strength of the damping effect
    public Button soundOnOffButton;
    public Sprite soundOnSprite; // Sprite for sound on
    public Sprite soundOffSprite; // Sprite for sound off

    private bool isMusicOn = true;
    private string musicOnKey = "musicOnKey";
    private List<float> playbackPositions = new List<float>(); // List to track playback positions
    private bool isRewinding = false; // Flag to check if rewinding is active
    private int currentRewindIndex = 0;
    private float smoothedPitch; // Smoothed pitch value

    private void Start()
    {
        if (musicSource != null && musicClip != null)
        {
            // Check saved music state and update isMusicOn accordingly
            isMusicOn = PlayerPrefs.GetInt(musicOnKey, 1) == 1; // Default to on if not set

            // Set initial music state
            musicSource.clip = musicClip;
            musicSource.loop = true; // Set the music to loop
            if (isMusicOn)
            {
                musicSource.Play(); // Start playing the music if music is on
            }
            smoothedPitch = minPitch; // Initialize smoothed pitch

            // Set the correct sprite based on the initial state
            UpdateButtonSprite();
        }

        // Add listener to the button to toggle music when clicked
        soundOnOffButton.onClick.AddListener(ToggleMusicPlaying);
    }

    private void Update()
    {
        if (isMusicOn)
        {
            // Play music
            musicSource.enabled = true;
            if (!isRewinding && Time.timeScale > 0) // Record only when not paused
            {
                // Calculate the desired pitch with damping effect
                float dampenedTimeScale = DampingFunction(Time.timeScale);
                float desiredPitch = Mathf.Clamp(dampenedTimeScale * timePitchFactor, minPitch, maxPitch);

                // Apply exponential smoothing to the pitch
                smoothedPitch = Mathf.Lerp(smoothedPitch, desiredPitch, pitchLerpSpeed * Time.deltaTime);

                // Set the pitch
                musicSource.pitch = smoothedPitch;

                // Record the playback position for rewinding
                if (musicSource.isPlaying)
                {
                    playbackPositions.Add(musicSource.time);
                }
            }
        }
        else
        {
            // Dont play music
            musicSource.enabled = false;
        }
    }

    private float DampingFunction(float value)
    {
        // Apply damping to the value to reduce the impact of changes
        return Mathf.Log10(1 + dampingStrength * value) / Mathf.Log10(1 + dampingStrength);
    }

    public void StartRewind()
    {
        if (playbackPositions.Count > 0)
        {
            isRewinding = true;
            currentRewindIndex = playbackPositions.Count - 1;
            StartCoroutine(RewindMusic());
        }
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    private IEnumerator RewindMusic()
    {
        while (currentRewindIndex >= 0)
        {
            musicSource.time = playbackPositions[currentRewindIndex];
            currentRewindIndex--;

            // Simulate the rewinding effect
            yield return new WaitForSeconds(1f / rewindSpeed); // Control rewind speed
        }

        // Stop rewinding and reset state
        isRewinding = false;
        playbackPositions.Clear(); // Clear positions after rewinding

        // Optionally, restart the music after rewinding
        musicSource.time = 0;
        musicSource.Play();
    }

    public void ToggleMusicPlaying()
    {
        isMusicOn = !isMusicOn; // Toggle music state
        PlayerPrefs.SetInt(musicOnKey, isMusicOn ? 1 : 0); // Save the state
        PlayerPrefs.Save(); // Ensure the value is saved immediately

        if (isMusicOn)
        {
            // Enable the audio source before playing to avoid the warning
            musicSource.enabled = true;
            musicSource.Play();
        }
        else
        {
            // Disable the audio source to save resources
            musicSource.Pause();
            musicSource.enabled = false;
        }

        // Update the button sprite to reflect the current state
        UpdateButtonSprite();
    }

    private void UpdateButtonSprite()
    {
        if (isMusicOn)
        {
            soundOnOffButton.image.sprite = soundOnSprite; // Set to sound on sprite
        }
        else
        {
            soundOnOffButton.image.sprite = soundOffSprite; // Set to sound off sprite
        }
    }
}
