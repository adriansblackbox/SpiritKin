using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Spawner : MonoBehaviour
{
    [SerializeField] int currentRound = 0;
    [SerializeField] int lowerLimitEnemyCount = 1;
    [SerializeField] int upperLimitEnemyCount = 4;

    public GameObject nonCursedContainer;
    public GameObject cursedContainer;
    public GameObject enemyPrefab;

    public float enemyNoSpawnRadius; //ensure distance from current enemies to desired spawn point is > than enemyNoSpawnRadius
    public float spawnInterval; //enemies will spawn in at intervals at shrines to show that they are slowly drawing in ghosts & to make feel more natural
    public int currentCursedShrines; //how many shrines are currently cursed
    public int maxCursedShrines; //how many shrines max do we want to allow to be cursed at one time

    private float myTime;
    private int shrineCount;
    public float shrineInterval = 15f;

    public void Start()
    {
        shrineCount = nonCursedContainer.transform.childCount;
        nextRound();
    }

    public void Update()
    {
        myTime += Time.deltaTime;
        if (myTime > shrineInterval && nonCursedContainer.transform.childCount > 0) //every 15 seconds
        {
            int temp = Random.Range(0, nonCursedContainer.transform.GetChild(0).childCount);
            Transform shrine = nonCursedContainer.transform.GetChild(temp);
            shrine.parent = cursedContainer.transform;
            shrine.GetComponent<Shrine>().cursed = true;
            currentCursedShrines++;
            Debug.Log("Shrine located at: (" + shrine.position.x + ", " + shrine.position.z + ") has been activated");
            myTime = 0;
        }
    }

    //call when a new round starts to handle spawning of enemies
    public void nextRound()
    {
        currentRound++;
        Debug.Log("Current Round: " + currentRound);
        calculateEnemyLimits();
        selectShrineEnemyCount();
        //removeRemainingEnemies(); remove any remaining enemies function [player should have killed all of them for round to end]
        
        //spawnEnemies();
    }

    //increments limits to help in scaling difficulty
    private void calculateEnemyLimits() 
    {
        if (currentRound % 2 == 1) //lower limit increments every other round
            lowerLimitEnemyCount++;
        else if (currentRound != 2 && currentRound % 2 == 0 && upperLimitEnemyCount - lowerLimitEnemyCount == 1) //upper limit increments every round after the lower limit increments
            upperLimitEnemyCount++;
    }

    private void selectShrineEnemyCount()
    {
        for (int i = 0; i < shrineCount; i++)
        {
            Transform shrine = nonCursedContainer.transform.GetChild(i);
            shrine.GetComponent<Shrine>().enemiesToSpawnWhenCursed = Random.Range(lowerLimitEnemyCount, upperLimitEnemyCount + 1);
        }
    }

    public void spawnEnemy(GameObject shrineToSpawnAt)
    {
        //find a randomized location for the enemy to spawn
        Vector3 enemyPosition = chooseLocation(shrineToSpawnAt.transform).position;
        //spawn in enemy
        GameObject enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        enemy.GetComponent<Enemy_Controller>().sc = shrineToSpawnAt.GetComponent<Shrine_Controller>();
        enemy.GetComponent<Enemy_Controller>().shrine = shrineToSpawnAt.transform;
        enemy.GetComponent<Enemy_Controller>().shrineSpawnRange = shrineToSpawnAt.GetComponent<Shrine>().shrineSpawnRange;
        
        //put in enemy container
        enemy.transform.parent = shrineToSpawnAt.transform.GetChild(0);

        //increment enemies spawned at shrine
        shrineToSpawnAt.GetComponent<Shrine>().amountAlreadySpawned++;
        enemy.GetComponent<Enemy_Controller>().enabled = true;        
    }

    //Spawns all enemies for a round around each shrine
    //(STATIC SPAWNING CAN BE DONE ELSEWHERE THIS IS TOTALLY PROCEDURAL)
    private void spawnEnemies() 
    {
        //go through each shrine
        for (int i = 0; i < shrineCount; i++)
        {
            //current shrine
            Transform shrine = nonCursedContainer.transform.GetChild(i);
            //number of enemies that should be at current shrine
            int enemyCount = Random.Range(lowerLimitEnemyCount, upperLimitEnemyCount + 1);
            Debug.Log("At least " + enemyCount + " enemies should be at shrine located at (" + shrine.position.x + ", " + shrine.position.z + ")");
            //if not enough enemies at shrine spawn more
            if (shrine.GetChild(0).childCount < enemyCount)
            {
                int numberOfEnemiesToBeSpawned = enemyCount - shrine.GetChild(0).childCount;
                //spawn each enemy at a valid location
                for (int j = 0; j < numberOfEnemiesToBeSpawned; j++)
                {
                    //choose a random location in the range around the shrine
                    Vector3 enemyPosition = chooseLocation(shrine).position;
                    //spawn in enemy
                    GameObject enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
                    //put in enemy container
                    enemy.transform.parent = shrine.GetChild(0);
                }
            }
        }       
    }

    //CURRENTLY ONLY THE X AND Z VALUES ARE CALCULATED SO THIS WILL HAVE TO BE UPDATED TO WORK WITH Y VALUES ONCE WE HAVE THE MAP IMPLEMENTED
    private NavMeshHit chooseLocation(Transform shrine)
    {
        var shrineScript = shrine.GetComponent<Shrine>();
        NavMeshHit hit;
        Vector3 rPoint = shrine.position + (Random.insideUnitSphere * shrineScript.shrineSpawnRange);

        NavMesh.SamplePosition(rPoint, out hit, 20.0f, NavMesh.AllAreas);
        return (hit);
    }
}
