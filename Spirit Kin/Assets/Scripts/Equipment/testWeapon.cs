using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testWeapon : Equipment
{
    public testWeapon(Sprite sprite)
    {
        name = "testWeapon";
        description = "testWeapon";
        icon = sprite;
        isEquipped = false;
    }
}
