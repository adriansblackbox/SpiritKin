using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Spawner : MonoBehaviour
{
    public float difficulty = 0f;

    [SerializeField] int totalShrinesCursed = 0;
    [SerializeField] int lowerLimitEnemyCount = 1;
    [SerializeField] int upperLimitEnemyCount = 4;

    public GameObject nonCursedContainer;
    public GameObject cursedContainer;
    public GameObject enemyPrefab;
    public CurseMeter curseMeter;

    public int totalShrines;

    public float spawnInterval; //enemies will spawn in at intervals at shrines to show that they are slowly drawing in ghosts & to make feel more natural
    public int currentCursedShrines; //how many shrines are currently cursed

    private float myTime;
    private float difficultyTimer;
    public float shrineInterval = 45f;
    private bool firstSpawn = true;

    public void Start()
    {
        scaleNumberOfEnemiesToSpawn();
    }

    //ensure that timers aren't running when timeScale = 0
    public void FixedUpdate()
    {
        if (!FindObjectOfType<MainHub>().playerInHub)
        {
            myTime += Time.deltaTime;
            difficultyTimer += Time.deltaTime;
        }
    }

    public void Update()
    {

        if (difficultyTimer >= 30f)
        {
            scaleDifficulty();
            difficultyTimer = 0f;
        }

        if (firstSpawn)
        {
            if (myTime > 5f)
            {
                firstSpawn = false;
                myTime = 0;
                if (nonCursedContainer.transform.childCount > 0)
                {
                    curseShrine();
                }
            }
        }
        else if (myTime > shrineInterval) 
        {
            myTime = 0;
            if (nonCursedContainer.transform.childCount > 0) //every 15 seconds -> actually 45 to 60 seconds is probably better
            {
                curseShrine();
            }
        }
    }

    private void curseShrine()
    {
        int temp = Random.Range(0, nonCursedContainer.transform.childCount);
        Transform shrine = nonCursedContainer.transform.GetChild(temp);
        shrine.parent = cursedContainer.transform;
        shrine.GetComponent<Shrine>().cursed = true;
        shrine.GetComponent<Shrine>().CurCurseTime = 0f;
        currentCursedShrines++;
        scaleNumberOfEnemiesToSpawn();
        shrine.GetComponent<Shrine>().setEnemiesToSpawn();
    }

    //increments limits to help in scaling NumberOfEnemiesToSpawn
    private void scaleNumberOfEnemiesToSpawn() 
    {
        totalShrinesCursed++;
        if (totalShrinesCursed % 2 == 1) //lower limit increments every other round
            lowerLimitEnemyCount++;
        else if (totalShrinesCursed != 2 && totalShrinesCursed % 2 == 0 && upperLimitEnemyCount - lowerLimitEnemyCount == 1) //upper limit increments every round after the lower limit increments
            upperLimitEnemyCount++;
        selectShrineEnemyCount();
    }

    private void scaleDifficulty()
    {
        // soft cap for difficulty at 10
        if (difficulty < 10)
            difficulty += 0.334f;
        else
            difficulty += 0.0834f;
        Debug.Log("Difficulty is: " + difficulty);
        curseMeter.SendMessage("difficultyUpdateCurse", difficulty);
    }

    private void selectShrineEnemyCount()
    {
        for (int i = 0; i < totalShrines - currentCursedShrines; i++)
        {
            Transform shrine = nonCursedContainer.transform.GetChild(i);
            shrine.GetComponent<Shrine>().setEnemiesToSpawnWhenCursed(Random.Range(lowerLimitEnemyCount, upperLimitEnemyCount + 1));
        }

        for (int i = 0; i < currentCursedShrines; i++)
        {
            Transform shrine = cursedContainer.transform.GetChild(i);
            shrine.GetComponent<Shrine>().setEnemiesToSpawnWhenCursed(Random.Range(lowerLimitEnemyCount, upperLimitEnemyCount + 1));
        }
    }

    public void spawnEnemy(GameObject shrineToSpawnAt)
    {
        //find a randomized location for the enemy to spawn
        Vector3 enemyPosition = chooseLocation(shrineToSpawnAt.transform).position;
        //spawn in enemy
        GameObject enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        enemy.GetComponent<Enemy_Controller>().ai = shrineToSpawnAt.GetComponent<AI_Manager>();
        enemy.GetComponent<Enemy_Controller>().shrine = shrineToSpawnAt.transform;
        enemy.GetComponent<Enemy_Controller>().shrineSpawnRange = shrineToSpawnAt.GetComponent<Shrine>().shrineSpawnRange;
        shrineToSpawnAt.GetComponent<AI_Manager>().enemiesIdling.Add(enemy);

        //set their stats to scale with difficulty
        float tmp = Random.Range(5,20);
        CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
        enemyStats.coins = (int) (tmp + tmp * difficulty);
        enemyStats.maxHealth = Mathf.Round(enemyStats.maxHealth + enemyStats.maxHealth * difficulty);
        enemyStats.currentHealth = enemyStats.maxHealth;
        enemyStats.damage.AddBaseValue(Mathf.Round(25 * (difficulty/4))); //The 25 should be an adjustable value linked to the damage stat, but works for now
        
        //put in enemy container
        enemy.transform.parent = shrineToSpawnAt.transform.GetChild(0);

        //increment enemies spawned at shrine
        shrineToSpawnAt.GetComponent<Shrine>().amountAlreadySpawned++;
    }

    public NavMeshHit chooseLocation(Transform shrine)
    {
        var shrineScript = shrine.GetComponent<Shrine>();
        NavMeshHit hit;
        //choose a quadrant to spawn into
            //for now random
                //maybe later work on balancing it so that there is enemies in each quadrant
        float temp = Random.Range(0.0f, 1.0f);

        Vector3 spawnPoint = shrine.position;
        if (temp > 0.75f) //quadrant 1 positive X + negative Z
        {
            spawnPoint.x += Random.Range(shrineScript.posLower, shrineScript.posUpperX);
            spawnPoint.z += Random.Range(shrineScript.negLower, shrineScript.negUpperZ);
        }
        else if (temp > 0.5f) //quadrant 2 negative X + negative Z
        {
            spawnPoint.x += Random.Range(shrineScript.negLower, shrineScript.negUpperX);
            spawnPoint.z += Random.Range(shrineScript.negLower, shrineScript.negUpperZ);
        }
        else if (temp > 0.25f) //quadrant 3 positive X + positive Z
        {
            spawnPoint.x += Random.Range(shrineScript.posLower, shrineScript.posUpperX);
            spawnPoint.z += Random.Range(shrineScript.posLower, shrineScript.posUpperZ);
        }
        else //quadrant 4 negative X + positive Z
        {
            spawnPoint.x += Random.Range(shrineScript.negLower, shrineScript.negUpperX);
            spawnPoint.z += Random.Range(shrineScript.posLower, shrineScript.posUpperZ);
        }
        //Navmesh.SamplePosition on the random position
        NavMesh.SamplePosition(spawnPoint, out hit, 100.0f, NavMesh.AllAreas);
        return (hit);
    }
}
