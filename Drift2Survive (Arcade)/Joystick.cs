using UnityEngine;
using UnityEngine.UI;

public class Joystick : MonoBehaviour
{
    public Image outerCircle;
    public Image innerRing;

    private Vector2 inputVector;
    private Vector2 touchStartPos;
    private GameManager gameManager;

    void Start()
    {
        outerCircle.gameObject.SetActive(false);
        innerRing.gameObject.SetActive(false);

        // Find the GameManager instance
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager script not found!");
        }
    }

    void Update()
    {
        if (gameManager != null && gameManager.isGameRunning && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                outerCircle.transform.position = touchStartPos;
                innerRing.transform.position = touchStartPos;
                outerCircle.gameObject.SetActive(true);
                innerRing.gameObject.SetActive(true);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                Vector2 touchPosition = touch.position;
                Vector2 direction = touchPosition - touchStartPos;
                inputVector = direction.magnitude > outerCircle.rectTransform.sizeDelta.x / 2
                    ? direction.normalized
                    : direction / (outerCircle.rectTransform.sizeDelta.x / 2);

                innerRing.rectTransform.position = new Vector2(
                    touchStartPos.x + inputVector.x * (outerCircle.rectTransform.sizeDelta.x / 2),
                    touchStartPos.y + inputVector.y * (outerCircle.rectTransform.sizeDelta.y / 2)
                );
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                ResetJoystick();
            }
        }
    }

    public float Horizontal()
    {
        return inputVector.x;
    }

    public float Vertical()
    {
        return inputVector.y;
    }

    public void ResetJoystick()
    {
        inputVector = Vector2.zero;
        outerCircle.gameObject.SetActive(false);
        innerRing.gameObject.SetActive(false);
    }
}
