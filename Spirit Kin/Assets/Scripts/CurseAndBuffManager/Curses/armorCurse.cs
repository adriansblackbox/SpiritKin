using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Buff;

public class armorCurse : Curse
{
    private PlayerStats pStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    
    public armorCurse (Sprite _image)
    {
        type = "Armor_Curse";
        isApplied = false;
        removeFlag = false;
        image = _image;
    }

    override public void invokeCurse () 
    {
        Debug.Log(type + " Added!");
        isApplied = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CurseMeter>().activeCurses.Add(this);
        Buff armorDebuff = new Buff(Buff.statType.armor, -10, -1, this);
        pStats.Buffs.Add(armorDebuff);
    } 

    override public void removeCurse () 
    {
        removeFlag = false;
        pStats.Buffs.Find(x => x.curseParent == this).removeFlag = true;
    } 
}
