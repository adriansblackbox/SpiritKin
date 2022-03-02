using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI_Manager : MonoBehaviour
{

    //SEPARATE SCRIPT (SHRINE CONTROLLER)
        //Split shrines into 4 quadrants
            //Each quadrant can hold at most 1/2 of total enemies
                //if a quadrant has more than 1/2 of total enemies select enough enemies at random to where its below 1/2
                    //if enemy is not current chasing player change the randomly selected enemies to relocating state
                        //send them to a neighboring quadrant for ease of use    

    private Transform enemiesContainer;
    public List<Vector3> surroundSpots = new List<Vector3>();
    public List<bool> surroundSpotAvailability = new List<bool>();
    private List<Vector3> surroundTrackingSpots = new List<Vector3>();
    public Transform Player;
    public float surroundRadius;

    public GameObject attackingEnemy;
    public List<GameObject> enemiesReadyToAttack = new List<GameObject>();

    private void Start() 
    {
        enemiesContainer = transform.GetChild(0);
        Player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        //Goal:
            //basic framework for selecting an enemy to attack
                //-> random !!!
                //-> queue system
                //-> order they arrive in

        if (attackingEnemy == null && enemiesReadyToAttack.Count > 0)
        {
            //select enemy + set enemy values to be ready to attack
                //EnemyMotion -> Waiting
                //EnemyAttack -> Attacking
            int ind = UnityEngine.Random.Range(0, enemiesReadyToAttack.Count - 1);
            attackingEnemy = enemiesReadyToAttack[ind];

            while(attackingEnemy == null){ // Tries to empty the list of invalid gameobjects, if we land on one.
                if(enemiesReadyToAttack.Count <= 0){
                    return;
                }

                enemiesReadyToAttack.RemoveAt(ind);
                ind = UnityEngine.Random.Range(0, enemiesReadyToAttack.Count - 1);
                attackingEnemy = enemiesReadyToAttack[ind];
            }

            attackingEnemy.GetComponent<Enemy_Controller>().EnemyMotion = Enemy_Controller.MotionState.Waiting;
            attackingEnemy.GetComponent<Enemy_Controller>().EnemyAttack = Enemy_Controller.AttackState.Attacking;
            Debug.Log("Selected Enemy");
        }

        //need to handle allocating an attacker -> ask adrian how he feels about this :D
            //first pass/first draft -> 1 enemy to attack + return to its spot
            //second pass -> 1 enemy to attack + stay engaged attacking player
            //third pass -> 1 main enemy attacking + staying engaged with player + 1 or 2 other enemies poking to make combat more difficult
    }

    public void finishAttack() //first pass super simple + will be used later for poke attacks
    {
        //return to spot
            //-> should be able to reset their state to surrounding
        attackingEnemy.GetComponent<Enemy_Controller>().EnemyAttack = Enemy_Controller.AttackState.NotAttacking;
        attackingEnemy.GetComponent<Enemy_Controller>().EnemyMotion = Enemy_Controller.MotionState.Surrounding;
        attackingEnemy = null;
        Debug.Log("Poke attack finished");
    }

            
    //each location should be equally spaced around the leading enemy
        //the leading enemy is the first enemy in the chasing list
            //every other enemy will not run directly at the player and will rather attempt to flank or surround player
                //need to account for the leading enemy when generating locations
    public void generateSurroundLocations()
    {
        //on the perimeter of a cirle around the player
        //-> Generate the circle
            //select x locations where x is the number of enemies
        float spacing = 360 / 8;
        //Debug.Log("Amount of Enemies: " + enemiesContainer.childCount);
        for (int i = 0; i < 8; ++i) //8 is for testing and can be changed later
        {
            
            //should output z equivalent for our circle
            float zVal = Mathf.Sin(Mathf.Deg2Rad * (spacing * i));

            //should output x equivalent for our circle
            float xVal = Mathf.Cos(Mathf.Deg2Rad * (spacing * i));

            //Debug.Log("Coordinate Pair: " + "( " + xVal + ", " + zVal + ")");

            Vector3 validSpot = new Vector3(xVal, 0, zVal);

            surroundSpots.Add(validSpot * surroundRadius);
            surroundSpotAvailability.Add(true);
            surroundTrackingSpots.Add(validSpot * surroundRadius * 1.5f);
        }
    }


    //loop through all spots
        //if spot isnt taken
            //calculate distance
                //store spot with least distance
    //if found a spot            
        //move to spot with least distance
    //else
        //go idle for now, but later will make new state


    //need a way to chekc if a spot is already taken
        //find the closest spot to the enemy
            //set their surround target to be that spot
    public List<Vector3> determineSurroundSpot(Transform enemy) 
    {
        float minDistA = Mathf.Infinity;
        float minDistB = Mathf.Infinity;
        int targetIndex = 100;
        for (int i = 0; i < surroundSpots.Count; i++)
        {
            if (surroundSpotAvailability[i])
            {
                var distance = Vector3.Distance(enemy.position, Player.position + surroundSpots[i]);
                if (distance < minDistA)
                {
                    targetIndex = i;
                    minDistA = distance;
                }
            }
        }
        
        if (targetIndex != 100) //we have found a spot
        {
            // Debug.Log("Found a spot: " + surroundSpots[targetIndex]);
            // Debug.Log("Surround Spot Availability: " + surroundSpotAvailability[targetIndex]);
            // Debug.Log("Surround Spot Count: " + surroundSpotAvailability.Count);
            surroundSpotAvailability[targetIndex] = false;
            enemy.GetComponent<Enemy_Controller>().surroundSpot = surroundSpots[targetIndex];
            enemy.GetComponent<Enemy_Controller>().surroundIndex = targetIndex;

            minDistA = Mathf.Infinity;
            minDistB = Mathf.Infinity;
            int targetIndexA = -1;
            int targetIndexB = -1;
            Vector3 targSpot = Vector3.zero;
            List<Vector3> Path = new List<Vector3>();
            Vector3 end = surroundSpots[targetIndex];
            Vector3 curr = Vector3.zero;
            Vector3 next = Vector3.zero;

            // Produce the first node on the surrounding areas that the enemy will visit to get them onto the ring
            for (int i = 0; i < surroundTrackingSpots.Count; i++)
            {
                var distance = Vector3.Distance(enemy.position, Player.position + surroundTrackingSpots[i]);
                if (distance < minDistA)
                {
                    minDistB = minDistA;
                    targetIndexB = targetIndexA;
                    minDistA = distance;
                    targetIndexA = i;
                }
                else if (distance < minDistB)
                {
                    minDistB = distance;
                    targetIndexB = i;
                }
            }
            //we have our two closest points
                //check which one is closer to the end spot
                //add that spot to the path as the first move
            if (Vector3.Distance(end, surroundTrackingSpots[targetIndexA]) < Vector3.Distance(end, surroundTrackingSpots[targetIndexB]))
            {
                curr = surroundTrackingSpots[targetIndexA];
                targetIndexB = -1;
            }
            else
            {
                curr = surroundTrackingSpots[targetIndexB];
                targetIndexA = -1;
            }
            Path.Add(curr);

            while (curr != surroundTrackingSpots[targetIndex]) //we are the tracking spot that links to our end spot
            {
                if (targetIndexA != -1) //A was chosen
                {
                    if (Vector3.Distance(end, surroundTrackingSpots[targetIndexA + 1]) < Vector3.Distance(end, surroundTrackingSpots[targetIndexA - 1]))
                        curr = surroundTrackingSpots[targetIndexA + 1];
                    else
                        curr = surroundTrackingSpots[targetIndexA - 1];             
                }
                else //B was chosen
                {
                    if (Vector3.Distance(end, surroundTrackingSpots[targetIndexB + 1]) < Vector3.Distance(end, surroundTrackingSpots[targetIndexB - 1]))
                        curr = surroundTrackingSpots[targetIndexB + 1];
                    else
                        curr = surroundTrackingSpots[targetIndexB - 1];
                }
                Path.Add(curr);
            }
            Path.Add(end);
            return Path;
            //Path
                //-> Front to back
                //where we start the path at the front and remove values until we get to the destination at the back

                //-> Back to front
                //where we start the path at the back and remove values until we reach the destination at the front

            //targSpot -> final tracking spot
            //end -> surroundSpot Location
        }
        else //sadge no spot for me
        {
            Debug.Log("No spot for me");
            return new List<Vector3>{};
        }
    }

    public Vector3 determineSurroundSpotV3(Transform enemy) 
    {
        float minDist = Mathf.Infinity;
        int targetIndex = 100;
        for (int i = 0; i < surroundSpots.Count; i++)
        {
            if (surroundSpotAvailability[i])
            {
                var distance = Vector3.Distance(enemy.position, Player.position + surroundSpots[i]);
                if (distance < minDist)
                {
                    targetIndex = i;
                    minDist = distance;
                }
            }
        }

        if (targetIndex != 100) //we have found a spot
        {
            Debug.Log("Found a spot");
            surroundSpotAvailability[targetIndex] = false;
            return surroundSpots[targetIndex];
        }
        else //sadge no spot for me
        {
            Debug.Log("No spot for me");
            return Vector3.zero;
        }
    }

    // public Vector3 calculateSurroundSpotInWorld()
    // {
    //     //do a raycast to ensure not hitting a wall and adjust values
    //         //output exact spot for enemy
    // }


    public bool checkIfNeedRelocate(int quadrant)
    {
        //check if quadrant has > 50% of enemies if so return false
        if (enemiesContainer.childCount >= 4) {
            float numberOfEnemiesInSelectedQuadrant = 0;
            for (int i = 0; i < enemiesContainer.childCount; i++)
            {
                if (enemiesContainer.GetChild(i).GetComponent<Enemy_Controller>().getQuadrant() == quadrant)
                    numberOfEnemiesInSelectedQuadrant+=1;
            }
            if (numberOfEnemiesInSelectedQuadrant / (float) enemiesContainer.childCount > 0.5f)
                return true;
            else  
                return false;
        }
        return false;
    }

    public float checkPatrol(float baseChance)
    {
        //go through enemies
            //if > 50% are patroling special chance to go idle
            //else normal chance
        var enemyCount = enemiesContainer.childCount;
        float enemiesPatrolingCount = 0.0f;
        for (int i = 0; i < enemyCount; i++)
        {
            if (enemiesContainer.GetChild(i).GetComponent<Enemy_Controller>().EnemyMotion == Enemy_Controller.MotionState.Patroling) enemiesPatrolingCount += 1;
        }
        if (enemiesPatrolingCount / (float) enemyCount > 0.5f)
            return baseChance * 1.5f;
        return baseChance;
    }

    public float checkIdle(float baseChance)
    {
        //go through enemies
            //if < .25f are patroling special chance to go idle
            //else normal chance
        var enemyCount = enemiesContainer.childCount;
        float enemiesPatrolingCount = 0.0f;
        for (int i = 0; i < enemyCount; i++)
        {
            if (enemiesContainer.GetChild(i).GetComponent<Enemy_Controller>().EnemyMotion == Enemy_Controller.MotionState.Patroling) enemiesPatrolingCount += 1;
        }
        if (enemiesPatrolingCount / (float) enemyCount < 0.25f)
            return baseChance * 1.5f;
        else
            return baseChance;
    }
}
