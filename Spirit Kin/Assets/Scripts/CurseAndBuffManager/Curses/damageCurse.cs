using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Buff;

public class damageCurse : Curse
{
    private PlayerStats pStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    Buff damageDebuff;
    
    public damageCurse ()
    {
        type = "Damage_Curse";
        isApplied = false;
        removeFlag = false;
        damageDebuff = new Buff(Buff.statType.damage, -10, -1);
        image = Resources.Load<Image>("UI/Damage_Curse");
    }

    override public void invokeCurse () 
    {
        Debug.Log(type + " Added!");
        isApplied = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CurseMeter>().activeCurses.Add(this);
        pStats.Buffs.Add(damageDebuff);
    } 

    override public void removeCurse () 
    {
        removeFlag = false;
        damageDebuff.removeFlag = true;
    } 
}
