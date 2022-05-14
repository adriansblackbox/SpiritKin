using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;

public class invertCurse : Curse
{
    private static GameObject player;
    private static PlayerController pControl;
    private System.Timers.Timer timer;
    private int timerTime;

    public invertCurse (Sprite _image, GameObject _player) {
        type = "invert_Curse";
        isApplied = false;
        removeFlag = false;
        image = _image;
        player = _player;
        pControl = player.GetComponent<PlayerController>();

        timerTime = 1000;
    }

    override public void invokeCurse () {
        Debug.Log(type + " Added!");
        isApplied = true;
        player.GetComponent<CurseMeter>().activeCurses.Add(this);

        player.SendMessage("InvertControls", true);

        timer = new System.Timers.Timer(timerTime);
        timer.Elapsed += c_ThresholdReached;
        timer.AutoReset = true;
        timer.Enabled = true;
    } 

    private static void c_ThresholdReached(object sender, EventArgs e) {
        if(pControl.getInverted()) player.SendMessage("InvertControls", false);
        else player.SendMessage("InvertControls", true);

        Debug.Log("Calling!");
    }

    override public void removeCurse () {
        removeFlag = false;
        isApplied = false;
        active = false;


        player.SendMessage("InvertControls", false);
    } 
}
