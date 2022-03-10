using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy_Action/Enemy_Attack")]
public class Enemy_Attack : Enemy_Action
{
    public int attackPriority = 3; //higher the priority the more the attack will happen
    public float recoveryTime = 2f; //time it takes enemy to recover after an attack

    public float minimumAttackAngle = -50f;
    public float maximumAttackAngle = 50f;

    //minimum and maximum range that an enemy can use an attack at
    public float minimumAttackRange = 0;
    public float maximumAttackRange = 5;
}
