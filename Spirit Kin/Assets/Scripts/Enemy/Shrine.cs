using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class Shrine : MonoBehaviour
{
    private Enemy_Spawner es;
    private AI_Manager ai;
    private GameManager gm;
    private PlayerData pd;
    private MainHub mh;

    public bool cursed;
    public float shrineSpawnRange;

    public int enemiesToSpawn; //actual value of enemies to spawn

    public int enemiesToSpawnWhenCursed; //tracking the value to be used when the shrine gets crused
    public int amountAlreadySpawned; //how many enemies have been spawned on this current cursing
    
    private float myTime = 3f;
    public float TotalCurseTime = 90f;
    public float CurCurseTime = 0f;
    public GameObject nonCursedContainer;

    //bounds for choosing spawning/patroling spots for the shrine
    public float posUpperX; 
    public float negUpperX; 
    public float posUpperZ; 
    public float negUpperZ;

    public float posLower = 5f;
    public float negLower = -5f; 

    //game over scene
    public GameObject gameOverScreen;
    public VisualEffect CurseBecon;

    public void Start()
    {
        es = GameObject.Find("ShrineManager").GetComponent<Enemy_Spawner>();
        ai = GetComponent<AI_Manager>();
        gm = FindObjectOfType<GameManager>();
        pd = FindObjectOfType<PlayerData>();
        mh = FindObjectOfType<MainHub>();
        ai.generateSurroundLocations();
    }

    //ensure that timers aren't running when timeScale = 0
    public void FixedUpdate()
    {
        myTime += Time.deltaTime;
    }

    public void Update()
    {

        if (amountAlreadySpawned >= enemiesToSpawn && transform.GetChild(0).childCount == 0 && cursed)
        {   
            cursed = false;
            amountAlreadySpawned = 0;
            transform.parent = nonCursedContainer.transform;
            es.currentCursedShrines--;
            pd.addShrinePurified(1);
            StopCurseVFX();
        }

        if (CurCurseTime < TotalCurseTime && cursed && !FindObjectOfType<MainHub>().playerInHub) {
            CurCurseTime += Time.deltaTime;
        } else if (cursed && CurCurseTime >= TotalCurseTime) {
            gm.gameOver = true;
            gameOverScreen.GetComponent<GameOver>().LoadGameOver();
            cursed = false;
        }
        if(FindObjectOfType<MainHub>().playerInHub)
            CurseBecon.pause = true;
        else
            CurseBecon.pause = false;

        //spawn an enemy at a shrine if there are 3 conditions met
            //1: The shrine must be cursed
            //2: There must have been at least [spawnInterval] seconds that have passed
            //3: The current amount of enemies instanced [amountAlreadySpawned] must be less than the max amount to be instanced for the current cursing [enemiesToSpawnWhenCursed]
        if (myTime > es.spawnInterval)
        {
            myTime = 0;
            if (amountAlreadySpawned < enemiesToSpawn && cursed)
                es.spawnEnemy(gameObject);
        }

    }

    public void setEnemiesToSpawnWhenCursed(int enemies) {
        enemiesToSpawnWhenCursed = enemies;
    }

    public void setEnemiesToSpawn() {
        enemiesToSpawn = enemiesToSpawnWhenCursed;
    }
    public void PlayCurseVFX() {
        //sets CurseBecon (and other vfx) duration parameter to duration, and plays effect
        CurseBecon.playRate = 1;
        CurseBecon.SetFloat("Duration", TotalCurseTime);
        CurseBecon.Play();
    }
    public void StopCurseVFX() {
        // manually stops CurseBecon (and other vfx) from playing
        CurseBecon.playRate = 100;
    }
}
