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
    
    public slowCurse (Sprite _image, CharacterStats _pStats, CurseMeter _cMeter)
    {
        pStats = _pStats;
        type = "Slow_Curse";
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

        pStats.speed.AddBaseValue(-0.25f);
    } 

    override public void removeCurse () 
    {
        removeFlag = false;
        isApplied = false;
        active = false;
        pStats.speed.AddBaseValue(0.25f);
    } 
}
