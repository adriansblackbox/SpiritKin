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
    }

    override public void invokeCurse () 
    {
        Debug.Log(type + " Added!");
        isApplied = true;
        cMeter.activeCurses.Add(this);
        pSpeed = pStats.speed.GetValue();

        pStats.speed.AddBaseValue(pSpeed * -0.5f);
    } 

    override public void removeCurse () 
    {
        removeFlag = false;
        isApplied = false;
        active = false;
        pStats.speed.AddBaseValue(pSpeed * 0.5f);
    } 
}
