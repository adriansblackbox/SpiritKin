using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class blindCurse : Curse
{
    public blindCurse (Sprite _image)
    {
        type = "Blind_Curse";
        isApplied = false;
        removeFlag = false;
        image = _image;
    }

    override public void invokeCurse () 
    {
        Debug.Log(type + " Added!");
        isApplied = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CurseMeter>().activeCurses.Add(this);
    } 

    override public void removeCurse () {} 
}
