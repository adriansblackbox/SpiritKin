using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class invertCurse : Curse
{
    public invertCurse () : base () 
    {
        type = "invert_Curse";
        isApplied = false;
        removeFlag = false;
    }

    override public void invokeCurse () 
    {
        Debug.Log(type + " Added!");
        isApplied = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CurseMeter>().activeCurses.Add(this);
    } 

    override public void removeCurse () {} 
}
