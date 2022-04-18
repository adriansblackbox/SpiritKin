using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBuff : Buff
{
    public DamageBuff(Sprite sprite)
    {
        baseDuration = 60f;
        baseTimeActive = 0f;
        duration = baseDuration;
        timeActive = baseTimeActive;

        teaName = "Damage Buff";
        description = "Increases damage by 10";
        Cost = 1;
        InvestCost = 5;
        buffSprite = sprite;
        stat = statType.damage;
        basePower = 10;
        isApplied = false;
        removeFlag = false;
    }
}
