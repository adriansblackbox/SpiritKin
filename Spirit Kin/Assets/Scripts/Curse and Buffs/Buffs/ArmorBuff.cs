using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmorBuff : Buff
{
    public ArmorBuff(Sprite sprite)
    {
        baseDuration = 60f;
        baseTimeActive = 0f;
        baseLevel = 0;

        teaName = "Armor Buff";
        type = "armor";
        Cost = 25;
        baseInvestCost = 100;
        buffSprite = sprite;
        stat = statType.armor;
        basePower = 10;
        isApplied = false;
        removeFlag = false;

        power = 0;
        investCost = baseInvestCost;
        duration = baseDuration;
        timeActive = baseTimeActive;
        level = baseLevel;

        description = "Increases armor by " + basePower;

    }
}
