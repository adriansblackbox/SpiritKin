using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    public int shrineSpawnRange; //range around the shrine where enemies can be spawned
    public int enemyNoSpawnRadius; //ensure distance from current enemies to desired spawn point is > than enemyNoSpawnRadius
    public int clusterChance; //chance that the enemy that spawns is a cluster of 2 or 3 (80% 2 enemies, 20% 3 enemies)

    [SerializeField] int currentRound = 1;
    [SerializeField] int lowerLimitEnemyCount = 2;
    [SerializeField] int upperLimitEnemyCount = 4;

    public GameObject shrineContainer;
    public GameObject enemyPrefab;

    //call when a new round starts to handle spawning of enemies
    public void nextRound() 
    {
        currentRound++;
        Debug.Log("Current Round: " + currentRound);
        calculateEnemyLimits();
        spawnEnemies();
    }

    //increments limits to help in scaling difficulty
    private void calculateEnemyLimits() 
    {
        if (currentRound % 2 == 1) //lower limit increments every other round
        {
            lowerLimitEnemyCount++;
        }
        else if (currentRound != 2 && currentRound % 2 == 0 && upperLimitEnemyCount - lowerLimitEnemyCount == 1) //upper limit increments every round after the lower limit increments
        {
            upperLimitEnemyCount++;
        }
    }

    //Spawns all enemies for a round around each shrine
    //(STATIC SPAWNING CAN BE DONE ELSEWHERE THIS IS TOTALLY PROCEDURAL)
    private void spawnEnemies() 
    {
        int shrineCount = shrineContainer.transform.childCount;
        //go through each shrine
        for (int i = 0; i < shrineCount; i++)
        {
            //current shrine
            Transform shrine = shrineContainer.transform.GetChild(i);
            //number of enemies that should be at current shrine
            int enemyCount = Random.Range(lowerLimitEnemyCount, upperLimitEnemyCount + 1);
            Debug.Log("At least " + enemyCount + " enemies should be at shrine located at (" + shrine.position.x + ", " + shrine.position.y + ")");
            //if not enough enemies at shrine spawn more
            if (shrine.GetChild(0).childCount < enemyCount)
            {
                int numberOfEnemiesToBeSpawned = enemyCount - shrine.GetChild(0).childCount;
                //spawn each enemy at a valid location
                for (int j = 0; j < numberOfEnemiesToBeSpawned; j++)
                {
                    //choose a random location in the range around the shrine
                    Vector3 enemyPosition = chooseLocation(shrine);
                    //spawn in enemy
                    GameObject enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
                    //put in enemy container
                    enemy.transform.parent = shrine.GetChild(0);

                    // //decide whether or not they are going to be a cluster
                    // if (Random.Range(0.0f, 1.0f) > 0.2f) //non-cluster 80%
                    // {
                    //     //instantiate single enemy at location
                    // }
                    // else //cluster 20%
                    // {
                        //instantiate a cluster of 2 or 3
                    // }
                }
            }
        }       
    }

    private Vector3 chooseLocation(Transform shrine)
    {
        while (true) 
        {
            float xPos = Random.Range(shrine.position.x - shrineSpawnRange, shrine.position.x + shrineSpawnRange);
            float zPos = Random.Range(shrine.position.z - shrineSpawnRange, shrine.position.z + shrineSpawnRange);
            Vector3 test = new Vector3(xPos, 0.0f, zPos);
            if (shrine.GetChild(0).childCount == 0) //if no enemies then location is valid
            {
                Debug.Log("ENEMY ATTEMPTING TO BE SPAWNED IN AT: (" + xPos + ", " + zPos + ")");
                return (test);
            }
            else //check current enemies
            {
                //go through each enemy already spawned
                int currentEnemies = shrine.GetChild(0).childCount;
                bool validLocation = true;
                for (int i = 0; i < currentEnemies; i++)
                {
                    Transform enemy = shrine.GetChild(0).GetChild(i);
                    //check if any enemies are too close
                    if (Vector3.Distance(test, enemy.position) < enemyNoSpawnRadius)
                        validLocation = false;
                }
                //if no enemies were too close return the spawn location
                if (validLocation)
                {
                    Debug.Log("ENEMY ATTEMPTING TO BE SPAWNED IN AT: (" + xPos + ", " + zPos + ")");
                    return (test); 
                }
            }
        }
    }
}
