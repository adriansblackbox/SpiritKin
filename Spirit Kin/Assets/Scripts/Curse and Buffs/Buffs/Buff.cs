using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "Buff", menuName = "Buff")]
public class Buff
{
    public int baseLevel = 0;

    public int level;

    public float baseDuration = 60f;

    public float duration;

    public float baseTimeActive = 0f;

    public float timeActive;

    public string teaName;

    public string description;

    public int Cost;

    public int baseInvestCost = 100;

    public int investCost;

    public Sprite buffSprite;

    public statType stat;

    public string type;

    public enum statType
    {
        none,
        health,
        armor,
        damage,
        speed
    }

    public float basePower;

    public float power;

    public bool isApplied = false;

    public bool removeFlag = false;

    private void OnEnable()
    {
        isApplied = false;
        removeFlag = false;
        duration = baseDuration;
        timeActive = baseTimeActive;
        power = basePower;
        investCost = baseInvestCost;
        level = baseLevel;
    }

    public void updateDescription()
    {
        description = "Increases " + type + " by " + (level+1) * basePower + "\n Lasts " + duration + "s";
    }
}
