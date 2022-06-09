using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment
{
    public string equipName;
    public string description;
    public Sprite equipSprite;
    public int Cost = 75;

    public int baseInvestCost = 100;
    public int investCost;

    public bool isEquipped;

    public int baseLevel = 0;
    public int level;

    public float baseDuration = 90f;
    public float duration;

    public float baseTimeActive = 0f;
    public float timeActive;

    private void OnEnable()
    {
        isEquipped = false;
        duration = baseDuration;
        investCost = baseInvestCost;
        timeActive = baseTimeActive;
        level = baseLevel;
    }

    public void updateDescription()
    {
        description = "Imbue weapon with Gravity Pull\nLasts " + duration + "s";
    }

}


