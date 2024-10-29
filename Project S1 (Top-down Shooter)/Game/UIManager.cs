using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image weaponImage;  // Reference to the UI Image component
    private Sprite currentWeaponSprite;  // Current weapon sprite

    void Start()
    {
        if (weaponImage == null)
        {
            Debug.LogError("Weapon Image is not assigned in the inspector.");
        }
    }

    public void UpdateWeaponUI(Sprite newWeaponSprite)
    {
        currentWeaponSprite = newWeaponSprite;
        weaponImage.sprite = currentWeaponSprite;
    }
}
