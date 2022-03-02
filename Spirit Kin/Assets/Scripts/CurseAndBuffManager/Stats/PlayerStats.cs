using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    [SerializeField]
    public List<Buff> Buffs = new List<Buff>();

    public List<GameObject> BuffsUI = new List<GameObject>();
    public Sprite Notch, damageBuff, speedBuff, armorBuff, healthBuff;

    void Start()
    {
        coins = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)){
            Debug.Log("dead");
            Die();
        }
        
        SoulsUI.text = coins + "/" + maxSouls;
        if (coins > maxSouls)
        {
            coins = maxSouls;
        }
        if (Buffs.Count != 0)
        {
            BuffHandler (Buffs);
            for(int i = 0; i < Buffs.Count; i++){
                if(Buffs[i].timeActive < Buffs[i].duration){
                    Buffs[i].timeActive += Time.deltaTime;
                    BuffsUI[i].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 1 - Buffs[i].timeActive/Buffs[i].duration;
                }else{
                    BuffsUI[i].transform.Find("Bar").gameObject.GetComponent<Image>().enabled = false;
                    Buffs[i].removeFlag = true;
                }
            }
        }
    }

    public void BuffHandler(List<Buff> Buffs)
    {
        Buffs
            .ForEach(x =>
            {
                int i = Buffs.FindIndex(y => y.teaName == x.teaName);
                if (!x.isApplied)
                {
                    switch (x.stat)
                    {
                        case (Buff.statType.health):
                            {
                                this.maxHealth += x.basePower;
                                this.currentHealth += x.basePower;
                                BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = healthBuff;
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.armor):
                            {
                                this.armor.AddBaseValue(x.basePower);
                                BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = armorBuff;
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.damage):
                            {
                                this.damage.AddBaseValue(x.basePower);
                                BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = damageBuff;
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.speed):
                            {
                                this.speed.AddBaseValue(x.basePower);
                                BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = speedBuff;
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                    }
                    BuffsUI[i].transform.Find("Bar").gameObject.GetComponent<Image>().enabled = true;
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
                                x.isApplied = false;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.armor):
                            {
                                this.armor.AddBaseValue(-x.basePower);
                                x.isApplied = false;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.damage):
                            {
                                this.damage.AddBaseValue(-x.basePower);
                                x.isApplied = false;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.speed):
                            {
                                this.speed.AddBaseValue(-x.basePower);
                                x.isApplied = false;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                    }
                    BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = Notch;
                }
            });
            for(int i = 0; i < Buffs.Count; i ++){
                if(Buffs[i].removeFlag){
                    Buffs.RemoveAt(i);
                    BuffsUI.Add(BuffsUI[0]);
                    BuffsUI.RemoveAt(0);
                }
            }
    }
    public void addBuff(Buff x)
    {
        if(Buffs.Count < 3){
            x.timeActive = 0;
            x.isApplied = false;
            x.removeFlag = false;
            Buffs.Add (x);
        }
    }

    public void removeBuff(Buff x)
    {
        int i = Buffs.FindIndex(y => y.teaName == x.teaName);
        Buffs[i].removeFlag = true;
        // make sure that shop buff is purchasable
        Buffs[i].isApplied = false;
    }
}
