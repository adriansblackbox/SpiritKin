using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testWeapon : Equipment
{
    public testWeapon(Sprite sprite)
    {
        equipName = "testWeapon";
        description = "testWeapon";
        equipSprite = sprite;
        duration = 60f;
        Cost = 1;
        InvestCost = 5;
        isEquipped = false;
    }
}
