using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI_Manager : MonoBehaviour
{

    private Transform enemiesContainer;
    public List<Vector3> surroundSpots = new List<Vector3>();
    public List<bool> surroundSpotAvailability = new List<bool>();
    private List<Vector3> surroundTrackingSpots = new List<Vector3>();
    public Transform Player;
    public float surroundRadius;
    public float selectAttackerTimer;

    public GameObject attackingEnemy;
    public List<GameObject> enemiesReadyToAttack = new List<GameObject>();
    public List<GameObject> enemiesIdling = new List<GameObject>();

    private void Start() 
    {
        enemiesContainer = transform.GetChild(0);
        Player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        selectAttackerTimer += Time.deltaTime;
        //Goal:
            //basic framework for selecting an enemy to attack
                //-> random !!!
                //-> queue system
                //-> order they arrive in

        if (selectAttackerTimer > 0.75f)
        {
            selectAttackerTimer = 0;
            if (attackingEnemy == null && enemiesReadyToAttack.Count > 0)
            {
                //select enemy + set enemy values to be ready to attack
                    //EnemyMotion -> Waiting
                    //EnemyAttack -> Attacking
                int ind = UnityEngine.Random.Range(0, enemiesReadyToAttack.Count - 1);
                if (enemiesReadyToAttack[ind] != null) 
                {
                    attackingEnemy = enemiesReadyToAttack[ind];
                    attackingEnemy.GetComponent<Enemy_Controller>().EnemyMotion = Enemy_Controller.MotionState.Waiting;
                    attackingEnemy.GetComponent<Enemy_Controller>().ThisEnemy.ResetPath();
                    attackingEnemy.GetComponent<Enemy_Controller>().movementQueue.Clear();
                    attackingEnemy.GetComponent<Enemy_Controller>().EnemyAttack = Enemy_Controller.AttackState.Attacking;
                    Debug.Log("Selected Enemy");
                }
                else
                {
                  enemiesReadyToAttack.RemoveAt(ind);
                }
            }
        }
    }

    public List<GameObject> getNearbyEnemies(GameObject enemy)
    {
        List<GameObject> enemiestoAlert = new List<GameObject>();
        for (int i = 0; i < enemiesIdling.Count; i++)
        {
            if (Vector3.Distance(enemiesIdling[i].transform.position, enemy.position) < 60f)
            {
                enemiestoAlert.Add(enemiesIdling[i].gameObject);
            }
        }
        return enemiesToAlert; 
    }

    public void generateSurroundLocations()
    {
        //on the perimeter of a cirle around the player
        //-> Generate the circle
            //select x locations where x is the number of enemies
        float spacing = 360 / 8;
        for (int i = 0; i < 8; ++i) //8 is for testing and can be changed later
        {
            
            //should output z equivalent for our circle
            float zVal = Mathf.Sin(Mathf.Deg2Rad * (spacing * i));

            //should output x equivalent for our circle
            float xVal = Mathf.Cos(Mathf.Deg2Rad * (spacing * i));

            Vector3 validSpot = new Vector3(xVal, 0, zVal);

            //setup all 3 variables to the end of our 3 lists
            surroundSpots.Add(validSpot * surroundRadius);
            surroundSpotAvailability.Add(true);
            surroundTrackingSpots.Add(validSpot * surroundRadius * 1.5f);
        }
    }

    public List<Vector3> determineSurroundSpot(Transform enemy)
    {
        float minDistA = Mathf.Infinity;
        float minDistB = Mathf.Infinity;
        int targetIndex = -1;
        float distance = 0.0f;
        for (int i = 0; i < surroundSpots.Count; i++)
        {
            if (surroundSpotAvailability[i])
            {
                distance = Vector3.Distance(enemy.position, Player.position + surroundSpots[i]);
                if (distance < minDistA)
                {
                    targetIndex = i;
                    minDistA = distance;
                }
            }
        }
        
        if (targetIndex >= 0) //we have found a spot
        {
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
            distance = 0.0f;
            // Produce the first node on the surrounding areas that the enemy will visit to get them onto the ring
            for (int i = 0; i < surroundTrackingSpots.Count; i++)
            {
                distance = Vector3.Distance(enemy.position, Player.position + surroundTrackingSpots[i]);
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
            
            //was A or B closer for the enemy + add to path
            int chosenIndex = (targetIndexA != -1) ? targetIndexA : targetIndexB;

            //check if surroudn spot is closer than tracking spot
            if (Vector3.Distance(enemy.position, curr + Player.position) < Vector3.Distance(enemy.position, end + Player.position) - 2f)
            {
                Path.Add(curr);
            }
            else
            {
                Path.Add(end);
                return Path;
            }
                
            //generate whole path based off of starting position and end position
            bool right = false;

            for (int i = chosenIndex + 1; i < surroundTrackingSpots.Count / 2 + chosenIndex + 1; i++)
            {
                if (i%surroundTrackingSpots.Count == targetIndex) // we are done and can go in positive direction
                {
                    right = true;
                    break;
                }
            }

            if (right) //add to path in postive direction until reach goal
            {
                for (int i = chosenIndex + 1; i < surroundTrackingSpots.Count / 2 + chosenIndex + 1 && i % surroundTrackingSpots.Count != (targetIndex + 1) % surroundTrackingSpots.Count; i++)
                    Path.Add(surroundTrackingSpots[i % surroundTrackingSpots.Count]);        
            }
            else //add to path in negative direction until reach goal
            {
                for (int i = chosenIndex + surroundTrackingSpots.Count - 1; i > surroundTrackingSpots.Count / 2 + chosenIndex - 1 && i % surroundTrackingSpots.Count != (targetIndex + surroundTrackingSpots.Count - 1) % surroundTrackingSpots.Count; i--)
                    Path.Add(surroundTrackingSpots[i % surroundTrackingSpots.Count]);
            }
            //make the decision
            Path.Add(end);

            return Path;
        }
        else //sadge no spot for me
        {
            Debug.Log("No spot for me");
            return null;
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

    //No longer needed, was too complex for our needs

    /*
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
    */
}
