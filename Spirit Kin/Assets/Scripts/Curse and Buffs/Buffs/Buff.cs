using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "Buff", menuName = "Buff")]
public class Buff
{
    public float baseDuration = 60f;

    public float duration;

    public float baseTimeActive = 0f;

    public float timeActive;

    public string teaName;

    public string description;

    public int Cost;

    public int InvestCost;

    public Sprite buffSprite;

    public statType stat;

    public enum statType
    {
        none,
        health,
        armor,
        damage,
        speed
    }

    public float basePower;

    public bool isApplied = false;

    public bool removeFlag = false;

    private void OnEnable()
    {
        isApplied = false;
        removeFlag = false;
        duration = baseDuration;
        timeActive = baseTimeActive;
    }
}
