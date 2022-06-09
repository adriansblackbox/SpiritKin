using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testWeapon : Equipment
{
    public testWeapon(Sprite sprite)
    {
        equipName = "Aether Pull";
        description = "Imbues weapon with Aether Pull\nLasts 90s";
        equipSprite = sprite;
        duration = 90f;
        Cost = 75;
        investCost = 100;
        isEquipped = false;
    }
}
