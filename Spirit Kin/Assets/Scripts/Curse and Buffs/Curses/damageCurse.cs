using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Buff;

public class damageCurse : Curse
{
    private CharacterStats pStats;
    private CurseMeter cMeter;
    
    public damageCurse (Sprite _image, CharacterStats _pStats, CurseMeter _cMeter)
    {
        pStats = _pStats;
        type = "Damage_Curse";
        isApplied = false;
        removeFlag = false;
        image = _image;
        cMeter = _cMeter;
        penaltyValue = -5;
    }

    override public void invokeCurse () 
    {
        isApplied = true;
        cMeter.activeCurses.Add(this);

        pStats.damage.AddBaseValue(penaltyValue);
    }

    override public void updateCurse (float newValue) {
        pStats.damage.AddBaseValue(newValue - penaltyValue);
        penaltyValue = newValue;
    }

    override public void removeCurse () 
    {
        removeFlag = false;
        isApplied = false;
        active = false;
        pStats.damage.AddBaseValue(-penaltyValue);
    } 
}
