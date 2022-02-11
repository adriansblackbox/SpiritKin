using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowCurse : Curse
{
    private PlayerController pController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    
    override public void invokeCurse () {
        pController.WalkSpeed *= 0.75f;
        pController.SprintSpeed *= 0.75f;
        pController.MinimumSpeed *= 0.75f;
    } 

    override public void removeCurse () {
        pController.WalkSpeed *= (4f / 3f);
        pController.SprintSpeed *= (4f / 3f);
        pController.MinimumSpeed *= (4f / 3f);
    } 
}
