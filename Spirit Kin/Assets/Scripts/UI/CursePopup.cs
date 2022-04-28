using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursePopup : MonoBehaviour
{
    public GameObject curseImage;
    public GameObject curseText;
    public GameObject background;

    public Sprite speedCurseImage;
    public Sprite armorCurseImage;
    public Sprite damageCurseImage;

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

    public void showCursePopup(string curseName)
    {
        showing = true;
        background.SetActive(true);

        switch (curseName)
        {
            case "Speed":
                curseText.GetComponent<Text>().text = "Speed Curse";
                curseImage.GetComponent<Image>().sprite = speedCurseImage;
                break;
            case "Defense":
                curseText.GetComponent<Text>().text = "Defense Curse";
                curseImage.GetComponent<Image>().sprite = armorCurseImage;
                break;
            case "Damage":
                curseText.GetComponent<Text>().text = "Damage Curse";
                curseImage.GetComponent<Image>().sprite = damageCurseImage;
                break;
        }

    }

    public void hideCursePopup()
    {
        showing = false;
        background.SetActive(false);
    }


}
