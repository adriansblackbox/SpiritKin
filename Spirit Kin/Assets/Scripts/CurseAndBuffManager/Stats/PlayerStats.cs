using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    [SerializeField]
    public List<Buff> Buffs = new List<Buff>();

    public GameObject[] BuffsUI;
    public Sprite Notch, damageBuff, speedBuff, armorBuff, healthBuff;

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
        int i = -1;
        Buffs
            .ForEach(x =>
            {
                i = Buffs.FindIndex(y => y.teaName == x.teaName);
                if (!x.isApplied)
                {
                    switch (x.stat)
                    {
                        case (Buff.statType.health):
                            {
                                this.maxHealth += x.basePower;
                                this.currentHealth += x.basePower;
                                BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = healthBuff;
                   
                                // Buffs.Remove(x);
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.armor):
                            {
                                this.armor.AddBaseValue(x.basePower);
                                BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = armorBuff;

                                // Buffs.Remove(x);
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.damage):
                            {
                                this.damage.AddBaseValue(x.basePower);
                                BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = damageBuff;

                                // Buffs.Remove(x);
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.speed):
                            {
                                this.speed.AddBaseValue(x.basePower);
                                BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = speedBuff;

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
                    BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = Notch;
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
        if(Buffs.Count < 3){
            x.isApplied = false;
            Buffs.Add (x);
        }
    }

    public void removeBuff(Buff x)
    {
        int i = Buffs.FindIndex(y => y.teaName == x.teaName);
        Buffs[i].removeFlag = true;
    }
}
