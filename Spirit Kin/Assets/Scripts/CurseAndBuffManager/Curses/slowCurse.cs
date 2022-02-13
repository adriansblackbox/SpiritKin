using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Buff;

public class slowCurse : Curse
{
    private PlayerStats pStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    Buff speedDebuff;
    
    public slowCurse ()
    {
        type = "Slow_Curse";
        isApplied = false;
        removeFlag = false;
        speedDebuff = new Buff(Buff.statType.speed, -10, -1);
        image = Resources.Load<Image>("UI/Quick_Curse");
    }

    override public void invokeCurse () 
    {
        Debug.Log(type + " Added!");
        isApplied = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CurseMeter>().activeCurses.Add(this);
        pStats.Buffs.Add(speedDebuff);
    } 

    override public void removeCurse () 
    {
        removeFlag = false;
        speedDebuff.removeFlag = true;
    } 
}
