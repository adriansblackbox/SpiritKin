using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Buff")]
public class Buff : ScriptableObject
{
	[Tooltip("How long the effect will linger")]
	[SerializeField] float baseDuration = 60f;
	public float duration;
	[SerializeField] float baseTimeActive = 0f;
	public float timeActive;

	public string teaName;
	public string description;
	public int Cost;
	public int InvestCost;
	public Sprite buffSprite;
    public statType stat;
	public enum statType  { none, health, armor, damage, speed };
	[Tooltip("How powerful the base effect will be")]
	public int basePower;
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