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

    public void Start()
    {
        es = GameObject.Find("ShrineManager").GetComponent<Enemy_Spawner>();
    }

    public void Update()
    {
        myTime += Time.deltaTime;    
        //spawn an enemy at a shrine if there are 3 conditions met
            //1: The shrine must be cursed
            //2: There must have been at least [spawnInterval] seconds that have passed
            //3: The current amount of enemies instanced [amountAlreadySpawned] must be less than the max amount to be instanced for the current cursing [enemiesToSpawnWhenCursed]
        if (cursed && myTime > es.spawnInterval && amountAlreadySpawned < enemiesToSpawnWhenCursed)
        {
            es.spawnEnemy(transform);
            myTime = 0;
        }
    }
}
