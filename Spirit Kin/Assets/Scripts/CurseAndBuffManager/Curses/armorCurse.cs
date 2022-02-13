using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Buff;

public class armorCurse : Curse
{
    private PlayerStats pStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    Buff armorCurseDebuff;
    
    public armorCurse ()
    {
        type = "Armor_Curse";
        isApplied = false;
        removeFlag = false;
        armorCurseDebuff = new Buff(Buff.statType.armor, -10, -1);
        image = Resources.Load<Image>("UI/Armor_Curse");
    }

    override public void invokeCurse () 
    {
        Debug.Log(type + " Added!");
        isApplied = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CurseMeter>().activeCurses.Add(this);
        pStats.Buffs.Add(armorCurseDebuff);
    } 

    override public void removeCurse () 
    {
        removeFlag = false;
        armorCurseDebuff.removeFlag = true;
    } 
}
