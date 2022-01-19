using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    [SerializeField]
    public List<Buff> Buffs = new List<Buff>();


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Buffs.Count != 0)
        {
            BuffHandler(Buffs);
        }

    }
    public void BuffHandler(List<Buff> Buffs)
    {


        Buffs.ForEach(x =>
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

    });

    }
}
