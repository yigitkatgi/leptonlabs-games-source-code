using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ShopManager;

public class SkinButton : MonoBehaviour
{
    public string skinName;
    public TMP_Text buttonText;
    private ShopManager skinManager;
    private GameManager gm;

    void Start()
    {
        skinManager = FindObjectOfType<ShopManager>();
        gm = FindObjectOfType<GameManager>();
        UpdateButton();
    }

    public void OnButtonClick()
    {
        skinManager.UnlockSkin(skinName);
        UpdateButton();
    }

    void UpdateButton()
    {
        Skin skin = skinManager.ballSkins.Find(s => s.name == skinName) ?? skinManager.obstacleSkins.Find(s => s.name == skinName);
        if (skin != null)
        {
            buttonText.text = skin.isUnlocked ? "Unlocked" : $"Unlock: {skin.price}";
            GetComponent<Button>().interactable = !skin.isUnlocked && gm.gold >= skin.price;
        }
    }
}
