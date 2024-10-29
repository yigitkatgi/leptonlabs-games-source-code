using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyCollisionScript : MonoBehaviour
{
    public ParticleSystem explosion;
    public ParticleSystem toolsExp;
    public ParticleSystem blueExplosion;
    public int goldAmount = 5;
    public GameObject goldPopUp;
    
    private Canvas canvas;
    CameraScript cameraScript;
    GameManager gameManager;
    private bool hasCollided = false;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        cameraScript = FindObjectOfType<CameraScript>();
        canvas = FindObjectOfType<Canvas>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Determine which object has the lower instance ID
            if (gameObject.GetInstanceID() < collision.gameObject.GetInstanceID())
            {
                // Play the particle effects at the collision point
                Instantiate(explosion, transform.position, explosion.transform.rotation);
                Instantiate(toolsExp, transform.position, toolsExp.transform.rotation);

                // Vibrate the phone
                Handheld.Vibrate();

                //Shake the camera
                cameraScript.TriggerShake(.5f,.3f);

                // Add gold
                gameManager.AddGold(goldAmount);
                
                // Gold popup
                var goldPopup = Instantiate(goldPopUp, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, canvas.transform);
                goldPopup.GetComponent<TMP_Text>().text = goldAmount.ToString();
            }

            // Destroy both game objects regardless of which one triggered the effect
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!hasCollided)
        {
            hasCollided = true;

            // Play the blue explosion effect
            Instantiate(blueExplosion, transform.position, blueExplosion.transform.rotation);

            // Add gold
            gameManager.AddGold(goldAmount);

            // Gold popup
            var goldPopup = Instantiate(goldPopUp, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, canvas.transform);
            goldPopup.GetComponent<TMP_Text>().text = goldAmount.ToString();

            // Vibrate the phone
            Handheld.Vibrate();

            //Shake the camera
            cameraScript.TriggerShake(.5f, .3f);

            // Destroy the game object
            Destroy(gameObject);
        }
    }
}
