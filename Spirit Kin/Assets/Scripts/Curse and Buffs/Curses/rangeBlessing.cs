using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rangeBlessing : Curse
{
    private GameObject player;
    private SwordCollision swordLength;
    private CurseMeter cMeter;
    public float baseValue;
    
    public rangeBlessing (Sprite _image, SwordCollision _swordLength, GameObject _player, CurseMeter _cMeter)
    {
        player = _player;
        type = "Range Blessing";
        isApplied = false;
        removeFlag = false;
        image = _image;
        cMeter = _cMeter;
        swordLength = _swordLength;

        penaltyValue = 1.25f;
        baseValue = swordLength.BladeLength;
    }

    override public void invokeCurse () 
    {
        isApplied = true;
        cMeter.activeCurses.Add(this);
    }

    override public void updateCurse () {
        if (active) {
            swordLength.BladeLength = baseValue * penaltyValue;
            swordLength.ScaleVFXToBlade();
        }
        else { 
            swordLength.BladeLength = baseValue;
            swordLength.ScaleVFXToBlade();
        }
    }

    override public void removeCurse () 
    {
        removeFlag = false;
        isApplied = false;
        active = false;

        swordLength.BladeLength = baseValue;
        swordLength.ScaleVFXToBlade();
    } 
}
