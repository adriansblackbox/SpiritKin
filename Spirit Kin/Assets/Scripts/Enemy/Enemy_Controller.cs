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

    #region States

    //ADD HITSTUN STATE
    public enum MotionState {
        Alerted,
        Idling,
        Patroling,
        Seeking,
        Relocating,
        Chasing,
        Surrounding,
        Waiting
    }

    //ADD HITSTUN STATE
    public enum AttackState {
        Attacking,
        NotAttacking,
        Waiting
    }
    [Header("Attacks")]
    public Enemy_Attack currentAttack;
    public Enemy_Attack[] enemyAttacks;
    public float currentRecoveryTime = 0f;
    public Material enemyAttackingMat;
    public Material enemyAttackingTwoMat;    
    public Material enemyNotAttackingMat;
    public float attackTimer = 0.0f;
    [Tooltip("Determines Speed of Charge")]
    public float durationOfCharge = 0.5f;
    public float chargeSpeed;
    private Vector3 dirVec;
    Vector3 startPosition = Vector3.zero;
    Vector3 endPosition = Vector3.zero;
    float timeCharging = 0;
    

    [Header("States")]


    public MotionState EnemyMotion;
    public AttackState EnemyAttack;

    public float patrolToIdleChance = 0.4f;
    public float idleToPatrolChance = 0.15f;
    public float swapStateInterval = 12f;
    public float shrineSpawnRange = 200f;

    #endregion
    
    #region Movement

    [Header("Movement")]
    public Vector3 surroundTarget = Vector3.zero;
    public Vector3 surroundSpot = Vector3.zero;
    public Vector3 nextSpot = Vector3.zero;
    public int surroundIndex = -1; //necessary for resetting surrounding spots after leaving surround state

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

    [Tooltip("Distance for enemy to break out of surrounding")]
    public float breakDist;

    #endregion

//////////////////////////////////////////////////NAVMESH

    public NavMeshAgent ThisEnemy;
    public NavMeshPath path;
    public GameObject player;
    public GameObject alertBox;

/////////////////////////////////////////////////SPHERECASTING
    [Header("Spherecasting")]
    public float raycastRadius;
    public float targetDetectionRange;

    private RaycastHit hitInfo;
    private bool hasDetectedPlayer = false;

/////////////////////////////////////////////////STATE BOX FOR TESTING
    [Header("Debugging")]
    public bool showLogs = true;

    public Material alertedMat;
    public Material seekingMat;
    public Material chasingMat;
    public Material idleMat;
    public Material patrolMat;
    public Material relocateMat;
    public Material surroundMat;
    public Material attackMat;
    public Material recoverMat;

    // Start is called before the first frame update
    void Start()
    {
        path = new UnityEngine.AI.NavMeshPath();
        ThisEnemy = GetComponent<UnityEngine.AI.NavMeshAgent>();
        EnemyMotion = MotionState.Idling;
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
        else if (EnemyMotion == MotionState.Waiting && EnemyAttack == AttackState.Attacking)
        {
            alertBox.GetComponent<MeshRenderer>().material = attackMat;
        }
        else if (EnemyMotion == MotionState.Waiting && EnemyAttack == AttackState.NotAttacking)
        {
            alertBox.GetComponent<MeshRenderer>().material = recoverMat;
        }

        myTime += Time.deltaTime;


        //we will want a function to handle the one time reset of values when moved to notAttacking
        switch (EnemyAttack)
        {
            case AttackState.Attacking:
                attackTimer += Time.deltaTime;
                if (currentRecoveryTime <= 0) //ready to attack
                    attackTarget(); //attack target with current attack, if no current attack then select one
                break;
            case AttackState.NotAttacking:
                //reset all values and get ready to be called upon again to attack
                    //-> second pass figure out next attack
                handleRecovery();
                GetComponentInChildren<MeshRenderer>().material = enemyNotAttackingMat;
                transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = enemyNotAttackingMat;
                break;
            case AttackState.Waiting:
                Log("Waiting to attack");
                break;
            default:
                break;
        }

        switch (EnemyMotion)
        {
            case MotionState.Patroling:
                ThisEnemy.speed = chaseSpeed;
                ThisEnemy.stoppingDistance = 2.5f;
                if (ThisEnemy.remainingDistance <= ThisEnemy.stoppingDistance) {
                    float temp = Random.Range(0.0f, 1.0f);
                    //if > 50% patroling increase chance to swap
                    if (temp < ai.checkPatrol(patrolToIdleChance) && timesPatroled > 4) //swap states
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
                ThisEnemy.speed = seekSpeed;
                ThisEnemy.stoppingDistance = 2.5f;
                if (ThisEnemy.remainingDistance <= ThisEnemy.stoppingDistance) {
                    if (myTime > swapStateInterval) {
                        float temp = Random.Range(0.0f, 1.0f);
                        //if < 25% patroling increase chance to swap
                        if (temp < ai.checkIdle(idleToPatrolChance)) //swap states
                        {
                            EnemyMotion = MotionState.Patroling;
                        } else {
                            ThisEnemy.SetDestination(findNextWaypoint());
                        }                    
                        myTime = 0.0f;
                    }
                }
                break;
            case MotionState.Relocating:
                ThisEnemy.speed = chaseSpeed;
                ThisEnemy.stoppingDistance = 2.5f;
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
                //look in player's direction
                transform.LookAt(player.transform.position);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
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
                else{
                    EnemyMotion = MotionState.Relocating;
                    ThisEnemy.ResetPath();
                }
                break;
            case MotionState.Chasing:
                //set speed to faster
                ThisEnemy.speed = chaseSpeed;
                ThisEnemy.stoppingDistance = 10;
                StopCoroutine(decideAlertedAction());

                //if the player is inside breakDist swap to surrounding
                if (Vector3.Distance(player.transform.position, transform.position) < breakDist - 1f)
                {
                    EnemyMotion = MotionState.Surrounding;
                    //-> going into surrounding what do we need to reset before then
                        //reset path
                    ThisEnemy.ResetPath();
                    break;
                }

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
                else{
                    EnemyMotion = MotionState.Relocating;
                    ThisEnemy.ResetPath();
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
                ThisEnemy.stoppingDistance = 0.5f;
                //look in player's direction
                transform.LookAt(player.transform.position);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                //determine a way to track which enemy aligns with which spot in generated surround spots array (n)
                //set destination to player position + surround spots array [n]

                // IMPLEMENT THIS TAKING INTO ACCOUNT NOT WANTING ENEMIES IN WALLS OR OFF MAP
                // if (surroundSpot != Vector3.zero)
                //     surroundTarget = ai.calculateSurroundSpotInWorld();

                //mini A* around the tracking spots to get to their surround spots
                    //still might have issues running into each other, but that can be figured out later

                //if the player is outside breakDist swap to chasing
                if (Vector3.Distance(player.transform.position, transform.position) > breakDist + 1f)
                {
                    EnemyMotion = MotionState.Chasing;
                    //-> going into chasing what do we need to reset before then
                        //reset spot in surrounding to be true [not taken anymore]
                        //empty movementQueue
                    movementQueue.Clear();
                    ai.surroundSpotAvailability[surroundIndex] = true;
                    surroundSpot = Vector3.zero;
                    nextSpot = Vector3.zero;
                    surroundTarget = Vector3.zero;
                    surroundIndex = -1;
                    ThisEnemy.ResetPath();
                    ai.enemiesReadyToAttack.Remove(gameObject);
                    break;
                }

                // if they dont have a path generate one
                if (movementQueue.Count == 0 && surroundSpot == Vector3.zero) {
                    movementQueue = ai.determineSurroundSpot(transform);
                    if (movementQueue.Count == 0) {
                        EnemyMotion = MotionState.Relocating;
                        break;
                    }

                }

                // if they have reached their spot give them a new one
                if (ThisEnemy.remainingDistance < ThisEnemy.stoppingDistance && movementQueue.Count > 0)
                {
                    nextSpot = movementQueue[0];
                    movementQueue.RemoveAt(0);
                    ThisEnemy.speed = chaseSpeed;
                } 
                else if (ThisEnemy.remainingDistance < ThisEnemy.stoppingDistance && movementQueue.Count == 0 && !ai.enemiesReadyToAttack.Contains(gameObject))
                {
                    ai.enemiesReadyToAttack.Add(gameObject);
                    ThisEnemy.speed = seekSpeed / 1.4f;
                }
                
                NavMeshHit hit;
                NavMesh.SamplePosition(nextSpot + player.transform.position, out hit, 400.0f, NavMesh.AllAreas);
                ThisEnemy.CalculatePath(hit.position, path); //might need to do the find spot Navmesh thing if doesnt work
                if (path.status == NavMeshPathStatus.PathComplete && Vector3.Distance(hit.position, transform.position) > ThisEnemy.stoppingDistance + 0.5f) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                    if (!exitedArena) { //if still in arena
                        ThisEnemy.SetDestination(hit.position);
                    } else {
                        EnemyMotion = MotionState.Relocating;
                        ThisEnemy.ResetPath();
                    }
                }
                else{
                    EnemyMotion = MotionState.Relocating;
                    ThisEnemy.ResetPath();
                }
                break;
            case MotionState.Waiting:
                Log("Waiting for Next Movement Action");
                //need to look into what we need to track or change when entering/exiting waiting
                break;
            default:
                break;
        }
    }

    private void handleRecovery()
    {
        if (currentRecoveryTime > 0) //recovering from attack
            currentRecoveryTime -= Time.deltaTime;
        else if (currentRecoveryTime <= 0) //ready to attack again
            finishAttack();
    }

    #region Attacks
        
    private void getAttack()
    {
        //starter implementation
            //set charge attack to be the current attack
        currentAttack = enemyAttacks[0];
        Log(currentAttack.name);

        if (currentAttack.name == "Charge")
        {
            //setup all of the values needed for the charge
            dirVec = player.transform.position - transform.position;
            timeCharging = 0;
            startPosition = transform.position;
            endPosition = player.transform.position + dirVec;
            endPosition.y = transform.position.y;
            ai.enemiesReadyToAttack.Remove(gameObject);
        }

        //full implementation vvv
            //get direction
            //get viewing angle
            //get distance

            //loop through attacks and consider attacks which are within the distance + viewing angle
                //select one randomly using attackPriority
    }

    private void attackTarget()
    {
        if (currentAttack == null) {
            getAttack();
        }
        else
        {
            if (currentAttack.name == "Charge")
            {
                chargeAttack();
                Log("Charge Attack");
            } 
            else if (currentAttack.name == "Swipe")
            {
                swipeAttack();
                Log("Swipe Attack");
            }
            //this is where we would do the animation
                //but instead have to handle it with code
        }
    }

    float yellowTime = 0.25f;
    float orangeTime = 0.15f;

    private void chargeAttack() //-> might need to be an IEnumerator
    {
        if (attackTimer < yellowTime) 
        {
            //aim at player
            transform.LookAt(player.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            //set mat to yellow
            GetComponentInChildren<MeshRenderer>().material = alertedMat;
            transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = alertedMat;
        } 
        else if (attackTimer < yellowTime + orangeTime)
        {
            //aim at player
            transform.LookAt(player.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            //set mat to orange
            GetComponentInChildren<MeshRenderer>().material = enemyAttackingTwoMat;
            transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = enemyAttackingTwoMat;
        }
        else if (attackTimer > yellowTime + orangeTime + 0.05f)
        {
            //set mat to red and charge
            GetComponentInChildren<MeshRenderer>().material = enemyAttackingMat;
            transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = enemyAttackingMat;

            //charge player
                //get direction vector
                //update lerp rate
                //lerp to position behind the player
            timeCharging += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, timeCharging/durationOfCharge);

            if (Vector3.Distance(transform.position, endPosition) < 0.1f || attackTimer > yellowTime + orangeTime + 1.5f) { //need to add second condition for if they get stuck or can't reach dest
                EnemyAttack = AttackState.NotAttacking;
                ai.attackingEnemy = null;
                currentRecoveryTime = currentAttack.recoveryTime;
                Log("Lerp Completed");
            }
        }
    }

    private void finishAttack()
    {
        Log("Recovered and ready to attack again");
        EnemyMotion = MotionState.Surrounding;
        EnemyAttack = AttackState.Waiting;
        currentAttack = null;
        attackTimer = 0.0f;

        //consider resetting surroundspot
        surroundTarget = Vector3.zero;
        surroundSpot = Vector3.zero;
        ai.surroundSpotAvailability[surroundIndex] = true;
        surroundIndex = -1;
        nextSpot = Vector3.zero;
    }

    private void swipeAttack()
    {

    }

    #endregion

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

            delta = beforeDist - afterDist;
            if (delta > chaseThreshold)
            {
                Log("Delta -> chaseThreshold -> NOW CHASING");
                ThisEnemy.ResetPath();
                EnemyMotion = MotionState.Chasing;
                yield break;
            }
        }
        if (EnemyMotion != MotionState.Chasing) 
        {
            ThisEnemy.ResetPath();
            EnemyMotion = MotionState.Seeking;
            Log("Didn't need to chase player -> Seeking after alerted");
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
                Log("Player Detected!");
                ThisEnemy.ResetPath();
                EnemyMotion = MotionState.Alerted;
                justAlerted = true;
            }
            else if (hitInfo.transform.CompareTag("Player") && EnemyMotion == MotionState.Seeking)
            {
                Log("Player Detected & Chasing after Seeking!");
                ThisEnemy.ResetPath();
                EnemyMotion = MotionState.Chasing;
            }
            else if (!hitInfo.transform.CompareTag("Player"))
            {
                hasDetectedPlayer = false;
            }
        }
    }

    //NEED TO CHANGE TO BE MORE FLUENT AND DETECT BETTER (MAYBE NOT SPHERECAST AND RATHER JUST A SPHERE IN FRONT OF ENEMY AT ALL TIMES)
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

    private void Log(object message)
    {
        if (showLogs)
            Debug.Log(message);
    }
}
