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
    private List<Vector3> surroundSpots = new List<Vector3>();
    private List<bool> surroundSpotAvailability = new List<bool>();
    private List<Vector3> surroundTrackingSpots = new List<Vector3>();
    public Transform Player;
    public float surroundRadius;

    private void Start() 
    {
        enemiesContainer = transform.GetChild(0);
        Player = GameObject.Find("Player").transform;
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
        Debug.Log("Amount of Enemies: " + enemiesContainer.childCount);
        for (int i = 0; i < 8; ++i) //8 is for testing and can be changed later
        {
            
            //should output z equivalent for our circle
            float zVal = Mathf.Sin(Mathf.Deg2Rad * (spacing * i));

            //should output x equivalent for our circle
            float xVal = Mathf.Cos(Mathf.Deg2Rad * (spacing * i));

            Debug.Log("Coordinate Pair: " + "( " + xVal + ", " + zVal + ")");

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

            minDist = Mathf.Infinity;
            Vector3 targSpot = Vector3.zero;
            List<Vector3> Path = new List<Vector3>();
            Vector3 curr = surroundSpots[targetIndex];
            Vector3 next = Vector3.zero;

            // Produce the first node on the surrounding areas that the enemy will visit to get them onto the ring
            for (int i = 0; i < surroundTrackingSpots.Count; i++)
            {
                if (surroundSpotAvailability[i])
                {
                    var distance = Vector3.Distance(enemy.position, Player.position + surroundTrackingSpots[i]);
                    if (distance < minDist)
                    {
                        targSpot = surroundTrackingSpots[i];
                        minDist = distance;
                    }
                } 
            }

            while(curr != targSpot) {
                Path.Add(curr);
                for (int i = 0; i < surroundTrackingSpots.Count; i++)
                {
                    if (surroundSpotAvailability[i])
                    {
                        var distance = Vector3.Distance(curr, surroundTrackingSpots[i]);
                        if (distance < minDist)
                        {
                            next = surroundTrackingSpots[i];
                            minDist = distance;
                        }
                    } 
                }
                curr = next;
            }
            Path.Add(targSpot);
            return Path;
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
