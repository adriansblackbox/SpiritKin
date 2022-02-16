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
    //Enemies will surround player
        //They will attack once at a time

///////////////////////////////////////////////////STATES
    
    //ADD HITSTUN STATE
    public enum MotionState {
        Alerted,
        Idling,
        Patroling,
        Seeking,
        Chasing,
        AfterChase
    }

    //ADD HITSTUN STATE
    public enum AttackState {
        Attacking,
        Surrounding,
        NotAttacking,
    }

    public MotionState EnemyMotion;
    public AttackState EnemyAttack;

    public float patrolToIdleChance;
    public float idleToPatrolChance;
    public float swapStateInterval;
    public float returnDist = 100;

    private float shrineDist;
    public float shrineSpawnRange;

    //time for enemy to decide next action after reaching edge of chase distance
    public float decisionTime = 1f; 

    private float myTime;
    private Vector3 startOfPath;

    public Transform shrine;

    [SerializeField] int quadrant;
    private int timesPatroled;
    public Shrine_Controller sc;

    public int numTimesCheckIfNeedChase;

    //threshold for the enemy to go from alerted into chase
        //represents 1/2 second of movement
    public float chaseThreshold;

    //threshold for the enemy to go from alerted into idle
        //represents distance after chase threshold checks
    public float idleThreshold;

//////////////////////////////////////////////////NAVMESH

    public NavMeshAgent ThisEnemy;
    public NavMeshPath path;
    public GameObject player;
    public GameObject alertBox;

/////////////////////////////////////////////////SPHERECASTING
    public float raycastRadius;
    public float targetDetectionRange;

    private RaycastHit hitInfo;
    private bool hasDetectedPlayer = false;


    // Start is called before the first frame update
    void Start()
    {
        path = new UnityEngine.AI.NavMeshPath();
        ThisEnemy = GetComponent<UnityEngine.AI.NavMeshAgent>();
        EnemyMotion = MotionState.Idling;
        EnemyAttack = AttackState.NotAttacking;
        //sc = transform.parent.parent.GetComponent<Shrine_Controller>();
        //shrine = transform.parent.parent;
        //shrineSpawnRange = shrine.GetComponent<Shrine>().shrineSpawnRange;
        determineQuadrant();
    }

    // Update is called once per frame
    void Update()
    {

        //spherecast to check for player
        checkForPlayer();

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
                shrineDist = Vector3.Distance(shrine.position, transform.position);
                if (shrineDist < shrineSpawnRange * 0.20 || shrineDist > shrineSpawnRange * 0.55 && !ThisEnemy.hasPath)
                    ThisEnemy.SetDestination(findIdleSpot());
                

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
                StartCoroutine(decideAlertedAction());
                break;
            case MotionState.Seeking:
                if (!ThisEnemy.hasPath) {
                    ThisEnemy.CalculatePath(player.transform.position, path);
                    ThisEnemy.SetDestination(player.transform.position);
                }
                //reached end of path without entering chasing
                if (ThisEnemy.remainingDistance < ThisEnemy.stoppingDistance + 0.01f)
                {
                    ThisEnemy.ResetPath();
                    //IEnumerator pause for 3 seconds then return to what they were previously doing
                }
                else
                {
                    //check if need to enter chasing

                }
                break;
            case MotionState.Chasing:
                if (!ThisEnemy.hasPath)
                    startOfPath = transform.position;
                ThisEnemy.CalculatePath(player.transform.position, path);
                if (path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                    if (Vector3.Distance(transform.position, startOfPath) < returnDist){
                        ThisEnemy.SetDestination(player.transform.position);
                    } else {
                        EnemyMotion = MotionState.AfterChase;
                        ThisEnemy.ResetPath();
                    }
                }
                break;
            case MotionState.AfterChase:
                //should i keep chasing or should i return to my prior commitment
                StartCoroutine(decideActionAfterChase());
                break;
            default:
                break;
        }
    }

///////////////////////////////////////////////////STATES    

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
            NavMeshHit hit;
            NavMesh.SamplePosition(point, out hit, 20.0f, NavMesh.AllAreas);

            ThisEnemy.CalculatePath(hit.position, path);
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
                NavMeshHit hit;
                NavMesh.SamplePosition(point, out hit, 20.0f, NavMesh.AllAreas);

                ThisEnemy.CalculatePath(hit.position, path);
                if(path.status == NavMeshPathStatus.PathComplete && Vector3.Distance(transform.position, hit.position) > 40f) { // Check if point is on navmesh
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

    //AFTER ENEMY HAS REACHED ITS CHASE LIMIT
    IEnumerator decideActionAfterChase() 
    {
        float distBeforeDecision = Vector3.Distance(transform.position, player.transform.position);
        //track player's current distance from enemy
        yield return new WaitForSeconds(decisionTime);
        float distAfterDecision = Vector3.Distance(transform.position, player.transform.position);
        //compare new distance to old & make decision
        if (distAfterDecision > distBeforeDecision)
        {
            EnemyMotion = MotionState.Idling;
        }
        else
        {
            EnemyMotion = MotionState.Chasing;
        }
    }

    //after 0.5 & 1.5 seconds
        //track a delta float of distance between player and enemy
        //significantly away from the enemy -> enemy returns to what it was doing
        //significatnly towards the enemy -> enemy chases player
        //if neither threshold is reached -> seek the player      
    IEnumerator decideAlertedAction()
    {
        float firstDist;
        float delta;
        float beforeDist;
        float afterDist = 0f;

        transform.LookAt(player.transform);

        ThisEnemy.CalculatePath(player.transform.position, path);
        firstDist = ThisEnemy.remainingDistance;
        for (int i = 0; i < numTimesCheckIfNeedChase; i++)
        {
            ThisEnemy.CalculatePath(player.transform.position, path);
            beforeDist = ThisEnemy.remainingDistance;
            yield return new WaitForSeconds(0.5f);
            ThisEnemy.CalculatePath(player.transform.position, path);
            afterDist = ThisEnemy.remainingDistance;

            //two cases
            //before > after positive -> running at enemy (check if dist is negligible like < 0.5 units)
            //after > before negative -> running away from enemy (check if dist is negligible like < 0.5 units)
            delta = beforeDist - afterDist;

            if (delta > chaseThreshold)
            {
                Debug.Log("CHASING PLAYER");
                EnemyMotion = MotionState.Chasing;
                yield return null;
            }
            else
            {
                Debug.Log("No need to chase player");
            }
        }
        yield return new WaitForSeconds(0.5f);
        //don't need to chase
            //enter seek or idle based on final check
            //has the player signifcantly moved away from the enemy
        if (firstDist - afterDist < idleThreshold)
        {
            EnemyMotion = MotionState.Idling;
            Debug.Log("Idle After Alerted");
        }
        else
        {
            EnemyMotion = MotionState.Seeking;
            Debug.Log("SEEKING THIS MF");
        }
    }

/////////////////////////////////////////////////SPHERECASTING
    private void checkForPlayer()
    {
        hasDetectedPlayer = Physics.SphereCast(transform.position, raycastRadius, transform.forward, out hitInfo, targetDetectionRange);

        if (hasDetectedPlayer)
        {
            if (hitInfo.transform.CompareTag("Player") && (EnemyMotion == MotionState.Idling || EnemyMotion == MotionState.Patroling))
            {
                Debug.Log("Player Detected!");
                ThisEnemy.ResetPath();
                EnemyMotion = MotionState.Alerted;
            }
            else if (hitInfo.transform.CompareTag("Player") && EnemyMotion == MotionState.Seeking)
            {
                Debug.Log("Player Detected & Chasing after Seeking!");
                EnemyMotion = MotionState.Chasing;
            }
            else if (!hitInfo.transform.CompareTag("Player"))
            {
                hasDetectedPlayer = false;
                Debug.Log("No Player Here");
            }
                
        }
    }

    void OnDrawGizmos()
    {
        if (hasDetectedPlayer)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(new Vector3(0f, 0f, targetDetectionRange / 2f), new Vector3(raycastRadius, raycastRadius / 5, targetDetectionRange - 5));
    }
}
