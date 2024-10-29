using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldPopupScript : MonoBehaviour
{
    public float lifeTime = 1f;
    public float floatSpeed = 0.5f;
    public GameManager gameManager;

    private TMP_Text goldText;

    private void Start()
    {
        goldText = GetComponent<TMP_Text>();
        gameManager = FindObjectOfType<GameManager>();
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        FloatUp();

        if (!gameManager.isGameRunning)
        {
            Destroy(gameObject);
        }
    }

    void FloatUp()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }
}
