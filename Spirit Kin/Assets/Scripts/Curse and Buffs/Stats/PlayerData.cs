using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{

    public long GoldEarned;
    public long DamageDealt;
    public long DamageTaken;
    public long TimeSurvived;
    public long SpiritDefeated;
    public long ShrinePurified;
    public long CursesPurified;
    public long BuffsPurchased;
    public long WeaponPurchased;
    public long DistanceTraveled;

    //PlayerData Constructor
    public PlayerData()
    {
        GoldEarned = 0;
        DamageDealt = 0;
        DamageTaken = 0;
        TimeSurvived = 0;
        SpiritDefeated = 0;
        ShrinePurified = 0;
        CursesPurified = 0;
        BuffsPurchased = 0;
        WeaponPurchased = 0;
        DistanceTraveled = 0;
    }
    
    
}
