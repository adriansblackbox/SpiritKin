using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class invertCurse : Curse
{
    private GameObject player;
    private PlayerController pControl;
    public float timerTime;
    public Inverter inv;

    public invertCurse (Sprite _image, GameObject _player, Inverter _inv) {
        type = "invert_Curse";
        isApplied = false;
        removeFlag = false;
        image = _image;
        player = _player;
        pControl = player.GetComponent<PlayerController>();
        inv = _inv;

        timerTime = 50.0f;
    }

    override public void invokeCurse () {
        Debug.Log(type + " Added!");
        isApplied = true;
        player.GetComponent<CurseMeter>().activeCurses.Add(this);

        player.SendMessage("InvertControls", true);
        player.SendMessage("passInvertCurse", this);
        inv.enabled = true;
        inv.inverted = true;
    } 

    override public void updateCurse (float difficulty) {
        float nv = timerTime / difficulty + 1.0f;
        if (nv < 5.0f) nv = 5.0f;
        timerTime = nv;
    }

    override public void removeCurse () {
        removeFlag = false;
        isApplied = false;
        active = false;

        inv.enabled = false;
        inv.inverted = false;

        player.SendMessage("InvertControls", false);
    } 
}
