using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Controller : MonoBehaviour
{
    //Waypoints (4-8 waypoints around the shrine)
        //more will ensure less obvious patrol paths
            //can generate paths rather than having them set

    //Idling enemy would have a smaller detection radius than that of a Patroling one

    //Patroling -> Idling
        //if less than 25% are patroling
            //Increased chance each interval
        //else
            //Standard chance each interval

    //Idling -> Patroling
        //if less than 50% of enemies are patroling
            //Standard chance each interval
        //else
            //Stay idle


    //ADD HITSTUN STATE
    public enum MotionState {
        Patroling,
        Idling,
        Alerted,
        Chasing,
        Returning,
        Relocating
    }

    //Enemies will surround player
        //They will attack once at a time

    //ADD HITSTUN STATE
    public enum AttackState {
        Attacking,
        Surrounding,
        NotAttacking,
    }

    public MotionState EnemyMotion;
    public AttackState EnemyAttack;

    public NavMeshAgent ThisEnemy;
    public NavMeshPath path;
    public GameObject player;

    public float patrolToIdleChance;
    public float idleToPatrolChance;
    public float swapStateInterval;

    private float myTime;

    private Transform shrine;

    //ENEMY VARIABLES
    [SerializeField] int quadrant;

    private int timesPatroled;

    private Shrine_Controller sc;

    // Start is called before the first frame update
    void Start()
    {
        path = new UnityEngine.AI.NavMeshPath();
        player = GameObject.FindWithTag("Player");
        ThisEnemy = GetComponent<UnityEngine.AI.NavMeshAgent>();
        EnemyMotion = MotionState.Idling;
        EnemyAttack = AttackState.NotAttacking;
        sc = transform.parent.parent.GetComponent<Shrine_Controller>();
        shrine = transform.parent.parent;
        determineQuadrant();
    }

    // Update is called once per frame
    void Update()
    {
        myTime += Time.deltaTime;
        switch (EnemyMotion)
        {
            case MotionState.Patroling:

                //GOAL IS TO HAVE THEM PATROL NOT JUST RANDOMLY WANDER (MIGHT BE WORTH TO EXPLORE WANDERING AT A LATER TIME)
                    //THEREFORE THE ENEMIES WOULD HAVE TO WALK PARALLEL WITH THE EDGES OF THE SHRINE OR IN A CIRCLE AROUND THE SHRINE OR JUST ENSURE THEY DONT WALK DIRECTLY AT THE SHRINE BECAUSE THAT DEFEATS THE PURPOSE

                //NECESSARY VARIABLES
                    //Destination Point
                    //Last Quadrant (WANT ENEMY TO MOVE QUADRANTS EVERY OTHER DESTINATION POINT TO MAKE IT FEEL LIKE THEY ARE ACTUALLY PATROLING AND WALKING AROUND THE SHRINE PROTECTING IT)

                //CONSIDERATIONS
                    //MAYBE ENEMY HAS TO PATROL TO AT LEAST 1 or 2 DESTINATION POINTS BEFORE THEY HAVE A CHANCE TO SWAP STATES
                
                if(ThisEnemy.remainingDistance < 0.01f){
                    ThisEnemy.SetDestination(findNextWaypoint());
                }                

                //CHECK IF NEED TO SWAP STATES
                // if (myTime > swapStateInterval)
                // {
                //     //check with Shrine_Controller if need to relocate
                //     float temp = Random.Range(0.0f, 1.0f);
                //     //ask shrine_controller if less than 25% are patroling
                //         //if yes have increased chance
                //     if (temp < patrolToIdleChance) //swap states
                //     {
                //         EnemyMotion = MotionState.Idling;
                //         Debug.Log("Now Idle");
                //     }
                //     myTime = 0;
                // }            
                break;
            case MotionState.Idling:

                //check if in idle range [0.25 * spawn range to 0.5 * spawn range]
                    //do nothing
                //else
                    //move to idle range
                        //select a random distance within the idle range
                if ((Vector3.Distance(shrine.position, transform.position) < shrine.GetComponent<Shrine>().shrineSpawnRange * 0.20 
                    || Vector3.Distance(shrine.position, transform.position) > shrine.GetComponent<Shrine>().shrineSpawnRange * 0.55)
                    && !ThisEnemy.hasPath)
                {
                    ThisEnemy.SetDestination(findIdleSpot());
                }

                //CHECK IF NEED TO SWAP STATES
                // if (myTime > swapStateInterval)
                // {
                //     //check with Shrine_Controller if need to relocate
                //     float temp = Random.Range(0.0f, 1.0f);
                //     if (temp < idleToPatrolChance) //swap states
                //     {
                //         EnemyMotion = MotionState.Patroling;
                //         Debug.Log("Now Patrol");
                //     }
                //     myTime = 0;
                // }
                break;
            case MotionState.Alerted:
                // Tether movement to player's, but reduce our movement speed. Keep turned towards the player. If player approaches for N seconds, Chasing state
                break;
            case MotionState.Chasing:
                ThisEnemy.CalculatePath(player.transform.position, path);
                if(path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                    //if(Vector3.distance(player.transform.position, shrine.transform.position)){
                    ThisEnemy.SetDestination(player.transform.position);
                    //}
                }
                else {}
                break;
            case MotionState.Returning:
                break;
            default:
                break;
        }
    }

    //FIRST ENEMY SPAWNED TAKES AN INDIRECT PATH TO THEIR LOCATION
    private Vector3 findIdleSpot()
    {
        if (sc.checkValidity(getQuadrant())) 
        {
            while(true){
                float upperLimitWaypoint = (float) shrine.GetComponent<Shrine>().shrineSpawnRange * 0.5f;
                float lowerLimitWaypoint = (float) shrine.GetComponent<Shrine>().shrineSpawnRange * 0.25f;

                float xpos = 0; 
                float zpos = 0; 

                if (quadrant == 1)
                {
                    xpos = Random.Range(shrine.position.x - lowerLimitWaypoint, shrine.position.x - upperLimitWaypoint);
                    zpos = Random.Range(shrine.position.z + lowerLimitWaypoint, shrine.position.z + upperLimitWaypoint);
                }
                else if (quadrant == 2)
                {
                    xpos = Random.Range(shrine.position.x + lowerLimitWaypoint, shrine.position.x + upperLimitWaypoint);
                    zpos = Random.Range(shrine.position.z + lowerLimitWaypoint, shrine.position.z + upperLimitWaypoint);
                }
                else if (quadrant == 3)
                {
                    xpos = Random.Range(shrine.position.x - lowerLimitWaypoint, shrine.position.x - upperLimitWaypoint);
                    zpos = Random.Range(shrine.position.z - lowerLimitWaypoint, shrine.position.z - upperLimitWaypoint);
                }
                else if (quadrant == 4)
                {
                    xpos = Random.Range(shrine.position.x + lowerLimitWaypoint, shrine.position.x + upperLimitWaypoint);
                    zpos = Random.Range(shrine.position.z - lowerLimitWaypoint, shrine.position.z - upperLimitWaypoint);
                }
                
                //ADJUST Y POS TO ALIGN WITH BLOCKOUT
                Vector3 point = new Vector3(xpos, 0.0f, zpos);

                ThisEnemy.CalculatePath(point, path);
                if(path.status == NavMeshPathStatus.PathComplete) { // Check if point is on navmesh
                    Debug.Log("TARGET LOCATION IS: " + point);
                    return point;
                }
            }
        }
        return new Vector3(0,0,0);        
    }

    //4 cases
        //Quadrant 1, or Upper Left
            //x is negative
            //z is positive
        //Quadrant 2, or Upper Right
            //x is positive
            //z is positive
        //Quadrant 3, or Lower Left
            //x is negative
            //z is negative
        //Quadrant 4, or Lower Right
            //x is positive
            //z is negative


    private Vector3 findNextWaypoint() 
    {
        if (timesPatroled == 0) //guaranteed to stay in current quadrant
        {
            Debug.Log("Is Quadrant " + getQuadrant() + " a valid location: " + sc.checkValidity(getQuadrant()));
            if (sc.checkValidity(getQuadrant())) 
            {
                while(true){
                    float upperLimitWaypoint = (float) shrine.GetComponent<Shrine>().shrineSpawnRange * 1.5f;
                    float lowerLimitWaypoint = (float) shrine.GetComponent<Shrine>().shrineSpawnRange * 0.5f;

                    float xpos = 0; 
                    float zpos = 0; 

                    if (quadrant == 1)
                    {
                        xpos = Random.Range(shrine.position.x - lowerLimitWaypoint, shrine.position.x - upperLimitWaypoint);
                        zpos = Random.Range(shrine.position.z + lowerLimitWaypoint, shrine.position.z + upperLimitWaypoint);
                    }
                    else if (quadrant == 2)
                    {
                        xpos = Random.Range(shrine.position.x + lowerLimitWaypoint, shrine.position.x + upperLimitWaypoint);
                        zpos = Random.Range(shrine.position.z + lowerLimitWaypoint, shrine.position.z + upperLimitWaypoint);
                    }
                    else if (quadrant == 3)
                    {
                        xpos = Random.Range(shrine.position.x - lowerLimitWaypoint, shrine.position.x - upperLimitWaypoint);
                        zpos = Random.Range(shrine.position.z - lowerLimitWaypoint, shrine.position.z - upperLimitWaypoint);
                    }
                    else if (quadrant == 4)
                    {
                        xpos = Random.Range(shrine.position.x + lowerLimitWaypoint, shrine.position.x + upperLimitWaypoint);
                        zpos = Random.Range(shrine.position.z - lowerLimitWaypoint, shrine.position.z - upperLimitWaypoint);
                    }
                    
                    //ADJUST Y POS TO ALIGN WITH BLOCKOUT
                    Vector3 point = new Vector3(xpos, 0.0f, zpos);

                    ThisEnemy.CalculatePath(point, path);
                    Debug.Log("Distance to Waypoint: " + Vector3.Distance(transform.position, point));
                    if(path.status == NavMeshPathStatus.PathComplete && Vector3.Distance(transform.position, point) > 40f) { // Check if point is on navmesh
                        Debug.Log("TARGET LOCATION IS: " + point);
                        return point;
                    }
                }
            }
        }
        // else if (timesPatroled == 1) //50% chance to move quadrants
        // {

        // }
        // else //guaranteed to move quadrants
        // {

        // }


        //select a quadrant for next waypoint
            //if enemy has not yet reached a waypoint in its current quadrant
                //select its current quadrant and check with shrine controller that its okay to stay
                    //if its okay to stay get a random location > 50 (adjustable) units from the enemy and return the Vector3
                    //else select one of the neighboring quadrants for its next waypoint & repeat check with shrine controller
                        //if its okay get a random location > 50 (adjustable) units from the enemy and return the Vector3
                        //else choose its other neighbor that wasn't chosen and get a random location > 50 (adjustable) units from the enemy and return the Vector3
        return new Vector3(0,0,0);
    }

    
    private void determineQuadrant()
    {
        if (transform.position.x < shrine.position.x && transform.position.z > shrine.position.z) //Quadrant 1
        {
            Debug.Log("Current Quadrant is 1");
            quadrant = 1;
        }
        else if (transform.position.x > shrine.position.x && transform.position.z > shrine.position.z) //Quadrant 2
        {
            Debug.Log("Current Quadrant is 2");
            quadrant = 2;
        }
        else if (transform.position.x < shrine.position.x && transform.position.z < shrine.position.z) //Quadrant 3
        {
            Debug.Log("Current Quadrant is 3");
            quadrant = 3;
        }
        else if (transform.position.x > shrine.position.x && transform.position.z < shrine.position.z) //Quadrant 4
        {
            Debug.Log("Current Quadrant is 4");
            quadrant = 4;
        }
    }

    public void setQuadrant(int setQuadVal)
    {
        quadrant = setQuadVal;
    }

    public int getQuadrant()
    {
        return quadrant;
    }
}
