using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teaBlessing : Curse
{
    private PlayerStats pStats;
    private CurseMeter cMeter;
    private List<float> basePowers;
    private List<Buff> teas;
    
    public teaBlessing (Sprite _image, TeaShopManager _teaList, PlayerStats _pStats, CurseMeter _cMeter)
    {
        pStats = _pStats;
        type = "Tea Blessing";
        isApplied = false;
        removeFlag = false;
        image = _image;
        cMeter = _cMeter;
        teas = _teaList.shopBuffList;

        basePowers = new List<float>();
        // Records all base powers prior to modification.
        for (int i = 0; i < teas.Count; ++i) {
            basePowers.Add(teas[i].basePower);
        }
        penaltyValue = 1.5f;
    }

    override public void invokeCurse () 
    {
        isApplied = true;
        cMeter.activeCurses.Add(this);

        updateCurse();
    }

    override public void updateCurse () {
        if(basePowers.Count == 0) return;
        
        for (int i = 0; i < teas.Count; ++i) {
            teas[i].basePower = basePowers[i] * penaltyValue;
            teas[i].updateDescription();

            if (teas[i].isApplied) {
                
                switch (teas[i].stat) {
                    case (Buff.statType.health):
                        Debug.Log("Before Update: " + pStats.maxHealth);
                        pStats.maxHealth -= teas[i].power;
                        pStats.currentHealth -= teas[i].power;
                        teas[i].power = (teas[i].level + 1) * teas[i].basePower;
                        pStats.maxHealth += teas[i].power;
                        pStats.currentHealth += teas[i].power;
                        break;
                    case (Buff.statType.armor):
                        pStats.armor.AddBaseValue(-teas[i].power);
                        teas[i].power = (teas[i].level + 1) * teas[i].basePower;
                        pStats.armor.AddBaseValue(teas[i].power);
                        break;
                    case (Buff.statType.damage):
                        pStats.damage.AddBaseValue(-teas[i].power);
                        teas[i].power = (teas[i].level + 1) * teas[i].basePower;
                        pStats.damage.AddBaseValue(teas[i].power);
                        break;
                    case (Buff.statType.speed):
                        pStats.speed.AddBaseValue(-teas[i].power);
                        teas[i].power = (teas[i].level + 1) * teas[i].basePower;
                        pStats.speed.AddBaseValue(teas[i].power);
                        break;
                }
            }
        }
    }

    override public void updateCurse (float difficulty) {
        penaltyValue = (1.0f + difficulty) * (penaltyValue - 1.0f);
        updateCurse();
    }

    override public void removeCurse () 
    {
        removeFlag = false;
        isApplied = false;
        active = false;
        
        for (int i = 0; i < teas.Count; ++i) {
            teas[i].basePower = basePowers[i];
            teas[i].updateDescription();
            if (teas[i].isApplied) {
                switch (teas[i].stat) {
                    case (Buff.statType.health):
                        pStats.maxHealth -= teas[i].power;
                        pStats.currentHealth -= teas[i].power;
                        teas[i].power = (teas[i].level + 1) * teas[i].basePower;
                        pStats.maxHealth += teas[i].power;
                        pStats.currentHealth += teas[i].power;
                        break;
                    case (Buff.statType.armor):
                        pStats.armor.AddBaseValue(-teas[i].power);
                        teas[i].power = (teas[i].level + 1) * teas[i].basePower;
                        pStats.armor.AddBaseValue(teas[i].power);
                        break;
                    case (Buff.statType.damage):
                        pStats.damage.AddBaseValue(-teas[i].power);
                        teas[i].power = (teas[i].level + 1) * teas[i].basePower;
                        pStats.damage.AddBaseValue(teas[i].power);
                        break;
                    case (Buff.statType.speed):
                        pStats.speed.AddBaseValue(-teas[i].power);
                        teas[i].power = (teas[i].level + 1) * teas[i].basePower;
                        pStats.speed.AddBaseValue(teas[i].power);
                        break;
                }
            }
        }
    } 
}
