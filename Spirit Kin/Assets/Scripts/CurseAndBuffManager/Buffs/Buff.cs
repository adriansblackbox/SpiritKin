using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Buff")]
public class Buff : ScriptableObject
{
	[SerializeField]
	public string teaName;
	public string description;
	public int Cost;
	public int InvestCost;
	public Sprite buffSprite;
    public statType stat;
	public enum statType  { none, health, armor, damage, speed };
	[Tooltip("How powerful the base effect will be")]
	public float basePower;

	[Tooltip("How long the effect will linger. Use -1 for forever, use 0 for instant.")]
	public float duration;
	public float timeActive = 0;

    public bool isApplied = false;
	public bool removeFlag = false;

	public void Awake(){
		isApplied = false;
		removeFlag = false;
	}
}