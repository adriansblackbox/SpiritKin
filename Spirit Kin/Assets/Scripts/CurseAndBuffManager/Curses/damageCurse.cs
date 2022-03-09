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
    }

    override public void invokeCurse () 
    {
        Debug.Log(type + " Added!");
        isApplied = true;
        cMeter.activeCurses.Add(this);

        pStats.damage.AddBaseValue(-10);
    } 

    override public void removeCurse () 
    {
        removeFlag = false;
        isApplied = false;
        active = false;
        pStats.damage.AddBaseValue(10);
    } 
}
