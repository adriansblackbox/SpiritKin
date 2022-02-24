using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Transform Player;

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
        float spacing = 360 / enemiesContainer.childCount;
        Debug.Log("Amount of Enemies: " + enemiesContainer.childCount);
        for (int i = 0; i < enemiesContainer.childCount; ++i)
        {
            
            //should output z equivalent for our circle
            float zVal = Mathf.Sin(Mathf.Deg2Rad * (spacing * i));

            //should output x equivalent for our circle
            float xVal = Mathf.Cos(Mathf.Deg2Rad * (spacing * i));

            Debug.Log("Coordinate Pair: " + "( " + xVal + ", " + zVal + ")");

            Vector3 validSpot = new Vector3(xVal, 0, zVal);
            validSpot += Player.position;

            surroundSpots.Add(validSpot);
        }
    }


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
