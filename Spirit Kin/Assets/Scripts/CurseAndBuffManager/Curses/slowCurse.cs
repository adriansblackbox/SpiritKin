using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Buff;

public class slowCurse : Curse
{
    private PlayerStats pStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    
    public slowCurse (Sprite _image)
    {
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

        pStats.Buffs.Add(new Buff(Buff.statType.speed, -10, -1, this));
    } 

    override public void removeCurse () 
    {
        removeFlag = false;
        pStats.Buffs.Find(x => x.curseParent == this).removeFlag = true;
    } 
}
