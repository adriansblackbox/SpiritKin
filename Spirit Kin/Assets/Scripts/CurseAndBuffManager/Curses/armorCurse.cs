using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Buff;

public class armorCurse : Curse
{
    private CharacterStats pStats;

    public armorCurse (Sprite _image, CharacterStats _pStats)
    {
        pStats = _pStats;
        type = "Armor_Curse";
        isApplied = false;
        removeFlag = false;
        image = _image;
    }

    override public void invokeCurse () 
    {
        isApplied = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CurseMeter>().activeCurses.Add(this);
        pStats.armor.AddBaseValue(-10);
    } 

    override public void removeCurse () 
    {
        removeFlag = false;
        isApplied = false;
        active = false;
        pStats.armor.AddBaseValue(10);
    } 
}
