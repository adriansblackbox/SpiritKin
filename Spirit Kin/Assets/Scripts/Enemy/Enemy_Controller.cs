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
        Relocating,
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

    public float patrolToIdleChance = 0.4f;
    public float idleToPatrolChance = 0.15f;
    public float swapStateInterval = 12f;
    public float returnDist = 100f;
    public float shrineSpawnRange = 200f;

    //time for enemy to decide next action after reaching edge of chase distance
    public float decisionTime = 1f; 

    private float myTime = 0.0f;
    private Vector3 startOfPath;

    public Transform shrine;

    [SerializeField] int quadrant;
    private int timesPatroled;
    public Shrine_Controller sc;
    public Enemy_Spawner es;

    public int numTimesCheckIfNeedChase = 4;

    //threshold for the enemy to go from alerted into chase
        //represents 1/2 second of movement
    public float chaseThreshold;

    //necessary for starting the coroutine
    private bool justAlerted;

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

/////////////////////////////////////////////////STATE BOX FOR TESTING

    public Material alertedMat;
    public Material seekingMat;
    public Material chasingMat;
    public Material idleMat;
    public Material patrolMat;
    public Material relocateMat;

    // Start is called before the first frame update
    void Start()
    {
        path = new UnityEngine.AI.NavMeshPath();
        ThisEnemy = GetComponent<UnityEngine.AI.NavMeshAgent>();
        EnemyMotion = MotionState.Idling;
        EnemyAttack = AttackState.NotAttacking;
        player = GameObject.Find("Player");
        es = GameObject.Find("ShrineManager").GetComponent<Enemy_Spawner>();
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
            alertBox.GetComponent<MeshRenderer>().material = alertedMat;
        } 
        else if (EnemyMotion == MotionState.Seeking) 
        {
            alertBox.GetComponent<MeshRenderer>().material = seekingMat;
        } 
        else if (EnemyMotion == MotionState.Idling) 
        {
            alertBox.GetComponent<MeshRenderer>().material = idleMat;
        } 
        else if (EnemyMotion == MotionState.Chasing) 
        {
            alertBox.GetComponent<MeshRenderer>().material = chasingMat;
        } 
        else if (EnemyMotion == MotionState.Relocating) 
        {
            alertBox.GetComponent<MeshRenderer>().material = relocateMat;
        } 
        else if (EnemyMotion == MotionState.Patroling) 
        {
            alertBox.GetComponent<MeshRenderer>().material = patrolMat;
        }

        myTime += Time.deltaTime;
        switch (EnemyMotion)
        {
            case MotionState.Patroling:
                if (ThisEnemy.remainingDistance <= ThisEnemy.stoppingDistance + 0.01f) {
                    float temp = Random.Range(0.0f, 1.0f);
                    //if > 50% patroling increase chance to swap
                    if (temp < sc.checkPatrol(patrolToIdleChance) && timesPatroled > 2) //swap states
                    {
                        EnemyMotion = MotionState.Idling;
                        timesPatroled = 0;
                    } else {
                        ThisEnemy.SetDestination(findNextWaypoint());
                    }
                    timesPatroled++;
                }
                break;
            case MotionState.Idling:
                if (myTime > swapStateInterval) {
                    float temp = Random.Range(0.0f, 1.0f);
                    //if < 25% patroling increase chance to swap
                    if (temp < sc.checkIdle(idleToPatrolChance)) //swap states
                    {
                        EnemyMotion = MotionState.Patroling;
                    }                    
                    myTime = 0.0f;
                }
                break;
            case MotionState.Relocating:
                if (Vector3.Distance(shrine.position, transform.position) > shrine.GetComponent<Shrine>().shrineSpawnRange * 0.55 && !ThisEnemy.hasPath)
                {
                    ThisEnemy.SetDestination(findRelocateSpot());
                }
                else if (ThisEnemy.hasPath && ThisEnemy.remainingDistance < ThisEnemy.stoppingDistance)
                {
                    ThisEnemy.ResetPath();
                    EnemyMotion = MotionState.Idling;
                }
                break;
            case MotionState.Alerted:
                // Tether movement to player's, but reduce our movement speed. Keep turned towards the player. If player approaches for N seconds, Chasing state
                transform.LookAt(player.transform);
                if (justAlerted)
                {
                    ThisEnemy.ResetPath();
                    justAlerted = false;
                    StartCoroutine(decideAlertedAction());
                }
                break;
            case MotionState.Seeking:
                StopCoroutine(decideAlertedAction());
                if (!ThisEnemy.hasPath) {
                    ThisEnemy.CalculatePath(player.transform.position, path);
                    if (path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete) {
                        ThisEnemy.SetDestination(player.transform.position);
                    }
                }
                //reached end of path without entering chasing
                else if (ThisEnemy.remainingDistance < ThisEnemy.stoppingDistance + 0.01f)
                {
                    ThisEnemy.ResetPath();
                    EnemyMotion = MotionState.Relocating;
                }
                break;
            case MotionState.Chasing:
                StopCoroutine(decideAlertedAction());
                if (!ThisEnemy.hasPath)
                    startOfPath = transform.position;
                ThisEnemy.CalculatePath(player.transform.position - (Vector3.Scale(transform.forward, new Vector3(3,1,3))), path);
                if (path.status == NavMeshPathStatus.PathComplete) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                    if (Vector3.Distance(transform.position, startOfPath) < returnDist) {
                        ThisEnemy.SetDestination(player.transform.position - (Vector3.Scale(transform.forward, new Vector3(3,1,3))));
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
    private Vector3 findRelocateSpot()
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

        return es.chooseLocation(shrine).position;        
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
        return es.chooseLocation(shrine).position;
       
    }
    
    private void determineQuadrant()
    {
        if (transform.position.x < shrine.position.x && transform.position.z > shrine.position.z) //Quadrant 1
        {
            quadrant = 1;
        }
        else if (transform.position.x > shrine.position.x && transform.position.z > shrine.position.z) //Quadrant 2
        {
            quadrant = 2;
        }
        else if (transform.position.x < shrine.position.x && transform.position.z < shrine.position.z) //Quadrant 3
        {
            quadrant = 3;
        }
        else if (transform.position.x > shrine.position.x && transform.position.z < shrine.position.z) //Quadrant 4
        {
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
            EnemyMotion = MotionState.Relocating;
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
        float delta;
        float beforeDist = 0f;
        float afterDist = 0f;

        transform.LookAt(player.transform);
        for (int i = 0; i < numTimesCheckIfNeedChase; i++)
        {
            //get distance before
            if (ThisEnemy.CalculatePath(player.transform.position, path))
            {
                ThisEnemy.SetDestination(player.transform.position);
                beforeDist = ThisEnemy.remainingDistance;
                ThisEnemy.ResetPath();
            }

            //wait half second
            yield return new WaitForSeconds(0.5f);

            //get distance after
            if (ThisEnemy.CalculatePath(player.transform.position, path))
            {
                ThisEnemy.SetDestination(player.transform.position);
                afterDist = ThisEnemy.remainingDistance;
                ThisEnemy.ResetPath();
            }

            //two cases
            //before > after positive -> running at enemy (check if dist is negligible like < 0.5 units)
            //after > before negative -> running away from enemy (check if dist is negligible like < 0.5 units)
            Debug.Log("Before Dist: " + beforeDist);
            Debug.Log("After Dist: " + afterDist);

            delta = beforeDist - afterDist;
            if (delta > chaseThreshold)
            {
                Debug.Log("Delta -> chaseThreshold -> NOW CHASING");
                ThisEnemy.ResetPath();
                EnemyMotion = MotionState.Chasing;
                yield break;
            }
        }
        if (EnemyMotion != MotionState.Chasing) 
        {
            ThisEnemy.ResetPath();
            EnemyMotion = MotionState.Seeking;
            Debug.Log("Didn't need to chase player -> Seeking after alerted");
        }
    }

/////////////////////////////////////////////////SPHERECASTING
    private void checkForPlayer()
    {
        hasDetectedPlayer = Physics.SphereCast(transform.position, raycastRadius, transform.forward, out hitInfo, targetDetectionRange);

        if (hasDetectedPlayer)
        {
            if (hitInfo.transform.CompareTag("Player") && (EnemyMotion == MotionState.Idling || EnemyMotion == MotionState.Patroling || EnemyMotion == MotionState.Relocating))
            {
                Debug.Log("Player Detected!");
                ThisEnemy.ResetPath();
                EnemyMotion = MotionState.Alerted;
                justAlerted = true;
            }
            else if (hitInfo.transform.CompareTag("Player") && EnemyMotion == MotionState.Seeking)
            {
                Debug.Log("Player Detected & Chasing after Seeking!");
                ThisEnemy.ResetPath();
                EnemyMotion = MotionState.Chasing;
            }
            else if (!hitInfo.transform.CompareTag("Player"))
            {
                hasDetectedPlayer = false;
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

        Gizmos.DrawWireCube(new Vector3(0f, 0f, targetDetectionRange / 4f), new Vector3(raycastRadius, raycastRadius / 5, targetDetectionRange - 20));
    }
}
