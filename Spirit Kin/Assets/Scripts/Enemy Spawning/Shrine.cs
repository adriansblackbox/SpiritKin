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
        es = GameObject.Find("SpawnManager").GetComponent<Enemy_Spawner>();
    }

    public void Update()
    {
        myTime += Time.deltaTime;
        if (myTime > 3) //every 15 seconds
        {
            float temp = Random.Range(0.0f, 1.0f);
            if (temp < 0.15 && es.currentCursedShrines < es.maxCursedShrines) //15% chance for shrine to be activated
            {
                cursed = true;
                es.currentCursedShrines++;
                Debug.Log("Shrine has been activated");
            }
            myTime = 0;
        }
    }
}
