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
        Surrounding
    }

    //ADD HITSTUN STATE
    public enum AttackState {
        Attacking,
        NotAttacking
    }

    public MotionState EnemyMotion;
    public AttackState EnemyAttack;

    public float patrolToIdleChance = 0.4f;
    public float idleToPatrolChance = 0.15f;
    public float swapStateInterval = 12f;
    public float shrineSpawnRange = 200f;

    public Vector3 surroundTarget = Vector3.zero;
    public Vector3 surroundSpot = Vector3.zero;
    public Vector3 nextSpot = Vector3.zero;

    public List<Vector3> movementQueue;

    //time for enemy to decide next action after reaching edge of chase distance
    public float decisionTime = 1f; 

    private float myTime = 0.0f;
    private Vector3 startOfPath;

    public Transform shrine;

    [SerializeField] int quadrant;
    private int timesPatroled;
    public AI_Manager ai;
    public Enemy_Spawner es;

    public int numTimesCheckIfNeedChase = 4;

    //threshold for the enemy to go from alerted into chase
        //represents 1/2 second of movement
    public float chaseThreshold;

    //necessary for starting the coroutine
    private bool justAlerted;

    //check if enemy has left arena
    public bool exitedArena;

    public float seekSpeed;
    public float chaseSpeed;

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
    public Material surroundMat;

    // Start is called before the first frame update
    void Start()
    {
        path = new UnityEngine.AI.NavMeshPath();
        ThisEnemy = GetComponent<UnityEngine.AI.NavMeshAgent>();
        EnemyMotion = MotionState.Idling;
        EnemyAttack = AttackState.NotAttacking;
        player = GameObject.Find("Player");
        es = GameObject.Find("ShrineManager").GetComponent<Enemy_Spawner>();
        ai = shrine.GetComponent<AI_Manager>();
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
        else if (EnemyMotion == MotionState.Surrounding)
        {
            alertBox.GetComponent<MeshRenderer>().material = surroundMat;
        }   

        myTime += Time.deltaTime;
        switch (EnemyMotion)
        {
            case MotionState.Patroling:
                ThisEnemy.speed = chaseSpeed;
                ThisEnemy.stoppingDistance = 0.05f;
                if (ThisEnemy.remainingDistance <= ThisEnemy.stoppingDistance + 0.01f) {
                    float temp = Random.Range(0.0f, 1.0f);
                    //if > 50% patroling increase chance to swap
                    if (temp < ai.checkPatrol(patrolToIdleChance) && timesPatroled > 2) //swap states
                    {
                        EnemyMotion = MotionState.Idling;
                        timesPatroled = 0;
                    } else {
                        ThisEnemy.SetDestination(findNextWaypoint());
                    }
                    timesPatroled++;
                }
                break;
            case MotionState.Idling: //update to make enemies rotate or move around slightly since having them be afk isn't interactive
                if (myTime > swapStateInterval) {
                    float temp = Random.Range(0.0f, 1.0f);
                    //if < 25% patroling increase chance to swap
                    if (temp < ai.checkIdle(idleToPatrolChance)) //swap states
                    {
                        EnemyMotion = MotionState.Patroling;
                    }                    
                    myTime = 0.0f;
                }
                break;
            case MotionState.Relocating:
                ThisEnemy.speed = chaseSpeed;
                ThisEnemy.stoppingDistance = 0.05f;
                if (ThisEnemy.hasPath && ThisEnemy.remainingDistance < ThisEnemy.stoppingDistance)
                {
                    ThisEnemy.ResetPath();
                    exitedArena = false;
                    EnemyMotion = MotionState.Idling;
                } 
                else if (!ThisEnemy.hasPath)
                {
                    ThisEnemy.SetDestination(es.chooseRelocation(shrine).position);
                }
                break;
            case MotionState.Alerted:
                // Tether movement to player's, but reduce our movement speed. Keep turned towards the player. If player approaches for N seconds, Chasing state
                transform.LookAt(player.transform.position + new Vector3(0.0f, 2.5f, 0.0f));
                if (justAlerted)
                {
                    ThisEnemy.ResetPath();
                    justAlerted = false;
                    StartCoroutine(decideAlertedAction());
                }
                break;
            case MotionState.Seeking:
                //set speed to normal
                ThisEnemy.speed = seekSpeed;
                ThisEnemy.stoppingDistance = 10;
                StopCoroutine(decideAlertedAction());
                if (!ThisEnemy.hasPath)
                    startOfPath = transform.position;
                ThisEnemy.CalculatePath(player.transform.position, path);
                if (path.status == NavMeshPathStatus.PathComplete) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                    if (!exitedArena) { //if still in arena
                        ThisEnemy.SetDestination(player.transform.position);
                    } else {
                        EnemyMotion = MotionState.Relocating;
                        ThisEnemy.ResetPath();
                    }
                }
                break;
            case MotionState.Chasing:
                //set speed to faster
                ThisEnemy.speed = chaseSpeed;
                ThisEnemy.stoppingDistance = 10;
                StopCoroutine(decideAlertedAction());
                if (!ThisEnemy.hasPath)
                    startOfPath = transform.position;
                ThisEnemy.CalculatePath(player.transform.position, path);
                if (path.status == NavMeshPathStatus.PathComplete) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                    if (!exitedArena) { //if still in arena
                        ThisEnemy.SetDestination(player.transform.position);
                    } else {
                        EnemyMotion = MotionState.Relocating;
                        ThisEnemy.ResetPath();
                    }
                }

                //check if enemy is in range to surround
                    //-> go to surround

                break;

                //after the above is broken < 1.5 * surroundRadius
                    //regenerate the spot so they go on the other side of the player
                    //-> rather than running through the player
                        //this causes issues because if the player is fully surrounded and finds a way out of that situation enemies will try to run through the player
                        //-> the paths need to be generated around the player not through them
                            //this is very difficult as we know  
                            //we might be able to store 3 positions one before the player, one to the side of the player, one at the location to make enemies go around
                                //before the player would need to be outside of surround radius so they dont run into each other
                                    //^^ this one is already needed because enemies will run into each ohter when assuming surrounding positions

            case MotionState.Surrounding:
                ThisEnemy.speed = chaseSpeed;
                ThisEnemy.stoppingDistance = 0.05f;
                //determine a way to track which enemy aligns with which spot in generated surround spots array (n)
                //set destination to player position + surround spots array [n]


                // working code, un-block to fix
                // if (surroundSpot == Vector3.zero)
                // {
                //     surroundSpot = ai.determineSurroundSpotV3(transform);
                // }

                // IMPLEMENT THIS TAKING INTO ACCOUNT NOT WANTING ENEMIES IN WALLS OR OFF MAP
                // if (surroundSpot != Vector3.zero)
                //     surroundTarget = ai.calculateSurroundSpotInWorld();

                //move to tracking spot
                    //move to surround spot

                //mini A* around the tracking spots to get to their surround spots
                    //still might have issues running into each other, but that can be figured out later

                // if they dont have a path generate one
                if (movementQueue.Count == 0 && surroundSpot == Vector3.zero) {
                    movementQueue = ai.determineSurroundSpot(transform);
                }

                // if they have reached their spot give them a new one
                if (ThisEnemy.remainingDistance < ThisEnemy.stoppingDistance && movementQueue.Count > 0)
                {
                    nextSpot = movementQueue[0];
                    movementQueue.RemoveAt(0);
                }
                NavMeshHit hit;
                NavMesh.SamplePosition(nextSpot + player.transform.position, out hit, 200.0f, NavMesh.AllAreas);
                ThisEnemy.CalculatePath(hit.position, path); //might need to do the find spot Navmesh thing if doesnt work
                if (path.status == NavMeshPathStatus.PathComplete && Vector3.Distance(hit.position, transform.position) > ai.surroundRadius * 1.5) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                    if (!exitedArena) { //if still in arena
                        ThisEnemy.SetDestination(hit.position);
                    } else {
                        EnemyMotion = MotionState.Relocating;
                        ThisEnemy.ResetPath();
                    }
                }
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
        if (ai.checkIfNeedRelocate(quadrant)) 
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

        return es.chooseRelocation(shrine).position;        
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
        for (int i = 0; i < numTimesCheckIfNeedChase; i++)
        {
            //get distance before
            if (ThisEnemy.CalculatePath(player.transform.position, path))
            {
                ThisEnemy.SetDestination(player.transform.position);
                beforeDist = ThisEnemy.remainingDistance;
            }
            //ThisEnemy.ResetPath();
            //wait half second
            yield return new WaitForSeconds(0.5f);

            //get distance after
            if (ThisEnemy.CalculatePath(player.transform.position, path))
            {
                ThisEnemy.SetDestination(player.transform.position);
                afterDist = ThisEnemy.remainingDistance;
            }
            //ThisEnemy.ResetPath();
            //two cases
            //before > after positive -> running at enemy (check if dist is negligible like < 0.5 units)
            //after > before negative -> running away from enemy (check if dist is negligible like < 0.5 units)
            // Debug.Log("Before Dist: " + beforeDist);
            // Debug.Log("After Dist: " + afterDist);

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
            if (hitInfo.transform.CompareTag("Player") && (EnemyMotion == MotionState.Idling || EnemyMotion == MotionState.Patroling))
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
