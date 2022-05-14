using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vampBlessing : Curse
{
    private GameObject player;
    private SwordCollision swordLength;
    private CurseMeter cMeter;
    
    public vampBlessing (Sprite _image, SwordCollision _swordLength, GameObject _player, CurseMeter _cMeter)
    {
        player = _player;
        type = "Range Blessing";
        isApplied = false;
        removeFlag = false;
        image = _image;
        cMeter = _cMeter;
        swordLength = _swordLength;

        penaltyValue = 0.1f;
    }

    override public void invokeCurse () 
    {
        isApplied = true;
        cMeter.activeCurses.Add(this);

        swordLength.vampBlessingOn = true;
        swordLength.vampAmount = penaltyValue;
    }

    override public void updateCurse(float difficulty) {
        penaltyValue *= (1 + difficulty);
        swordLength.vampAmount = penaltyValue;
    }

    override public void removeCurse () 
    {
        removeFlag = false;
        isApplied = false;
        active = false;

        swordLength.vampBlessingOn = false;
    } 
}