using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //create gameobject for each stat
    public GameObject GoldEarnedUI;
    public GameObject DamageDealtUI;
    public GameObject DamageTakenUI;
    public GameObject TimeSurvivedUI;
    public GameObject SpiritDefeatedUI;
    public GameObject ShrinePurifiedUI;
    public GameObject CursesPurifiedUI;
    public GameObject BuffsPurchasedUI;
    public GameObject WeaponPurchasedUI;
    public GameObject DistanceTraveledUI;

    private GameManager gm;
    private float myTime;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        myTime += Time.deltaTime;
        if (myTime > 1.0f)
        {
            addTimeSurvived(1);
            myTime = 0f;
        }    
    }

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

    //setters
    public void addGoldEarned(int gold)
    {
        GoldEarned += gold;
    }
    public void addDamageDealt(int damage) //implemented
    {
        DamageDealt += damage;
    }
    public void addDamageTaken(int damage) //implemented
    {
        DamageTaken += damage;
    }
    public void addTimeSurvived(int time) //implemented
    {
        TimeSurvived += time;
    }
    public void addSpiritDefeated(int spirit) //implemented
    {
        SpiritDefeated += spirit;
    }
    public void addShrinePurified(int shrine) //implemented
    {
        ShrinePurified += shrine;
    }
    public void addCursesPurified(int curse) //implemented
    {
        CursesPurified += curse;
    }
    public void addBuffsPurchased(int buff) //implemented
    {
        BuffsPurchased += buff;
    }
    public void addWeaponPurchased(int weapon)
    {
        WeaponPurchased += weapon;
    }
    public void addDistanceTraveled(int distance)
    {
        DistanceTraveled += distance;
    }

    void Update()
    {
        //update UI
        GoldEarnedUI.GetComponent<Text>().text = "Gold Earned: " + GoldEarned;
        DamageDealtUI.GetComponent<Text>().text = "Damage Dealt: " + DamageDealt;
        DamageTakenUI.GetComponent<Text>().text = "Damage Taken: " + DamageTaken;
        TimeSurvivedUI.GetComponent<Text>().text = "Time Survived: " + TimeSurvived/60 + ":" + TimeSurvived%60;
        SpiritDefeatedUI.GetComponent<Text>().text = "Spirit Defeated: " + SpiritDefeated;
        ShrinePurifiedUI.GetComponent<Text>().text = "Shrine Purified: " + ShrinePurified;
        CursesPurifiedUI.GetComponent<Text>().text = "Curses Purified: " + CursesPurified;
        BuffsPurchasedUI.GetComponent<Text>().text = "Buffs Purchased: " + BuffsPurchased;
        WeaponPurchasedUI.GetComponent<Text>().text = "Weapons Purchased: " + WeaponPurchased;
        DistanceTraveledUI.GetComponent<Text>().text = "Distance Traveled: " + DistanceTraveled;
        //save data
        // SaveData();

    }

    //save data
    public void SaveData()
    {
        PlayerPrefs.SetInt("GoldEarned", (int)GoldEarned);
        PlayerPrefs.SetInt("DamageDealt", (int)DamageDealt);
        PlayerPrefs.SetInt("DamageTaken", (int)DamageTaken);
        PlayerPrefs.SetInt("TimeSurvived", (int)TimeSurvived);
        PlayerPrefs.SetInt("SpiritDefeated", (int)SpiritDefeated);
        PlayerPrefs.SetInt("ShrinePurified", (int)ShrinePurified);
        PlayerPrefs.SetInt("CursesPurified", (int)CursesPurified);
        PlayerPrefs.SetInt("BuffsPurchased", (int)BuffsPurchased);
        PlayerPrefs.SetInt("WeaponPurchased", (int)WeaponPurchased);
        PlayerPrefs.SetInt("DistanceTraveled", (int)DistanceTraveled);
    }

    //load data
    public void LoadData()
    {
        GoldEarned = PlayerPrefs.GetInt("GoldEarned");
        DamageDealt = PlayerPrefs.GetInt("DamageDealt");
        DamageTaken = PlayerPrefs.GetInt("DamageTaken");
        TimeSurvived = PlayerPrefs.GetInt("TimeSurvived");
        SpiritDefeated = PlayerPrefs.GetInt("SpiritDefeated");
        ShrinePurified = PlayerPrefs.GetInt("ShrinePurified");
        CursesPurified = PlayerPrefs.GetInt("CursesPurified");
        BuffsPurchased = PlayerPrefs.GetInt("BuffsPurchased");
        WeaponPurchased = PlayerPrefs.GetInt("WeaponPurchased");
        DistanceTraveled = PlayerPrefs.GetInt("DistanceTraveled");
    }

    //reset data
    public void ResetData()
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
