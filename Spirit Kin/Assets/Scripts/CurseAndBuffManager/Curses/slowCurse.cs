using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowCurse : Curse
{
    private PlayerController pController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    
    public slowCurse () : base () 
    {
        type = "Slow_Curse";
        isApplied = false;
        removeFlag = false;
    }

    override public void invokeCurse () 
    {
        Debug.Log(type + " Added!");
        isApplied = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CurseMeter>().activeCurses.Add(this);
        pController.WalkSpeed *= 0.75f;
        pController.SprintSpeed *= 0.75f;
        pController.MinimumSpeed *= 0.75f;
    } 

    override public void removeCurse () 
    {
        removeFlag = false;
        pController.WalkSpeed *= (4f / 3f);
        pController.SprintSpeed *= (4f / 3f);
        pController.MinimumSpeed *= (4f / 3f);
    } 
}
