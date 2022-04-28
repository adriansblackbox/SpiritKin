using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Buff;

public class armorCurse : Curse
{
    private CharacterStats pStats;
    private CurseMeter cMeter;
    
    public armorCurse (Sprite _image, CharacterStats _pStats, CurseMeter _cMeter)
    {
        pStats = _pStats;
        type = "Armor_Curse";
        isApplied = false;
        removeFlag = false;
        image = _image;
        cMeter = _cMeter;
        penaltyValue = -10.0f;
    }

    override public void invokeCurse () {
        isApplied = true;
        cMeter.activeCurses.Add(this);
        pStats.armor.AddBaseValue(penaltyValue);
    }

    override public void updateCurse (float newValue) {
        pStats.armor.AddBaseValue(newValue - penaltyValue);
        penaltyValue = newValue;
    }

    override public void removeCurse () {
        removeFlag = false;
        isApplied = false;
        active = false;
        pStats.armor.AddBaseValue(-penaltyValue);
    } 
}
