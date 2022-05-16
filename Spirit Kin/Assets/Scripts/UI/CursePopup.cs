using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CursePopup : MonoBehaviour
{
    public GameObject curseImage;
    public GameObject curseText;
    public GameObject background;

    public Sprite speedCurseImage;
    public Sprite armorCurseImage;
    public Sprite damageCurseImage;
    public Sprite blindCurseImage;
    public Sprite moneyCurseImage;
    public Sprite invertCurseImage;
    public Sprite rangeBlessingImage;
    public Sprite teaBlessingImage;
    public Sprite vampBlessingImage;

    public float popupDuration = 5f;

    private float popupTimer;
    private bool showing;

    void FixedUpdate()
    {
        if (showing)
        {
            popupTimer += Time.deltaTime;
        }
    }

    void Update()
    {
        if (showing && popupTimer > popupDuration)
        {
            hideCursePopup();
        }
    }

    public void showCursePopup(string curseType)
    {
        showing = true;
        background.SetActive(true);

        switch (curseType)
        {
            case "Slow_Curse":
                curseText.GetComponent<TextMeshProUGUI>().text = "Slow Curse";
                curseImage.GetComponent<Image>().sprite = speedCurseImage;
                break;
            case "Armor_Curse":
                curseText.GetComponent<TextMeshProUGUI>().text = "Frail Curse";
                curseImage.GetComponent<Image>().sprite = armorCurseImage;
                break;
            case "Damage Curse":
                curseText.GetComponent<TextMeshProUGUI>().text = "Damage Curse";
                curseImage.GetComponent<Image>().sprite = damageCurseImage;
                break;
            case "Blind_Curse":
                curseText.GetComponent<TextMeshProUGUI>().text = "Blind Curse";
                curseImage.GetComponent<Image>().sprite = blindCurseImage;
                break;
            case "invert_Curse":
                curseText.GetComponent<TextMeshProUGUI>().text = "Invert Curse";
                curseImage.GetComponent<Image>().sprite = invertCurseImage;
                break;
            case "Money Drought":
                curseText.GetComponent<TextMeshProUGUI>().text = "Money Drought";
                curseImage.GetComponent<Image>().sprite = moneyCurseImage;
                break;
            case "Range Blessing":
                curseText.GetComponent<TextMeshProUGUI>().text = "Range Blessing";
                curseImage.GetComponent<Image>().sprite = rangeBlessingImage;
                break;
            case "Tea Blessing":
                curseText.GetComponent<TextMeshProUGUI>().text = "Tea Blessing";
                curseImage.GetComponent<Image>().sprite = teaBlessingImage;
                break;
            case "Vamp Blessing":
                curseText.GetComponent<TextMeshProUGUI>().text = "Vamp Blessing";
                curseImage.GetComponent<Image>().sprite = vampBlessingImage;
                break;
        }
    }

    public void hideCursePopup()
    {
        showing = false;
        background.SetActive(false);
        popupTimer = 0;
    }


}
