using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Buff;

public class damageCurse : Curse
{
    private CharacterStats pStats;
    
    public damageCurse (Sprite _image, CharacterStats _pStats)
    {
        pStats = _pStats;
        type = "Damage_Curse";
        isApplied = false;
        removeFlag = false;
        image = _image;
    }

    override public void invokeCurse () 
    {
        Debug.Log(type + " Added!");
        isApplied = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CurseMeter>().activeCurses.Add(this);

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
