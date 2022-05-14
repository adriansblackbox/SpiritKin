using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBuff : Buff
{
    public HealthBuff(Sprite sprite)
    {
        baseDuration = 60f;
        baseTimeActive = 0f;      
        baseLevel = 0;

        teaName = "Health Buff";
        type = "health";
        Cost = 25;
        baseInvestCost = 100;
        buffSprite = sprite;
        stat = statType.health;
        basePower = 50;
        isApplied = false;
        removeFlag = false;

        duration = baseDuration;
        timeActive = baseTimeActive;
        power = 0;
        investCost = baseInvestCost;
        level = baseLevel;

        description = "Increases health by " + basePower;  
    }
}
