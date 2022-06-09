using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBuff : Buff
{
    public DamageBuff(Sprite sprite)
    {
        baseDuration = 60f;
        baseTimeActive = 0f;
        baseLevel = 0;

        teaName = "Damage Tea";
        type = "damage";
        Cost = 25;
        baseInvestCost = 100;
        buffSprite = sprite;
        stat = statType.damage;
        basePower = 10;
        isApplied = false;
        removeFlag = false;

        duration = baseDuration;
        timeActive = baseTimeActive;
        power = basePower;
        investCost = baseInvestCost;
        level = baseLevel;

        description = "Increases damage by " + basePower + "\n Lasts " + duration + "s";
    }
}
