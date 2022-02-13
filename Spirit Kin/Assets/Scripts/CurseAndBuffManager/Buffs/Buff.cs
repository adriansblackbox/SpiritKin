using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Buff")]
public class Buff : ScriptableObject
{
	[SerializeField]
    public statType stat;
	public enum statType  { none, health, armor, damage, speed };
	[Tooltip("How powerful the base effect will be")]
	public int basePower;

	[Tooltip("How long the effect will linger. Use -1 for forever, use 0 for instant.")]
	public float duration;

    public bool isApplied = false;
	public bool removeFlag = false;

	public Buff(statType _stat, int _basePower, float _duration) {
		this.stat = _stat;
		this.basePower = _basePower;
		this.duration = _duration;
		this.isApplied = false;
		this.removeFlag = false;
	}
}