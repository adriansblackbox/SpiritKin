using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatVFX : MonoBehaviour
{
    private List<Curse> activeCurses = new List<Curse>();
    public Image healthUp, healthDown, armorUp, armorDown, speedUp, speedDown, damageUp, damageDown;
    public Sprite notch, upArrow, downArrow;
    private void Start() {
    }

    public void removeBuffStat(string buff){
        Debug.Log("Tea Name: " + buff);
        switch (buff)
        {
            case "Armor":
                armorUp.sprite = notch;
                break;
            case "Health":
                healthUp.sprite = notch;
            break;
            case "Speed":
                speedUp.sprite = notch;
            break;
            case "Damage":
                damageUp.sprite = notch;
            break;
        }
    }
    public void addBuffStat(string buff){
        switch (buff)
        {
            case "Armor":
                armorUp.sprite = upArrow;
                break;
            case "Health":
                healthUp.sprite = upArrow;
            break;
            case "Speed":
                speedUp.sprite = upArrow;
            break;
            case "Damage":
                damageUp.sprite = upArrow;
            break;
        }
    }
    public void removeCurseStat(string curse){
        switch (curse)
        {
            case "Armor_Curse":
                armorDown.sprite = notch;
                break;
            case "Health_Curse":
                healthDown.sprite = notch;
            break;
            case "Slow_Curse":
                speedDown.sprite = notch;
            break;
            case "Damage_Curse":
                damageDown.sprite = notch;
            break;
        }
    }
    public void addCurseStat(string curse){
        switch (curse)
        {
            case "Armor_Curse":
                armorDown.sprite = downArrow;
                break;
            case "Health_Curse":
                healthDown.sprite = downArrow;
            break;
            case "Slow_Curse":
                speedDown.sprite = downArrow;
            break;
            case "Damage_Curse":
                damageDown.sprite = downArrow;
            break;
        }
    }
}
