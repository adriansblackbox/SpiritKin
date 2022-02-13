using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Buff;

public class damageCurse : Curse
{
    private PlayerStats pStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    
    public damageCurse (Sprite _image)
    {
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

        Buff damageDebuff = new Buff(Buff.statType.damage, -10, -1, this);
        pStats.Buffs.Add(damageDebuff);
    } 

    override public void removeCurse () 
    {
        removeFlag = false;
        pStats.Buffs.Find(x => x.curseParent == this).removeFlag = true;
    } 
}
