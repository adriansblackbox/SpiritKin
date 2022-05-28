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
        duration = 100f;
        Cost = 25;
        InvestCost = 100;
        isEquipped = false;
    }
}
