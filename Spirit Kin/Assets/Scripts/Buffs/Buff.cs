using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Buff")]
public class Buff : ScriptableObject
{
	[SerializeField]
	public string teaName;
    public statType stat;
	public enum statType  { none, health, armor, damage, speed };
	[Tooltip("How powerful the base effect will be")]
	public int basePower;

	[Tooltip("How long the effect will linger. Use -1 for forever, use 0 for instant.")]
	public int duration;

    public bool isApplied = false;
	
}