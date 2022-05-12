using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moneyCurse : Curse
{
    private PlayerStats pStats;
    private CurseMeter cMeter;
    
    public moneyCurse (Sprite _image, PlayerStats _pStats, CurseMeter _cMeter)
    {
        pStats = _pStats;
        type = "Money Drought";
        isApplied = false;
        removeFlag = false;
        image = _image;
        cMeter = _cMeter;
        penaltyValue = 0.1f;
    }

    override public void invokeCurse () 
    {
        isApplied = true;
        cMeter.activeCurses.Add(this);

        pStats.noCoindens = true;
        pStats.moneyCurseLock = pStats.coins;
    }

    override public void updateCurse (float newValue) {
        penaltyValue = newValue;
    }

    public float GetValue () {
        return penaltyValue;
    }

    override public void removeCurse () 
    {
        removeFlag = false;
        isApplied = false;
        active = false;
        pStats.noCoindens = false;
    } 
}
