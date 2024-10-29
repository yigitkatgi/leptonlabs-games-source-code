using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class WeaponBase : MonoBehaviour
{
    public GameObject joystickPrefab;  // Prefab for the joystick
    public float maxJoystickDistance = 100f;  // Maximum distance the inner part can move
    public GameObject aimLinePrefab;
    public GameObject bulletPrefab;
    public float cooldown = 1f;
    public bool readyToShoot;
    public Sprite weaponSprite;
    public Color readyColor = Color.white;

    protected bool isPlayer;
    protected GameObject joystickInstance;
    protected RectTransform joystickBackground;
    protected RectTransform joystickHandle;
    protected Vector2 startPos;
    protected Canvas canvas;
    public float aimAngle;
    protected GameObject aimLine;
    protected float timer;
    protected Quaternion originalRotation;
    protected Color originalColor;
    protected bool isCooldown = false;
    protected TimeManager timeManager;
    protected UIManager weaponUIManager;
    protected Image weaponUIImage;
    protected Vector2 direction;
    protected Vector3 worldDirection; // Store world space direction

    private Camera mainCamera; // Reference to the main camera

    protected virtual void Start()
    {
        if (GetComponentInParent<PlayerMovementScript>() != null)
        {
            isPlayer = true;
        }
        else
        {
            isPlayer = false;
        }

        if (isPlayer)
        {
            canvas = FindObjectOfType<Canvas>();
            timeManager = FindObjectOfType<TimeManager>();
            weaponUIManager = FindObjectOfType<UIManager>();
            timer = 0;
            readyToShoot = false;
            GameObject weaponUIImageGameObject = GameObject.FindWithTag("WeaponUI");
            weaponUIImage = weaponUIImageGameObject.GetComponent<Image>();

            if (weaponUIImage != null)
            {
                originalRotation = weaponUIImage.rectTransform.rotation;
                originalColor = weaponUIImage.color;
            }

            if (weaponUIManager != null && weaponSprite != null)
            {
                weaponUIManager.UpdateWeaponUI(weaponSprite);
            }

            MakeReadyToShoot();

            // Get the main camera
            mainCamera = Camera.main;
        }
    }

    protected virtual void Update()
    {
        if (isPlayer)
        {
            timer += Time.deltaTime;

            if (timer >= cooldown)
            {
                readyToShoot = true;

                if (isCooldown)
                {
                    StopCooldownEffects();
                }
            }
            else
            {
                readyToShoot = false;

                if (!isCooldown)
                {
                    StartCooldownEffects();
                }

                RotateWeaponUIImageDuringCooldown();
            }

            HandleTouch();
        }
    }

    private void StartCooldownEffects()
    {
        isCooldown = true;
    }

    private void StopCooldownEffects()
    {
        isCooldown = false;
        if (weaponUIImage != null)
        {
            weaponUIImage.rectTransform.rotation = originalRotation;
            StartCoroutine(FlashWeaponUIImageColor());
        }
    }

    private void RotateWeaponUIImageDuringCooldown()
    {
        if (weaponUIImage != null)
        {
            float rotationSpeed = 360f / cooldown;
            weaponUIImage.rectTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator FlashWeaponUIImageColor()
    {
        if (weaponUIImage != null)
        {
            weaponUIImage.color = readyColor;
            yield return new WaitForSeconds(0.1f);
            weaponUIImage.color = originalColor;
        }
    }

    protected void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.position.x > Screen.width / 2)
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            startPos = touch.position;
                            joystickInstance = Instantiate(joystickPrefab, canvas.transform);
                            joystickInstance.transform.position = startPos;
                            aimLine = Instantiate(aimLinePrefab, transform.position, Quaternion.identity);
                            aimLine.transform.SetParent(transform);

                            joystickBackground = joystickInstance.transform.GetChild(0).GetComponent<RectTransform>();
                            joystickHandle = joystickInstance.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
                            break;

                        case TouchPhase.Moved:
                            if (joystickInstance)
                            {
                                Vector2 currentPos = touch.position;
                                direction = currentPos - startPos;

                                // Convert screen direction to world space
                                Vector3 worldStartPos = mainCamera.ScreenToWorldPoint(new Vector3(startPos.x, startPos.y, mainCamera.nearClipPlane));
                                Vector3 worldCurrentPos = mainCamera.ScreenToWorldPoint(new Vector3(currentPos.x, currentPos.y, mainCamera.nearClipPlane));
                                worldDirection = (worldCurrentPos - worldStartPos).normalized;

                                // Calculate aim angle in world space
                                aimAngle = Mathf.Atan2(worldDirection.y, worldDirection.x) * Mathf.Rad2Deg;

                                float distance = Mathf.Min(direction.magnitude, maxJoystickDistance);
                                Vector2 limitedDirection = direction.normalized * distance;

                                joystickHandle.anchoredPosition = limitedDirection;
                                aimLine.transform.rotation = Quaternion.Euler(0, 0, aimAngle - 90);
                            }
                            break;

                        case TouchPhase.Ended:
                            if (readyToShoot) Shoot();
                            Destroy(joystickInstance);
                            Destroy(aimLine);
                            break;

                        case TouchPhase.Canceled:
                            Destroy(joystickInstance);
                            Destroy(aimLine);
                            direction = Vector2.zero;
                            break;
                    }
                }
            }
        }
    }

    // New method to check if there is aiming input
    public bool HasAimInput()
    {
        // Return true if there's touch input on the aiming side of the screen
        return Input.touchCount > 0 && joystickInstance != null;
    }

    public void MakeReadyToShoot()
    {
        timer = cooldown;
        readyToShoot = true;
        StopCooldownEffects();
    }

    // Abstract method to be implemented by derived classes
    protected abstract void Shoot();
}
