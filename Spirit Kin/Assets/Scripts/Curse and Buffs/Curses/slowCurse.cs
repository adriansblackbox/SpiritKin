using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Buff;

public class slowCurse : Curse
{
    private CharacterStats pStats;
    private CurseMeter cMeter;
    private float pSpeed;
    
    public slowCurse (Sprite _image, CharacterStats _pStats, CurseMeter _cMeter)
    {
        pStats = _pStats;
        type = "Slow_Curse";
        isApplied = false;
        removeFlag = false;
        image = _image;
        cMeter = _cMeter;
        penaltyValue = -0.1f;
    }

    override public void invokeCurse () 
    {
        isApplied = true;
        cMeter.activeCurses.Add(this);
        pSpeed = pStats.speed.GetValue();

        pStats.speed.AddBaseValue(pSpeed * penaltyValue);
    }

    override public void updateCurse () {
        pStats.speed.AddBaseValue(pSpeed * -penaltyValue);
        pSpeed = pStats.speed.GetValue();
        pStats.speed.AddBaseValue(pSpeed * penaltyValue);
    }

    override public void updateCurse (float difficulty) {
        float newValue = difficulty * penaltyValue;
        pStats.speed.AddBaseValue(pSpeed * -penaltyValue);
        penaltyValue = newValue;
        if(pSpeed + (pSpeed * penaltyValue) < 0.15f) {
            penaltyValue = -0.85f; // Cap speed curse at 85%
        }
        pStats.speed.AddBaseValue(pSpeed * penaltyValue);
    }

    override public void removeCurse () 
    {
        removeFlag = false;
        isApplied = false;
        active = false;
        pStats.speed.AddBaseValue(pSpeed * -penaltyValue);
    } 
}
