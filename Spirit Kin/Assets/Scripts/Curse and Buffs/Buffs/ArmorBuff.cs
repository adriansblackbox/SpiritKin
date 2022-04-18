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
        duration = baseDuration;
        timeActive = baseTimeActive;

        teaName = "Armor Buff";
        description = "Increases armor by 10";
        Cost = 1;
        InvestCost = 5;
        buffSprite = sprite;
        stat = statType.armor;
        basePower = 10;
        isApplied = false;
        removeFlag = false;
    }
}
