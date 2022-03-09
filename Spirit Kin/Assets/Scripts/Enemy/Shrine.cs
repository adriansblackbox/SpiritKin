using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shrine : MonoBehaviour
{
    private Enemy_Spawner es;
    private AI_Manager ai;

    public bool cursed;
    public float shrineSpawnRange;

    private int enemiesToSpawn; //actual value of enemies to spawn

    public int enemiesToSpawnWhenCursed; //tracking the value to be used when the shrine gets crused
    public int amountAlreadySpawned; //how many enemies have been spawned on this current cursing

    //TRACK HOW MANY ENEMIES THE PLAYER HAS BEATEN
    
    private float myTime;
    public float TotalCurseTime = 90f;
    public float CurCurseTime = 0f;
    public GameObject Beacon;
    public GameObject nonCursedContainer;

    //bounds for choosing spawning/patroling spots for the shrine
    public float posUpperX; 
    public float negUpperX; 
    public float posUpperZ; 
    public float negUpperZ;

    public float posLower = 5f;
    public float negLower = -5f; 

    public void Start()
    {
        es = GameObject.Find("ShrineManager").GetComponent<Enemy_Spawner>();
        ai = GetComponent<AI_Manager>();
        Beacon.SetActive(false);
        ai.generateSurroundLocations();
    }

    public void Update()
    {
        myTime += Time.deltaTime;

        if (amountAlreadySpawned >= enemiesToSpawn && transform.GetChild(0).childCount == 0 && cursed)
        {   
            cursed = false;
            amountAlreadySpawned = 0;
            transform.parent = nonCursedContainer.transform;
            es.currentCursedShrines--;
        }
            

        if (CurCurseTime < TotalCurseTime && cursed) {
            CurCurseTime += Time.deltaTime;
        } else if(cursed) {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("MainMenu");
        }

        if (cursed) {
            Beacon.SetActive(true);
        } else {
            Beacon.SetActive(false);
        }

        //spawn an enemy at a shrine if there are 3 conditions met
            //1: The shrine must be cursed
            //2: There must have been at least [spawnInterval] seconds that have passed
            //3: The current amount of enemies instanced [amountAlreadySpawned] must be less than the max amount to be instanced for the current cursing [enemiesToSpawnWhenCursed]
        if (cursed && myTime > es.spawnInterval && amountAlreadySpawned < enemiesToSpawn)
        {
            myTime = 0;
            es.spawnEnemy(gameObject);
        } else if (myTime > es.spawnInterval) myTime = 0;
    }

    public void setEnemiesToSpawnWhenCursed(int enemies) {
        enemiesToSpawnWhenCursed = enemies;
    }

    public void setEnemiesToSpawn() {
        enemiesToSpawn = enemiesToSpawnWhenCursed;
    }
}
