using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Spawner : MonoBehaviour
{
    [SerializeField] int totalShrinesCursed = 0;
    [SerializeField] int lowerLimitEnemyCount = 1;
    [SerializeField] int upperLimitEnemyCount = 4;

    public GameObject nonCursedContainer;
    public GameObject cursedContainer;
    public GameObject enemyPrefab;

    public int totalShrines;

    public float spawnInterval; //enemies will spawn in at intervals at shrines to show that they are slowly drawing in ghosts & to make feel more natural
    public int currentCursedShrines; //how many shrines are currently cursed

    private float myTime;
    public float shrineInterval = 45f;
    private bool firstSpawn = true;

    public void Start()
    {
        scaleDifficulty();
    }

    public void Update()
    {
        if(!FindObjectOfType<MainHub>().playerInHub)
            myTime += Time.deltaTime;

        if (firstSpawn)
        {
            if (myTime > 5f)
            {
                firstSpawn = false;
                myTime = 0;
                if (nonCursedContainer.transform.childCount > 0) //every 15 seconds -> actually 45 to 60 seconds is probably better
                {
                    int temp = Random.Range(0, nonCursedContainer.transform.childCount);
                    Transform shrine = nonCursedContainer.transform.GetChild(temp);
                    shrine.parent = cursedContainer.transform;
                    shrine.GetComponent<Shrine>().cursed = true;
                    shrine.GetComponent<Shrine>().CurCurseTime = 0f;
                    currentCursedShrines++;
                    scaleDifficulty();
                    shrine.GetComponent<Shrine>().setEnemiesToSpawn();
                }
            }
        }
        else if (myTime > shrineInterval) 
        {
            myTime = 0;
            if (nonCursedContainer.transform.childCount > 0) //every 15 seconds -> actually 45 to 60 seconds is probably better
            {
                int temp = Random.Range(0, nonCursedContainer.transform.childCount);
                Transform shrine = nonCursedContainer.transform.GetChild(temp);
                shrine.parent = cursedContainer.transform;
                shrine.GetComponent<Shrine>().cursed = true;
                shrine.GetComponent<Shrine>().CurCurseTime = 0f;
                currentCursedShrines++;
                scaleDifficulty();
                shrine.GetComponent<Shrine>().setEnemiesToSpawn();
            }
        }
    }

    //increments limits to help in scaling difficulty
    private void scaleDifficulty() 
    {
        totalShrinesCursed++;
        if (totalShrinesCursed % 2 == 1) //lower limit increments every other round
            lowerLimitEnemyCount++;
        else if (totalShrinesCursed != 2 && totalShrinesCursed % 2 == 0 && upperLimitEnemyCount - lowerLimitEnemyCount == 1) //upper limit increments every round after the lower limit increments
            upperLimitEnemyCount++;
        selectShrineEnemyCount();
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

    //THIS DOESN'T WORKvvvvvvvvvvvvvvvvv
    //
    // public NavMeshHit chooseRelocation(Transform shrine)
    // {
    //     var shrineScript = shrine.GetComponent<Shrine>();
    //     NavMeshHit hit;
    //     Vector3 rPoint = shrine.position + (Random.insideUnitSphere * shrineScript.shrineSpawnRange * 2);
    //     rPoint.y = shrine.position.y;

    //     NavMesh.SamplePosition(rPoint, out hit, 40.0f, NavMesh.AllAreas);
    //     return (hit);
    // }
}
