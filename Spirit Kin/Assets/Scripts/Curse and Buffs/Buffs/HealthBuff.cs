using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBuff : Buff
{
    public HealthBuff(Sprite sprite)
    {
        baseDuration = 60f;
        baseTimeActive = 0f;
        duration = baseDuration;
        timeActive = baseTimeActive;

        teaName = "Health Buff";
        description = "Increases health by 10";
        Cost = 1;
        InvestCost = 5;
        buffSprite = sprite;
        stat = statType.health;
        basePower = 10;
        isApplied = false;
        removeFlag = false;
    }
}
