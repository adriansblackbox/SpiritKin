using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBuff : Buff
{
    public SpeedBuff(Sprite sprite)
    {
        baseDuration = 60f;
        baseTimeActive = 0f;
        duration = baseDuration;
        timeActive = baseTimeActive;

        teaName = "Speed Buff";
        description = "Increases speed by 0.5";
        Cost = 1;
        InvestCost = 5;
        buffSprite = sprite;
        stat = statType.speed;
        basePower = 0.5f;
        isApplied = false;
        removeFlag = false;
    }
}
