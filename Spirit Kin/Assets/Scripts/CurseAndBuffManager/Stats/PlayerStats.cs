using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    [SerializeField]
    public List<Buff> Buffs = new List<Buff>();

    void Start()
    {
        Coins = 10000;
    }

    // Update is called once per frame
    void Update()
    {
        
        SoulsUI.text = currSouls + "/" + maxSouls;
        CoinsUI.text = "" + Coins;
        if (currSouls > maxSouls)
        {
            currSouls = maxSouls;
        }
        if (Buffs.Count != 0)
        {
            BuffHandler (Buffs);
        }
    }

    public void BuffHandler(List<Buff> Buffs)
    {
        Buffs
            .ForEach(x =>
            {
                if (!x.isApplied)
                {
                    switch (x.stat)
                    {
                        case (Buff.statType.health):
                            {
                                this.maxHealth += x.basePower;
                                this.currentHealth += x.basePower;

                                // Buffs.Remove(x);
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.armor):
                            {
                                this.armor.AddBaseValue(x.basePower);

                                // Buffs.Remove(x);
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.damage):
                            {
                                this.damage.AddBaseValue(x.basePower);

                                // Buffs.Remove(x);
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.speed):
                            {
                                this.speed.AddBaseValue(x.basePower);

                                // Buffs.Remove(x);
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                    }
                }
                if (x.removeFlag)
                {
                    switch (x.stat)
                    {
                        case (Buff.statType.health):
                            {
                                this.maxHealth -= x.basePower;
                                if (this.currentHealth > this.maxHealth)
                                {
                                    this.currentHealth = this.maxHealth;
                                }

                                // Buffs.Remove(x);
                                x.isApplied = false;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.armor):
                            {
                                this.armor.AddBaseValue(-x.basePower);

                                // Buffs.Remove(x);
                                x.isApplied = false;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.damage):
                            {
                                this.damage.AddBaseValue(-x.basePower);

                                // Buffs.Remove(x);
                                x.isApplied = false;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.speed):
                            {
                                this.speed.AddBaseValue(-x.basePower);

                                // Buffs.Remove(x);
                                x.isApplied = false;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                    }
                    Buffs.Remove (x);
                }
            });
    }

    public IEnumerator buffTimer(Buff x)
    {
        float timeRemaining = x.duration;
        float timeStep = 0.2f * timeRemaining;
        while (timeRemaining > 0)
        {
            // Demonstrate the change
            // lerpingFunction(timeRemaining, timeRemaining - timeStep)
            yield return new WaitForSeconds(timeStep);
        }
        x.removeFlag = true;
    }

    public void addBuff(Buff x)
    {
        Buffs.Add (x);
    }

    public void removeBuff(Buff x)
    {
        int i = Buffs.FindIndex(y => y.teaName == x.teaName);
        Buffs[i].removeFlag = true;
    }
}
