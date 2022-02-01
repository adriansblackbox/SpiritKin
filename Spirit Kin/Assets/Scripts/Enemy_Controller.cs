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
    public GameObject alertBox;

    public float patrolToIdleChance;
    public float idleToPatrolChance;
    public float swapStateInterval;
    public float returnDist;

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
        EnemyMotion = MotionState.Chasing;
        EnemyAttack = AttackState.NotAttacking;
        sc = transform.parent.parent.GetComponent<Shrine_Controller>();
        shrine = transform.parent.parent;
        determineQuadrant();
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemyMotion == MotionState.Alerted)
        {
            alertBox.SetActive(true);
        } else {
            alertBox.SetActive(false);
        }

        Debug.Log(EnemyMotion);

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
                
                if (ThisEnemy.remainingDistance <= ThisEnemy.stoppingDistance + 0.01f) {
                    float temp = Random.Range(0.0f, 1.0f);
                    //if > 50% patroling increase chance to swap
                    if (temp < sc.checkPatrol(patrolToIdleChance) && timesPatroled > 2) //swap states
                    {
                        EnemyMotion = MotionState.Idling;
                        timesPatroled = 0;
                        Debug.Log("Now " + EnemyMotion);
                    } else {
                        ThisEnemy.SetDestination(findNextWaypoint());
                    }
                    timesPatroled++;
                }                   
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

                //DO WE NEED TO RELOCATE?

                if (myTime > swapStateInterval) {
                    if (ThisEnemy.remainingDistance <= ThisEnemy.stoppingDistance + 0.01f)
                    {
                        float temp = Random.Range(0.0f, 1.0f);
                        //if < 25% patroling increase chance to swap
                        if (temp < sc.checkIdle(idleToPatrolChance)) //swap states
                        {
                            EnemyMotion = MotionState.Patroling;
                            Debug.Log("Now " + EnemyMotion);
                        }                    
                    }
                    myTime = 0.0f;
                }
                break;
            case MotionState.Alerted:
                // Tether movement to player's, but reduce our movement speed. Keep turned towards the player. If player approaches for N seconds, Chasing state
                
                //possibly IEnumerator to decide next state
                    //enter alerted
                    //after x seconds decide which next state is best
                
                //transform.LookAt(player.transform);
                break;
            case MotionState.Chasing:
                ThisEnemy.CalculatePath(player.transform.position, path);
                if (path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                    if (Vector3.Distance(transform.position, shrine.transform.position) < returnDist){
                        ThisEnemy.SetDestination(player.transform.position);
                    } else {
                        EnemyMotion = MotionState.Alerted;
                        ThisEnemy.ResetPath();
                    }
                }
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
        //if we need to relocate select new quadrant
        if (sc.checkIfNeedRelocate(quadrant)) 
        {
            float neighbor = Random.Range(0.0f, 1.0f);

            switch (quadrant)
            {
                case 1:
                    if (neighbor > 0.5f) //go right
                        quadrant = 2;
                    else //go down
                        quadrant = 3;
                    break;
                case 2:
                    if (neighbor > 0.5f) //go right
                        quadrant = 1;
                    else //go down
                        quadrant = 4;
                    break;
                case 3:
                    if (neighbor > 0.5f) //go right
                        quadrant = 4;
                    else //go down
                        quadrant = 1;
                    break;
                case 4:
                    if (neighbor > 0.5f) //go right
                        quadrant = 3;
                    else //go down
                        quadrant = 2;
                    break;
            }
        } 
        while(true) {
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
                return point;
            }
        }      
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
        if (true) //guaranteed to stay in current quadrant //TIMES PATROLED = 0
        {
            while(true) {
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
                if(path.status == NavMeshPathStatus.PathComplete && Vector3.Distance(transform.position, point) > 40f) { // Check if point is on navmesh
                    return point;
                }
            }
        }
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
