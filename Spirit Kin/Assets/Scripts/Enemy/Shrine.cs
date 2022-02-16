using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour
{
    private Enemy_Spawner es;

    public bool cursed;
    public float shrineSpawnRange;
    public int enemiesToSpawnWhenCursed; //how many enemies will be spawned each time this shrine gets cursed
    public int amountAlreadySpawned; //how many enemies have been spawned on this current cursing

    //TRACK HOW MANY ENEMIES THE PLAYER HAS BEATEN
    
    private float myTime;
    public float TotalCurseTime = 3f;
    private float CurCurseTime = 0f;
    public GameObject Beacon;

    public void Start()
    {
        es = GameObject.Find("ShrineManager").GetComponent<Enemy_Spawner>();
        Beacon.SetActive(false);
    }

    public void Update()
    {
        if(CurCurseTime < TotalCurseTime && cursed){
            CurCurseTime += Time.deltaTime;
        }else if(cursed){
            Debug.Log("YOU SUCK");
            // For the laughs
            //Application.Quit();
        }
        if(cursed){
            Beacon.SetActive(true);
        }else{
            Beacon.SetActive(false);
        }
        myTime += Time.deltaTime;    
        //spawn an enemy at a shrine if there are 3 conditions met
            //1: The shrine must be cursed
            //2: There must have been at least [spawnInterval] seconds that have passed
            //3: The current amount of enemies instanced [amountAlreadySpawned] must be less than the max amount to be instanced for the current cursing [enemiesToSpawnWhenCursed]
        if (cursed && myTime > es.spawnInterval && amountAlreadySpawned < enemiesToSpawnWhenCursed)
        {
            //es.spawnEnemy(gameObject);
            myTime = 0;
        }
    }
}
