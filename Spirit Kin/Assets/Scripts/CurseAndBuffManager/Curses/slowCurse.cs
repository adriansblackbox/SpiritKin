using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Buff;

public class slowCurse : Curse
{
    private CharacterStats pStats;
    
    public slowCurse (Sprite _image, CharacterStats _pStats)
    {
        pStats = _pStats;
        type = "Slow_Curse";
        isApplied = false;
        removeFlag = false;
        image = _image;
    }

    override public void invokeCurse () 
    {
        Debug.Log(type + " Added!");
        isApplied = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CurseMeter>().activeCurses.Add(this);

        pStats.speed.AddBaseValue(-0.5f);
    } 

    override public void removeCurse () 
    {
        removeFlag = false;
        isApplied = false;
        active = false;
        pStats.speed.AddBaseValue(0.5f);
    } 
}
