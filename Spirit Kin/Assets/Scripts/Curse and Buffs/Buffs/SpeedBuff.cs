using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBuff : Buff
{
    public SpeedBuff(Sprite sprite)
    {
        baseDuration = 60f;
        baseTimeActive = 0f;
        baseLevel = 0;

        teaName = "Speed Buff";
        type = "speed";
        Cost = 25;
        baseInvestCost = 100;
        buffSprite = sprite;
        stat = statType.speed;
        basePower = 0.15f;
        isApplied = false;
        removeFlag = false;

        power = basePower;
        investCost = baseInvestCost;
        duration = baseDuration;
        timeActive = baseTimeActive;
        level = baseLevel;

        description = "Increases speed by " + basePower;
    }
}
