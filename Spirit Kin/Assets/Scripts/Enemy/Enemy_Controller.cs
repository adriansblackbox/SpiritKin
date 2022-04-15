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
    [Header("Animation")]
    public Animator enemyAnimator;
    public bool right = false;
    public bool left = false;

    [Header("Attacks")]
    public Enemy_Attack currentAttack;
    public Enemy_Attack[] enemyAttacks;
    public float currentRecoveryTime = 0f;
    public Material enemyAttackingMat;
    public Material enemyAttackingTwoMat;    
    public BoxCollider enemyCollider;
    public float attackTimer = 0.0f;
    [Tooltip("Determines Speed of Charge")]
    public float durationOfCharge = 0.5f;
    [Tooltip("Determines Length of Charge")]
    public float chargeLength = 20f;
    public float durationOfLunge = 0.25f;
    public float lungeLength = 7.5f;
    private Vector3 dirVec;
    Vector3 startPosition = Vector3.zero;
    Vector3 endPosition = Vector3.zero;
    float timeCharging = 0;
    float timeLunging = 0;

    public Transform[] rightSwipeOriginPoints;
    public Transform[] leftSwipeOriginPoints;
    public LayerMask swipeLayerMask;

    public GameObject leftSwipeTrail;
    public GameObject rightSwipeTrail;    

    [Header("States")]


    public MotionState EnemyMotion;
    public AttackState EnemyAttack;

    public bool stunned = false;
    private bool stunnedLastFrame = false;

    public float patrolToIdleChance = 0.4f;
    public float idleToPatrolChance = 0.15f;
    public float swapStateInterval = 12f;
    public float shrineSpawnRange = 200f;

    #endregion
    
    #region Movement

    [Header("Movement")]
    public Vector3 relocateSpot = Vector3.zero;
    public Vector3 surroundTarget = Vector3.zero;
    public Vector3 surroundSpot = Vector3.zero;
    public Vector3 nextSpot = Vector3.zero;
    public Vector3 unstuckingCheck = Vector3.zero; // Necessary for helping the AI get unstuck from where its at
    public int surroundIndex = -1; //necessary for resetting surrounding spots after leaving surround state

    public List<Vector3> movementQueue = new List<Vector3>();

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

////////////////////////////////////////////////// NAVMESH
    [Header("Navmesh")]
    public NavMeshAgent ThisEnemy;
    public NavMeshPath path;
    public GameObject player;
    public GameObject alertBox;

///////////////////////////////////////////////// SPHERECASTING
    [Header("Spherecasting")]
    public Transform[] visionFanOrigins;
    public float raycastRadius;
    public float targetDetectionRange;

    private RaycastHit hitInfo;
    private bool hasDetectedPlayer = false;

///////////////////////////////////////////////// STATE BOX FOR TESTING
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

        //This is done in the functions, but might be better to do it here
        enemyAnimator.SetBool("Right", right);
        enemyAnimator.SetBool("Left", left);

        //spherecast to check for player
        checkForPlayer();
        myTime += Time.deltaTime;

        //we will want a function to handle the one time reset of values when moved to notAttacking
        switch (EnemyAttack)
        {
            case AttackState.Attacking:
                attackTimer += Time.deltaTime;
                if (currentRecoveryTime <= 0 && !stunned) //ready to attack
                    attackTarget(); //attack target with current attack, if no current attack then select one
                break;
            case AttackState.NotAttacking:
                //reset all values and get ready to be called upon again to attack
                    //-> second pass figure out next attack
                handleRecovery();
                break;
            case AttackState.Waiting:
                Log("Waiting to attack");
                break;
            default:
                break;
        }

        if (!stunned)
        {
            if (stunnedLastFrame)
            {
                stunnedLastFrame = false;
                alertBox.SetActive(false);
                if (EnemyAttack != AttackState.Attacking && attackTimer == 0) {
                    EnemyMotion = MotionState.Chasing;
                } 
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
                                break;
                            }                 
                            myTime = 0.0f;
                        }
                        ThisEnemy.SetDestination(findNextWaypoint());
                    }
                    break;
                case MotionState.Relocating:
                    ThisEnemy.speed = chaseSpeed + 5f;
                    ThisEnemy.stoppingDistance = 5f;

                    if (unstuckingCheck == Vector3.zero) {
                        StartCoroutine(unstuckTimer());
                    }
                    //reached destination
                    //Log("Remaining: " + ThisEnemy.remainingDistance + " vs. Stopping: " + ThisEnemy.stoppingDistance);
                    if (ThisEnemy.hasPath && path.status == NavMeshPathStatus.PathComplete && ThisEnemy.remainingDistance < ThisEnemy.stoppingDistance)
                    {
                        ThisEnemy.ResetPath();
                        EnemyMotion = MotionState.Idling;
                    }
                    if (relocateSpot == Vector3.zero)
                        relocateSpot = es.chooseLocation(shrine).position;
                    ThisEnemy.CalculatePath(relocateSpot, path);
                    if (path.status == NavMeshPathStatus.PathComplete)
                        ThisEnemy.SetDestination(relocateSpot);
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
                    if (exitedArena)
                    {
                        ThisEnemy.ResetPath();
                        exitedArena = false;
                        EnemyMotion = MotionState.Relocating;
                        break;
                    }    
                    //set speed to normal
                    ThisEnemy.speed = seekSpeed;
                    ThisEnemy.stoppingDistance = 10;
                    StopCoroutine(decideAlertedAction());
                    if (!ThisEnemy.hasPath)
                        startOfPath = transform.position;
                    ThisEnemy.CalculatePath(player.transform.position, path);
                    if (path.status == NavMeshPathStatus.PathComplete) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                        ThisEnemy.SetDestination(player.transform.position);
                    }
                    break;
                case MotionState.Chasing:
                    if (exitedArena)
                    {
                        ThisEnemy.ResetPath();
                        exitedArena = false;
                        EnemyMotion = MotionState.Relocating;
                        break;
                    }            
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

                    // if (!ThisEnemy.hasPath)
                    //     startOfPath = transform.position;
                    ThisEnemy.CalculatePath(player.transform.position, path);
                    if (path.status == NavMeshPathStatus.PathComplete) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                        ThisEnemy.SetDestination(player.transform.position);
                    }
                    break;
                case MotionState.Surrounding:
                    if (exitedArena)
                    {
                        resetSurround();
                        exitedArena = false;
                        EnemyMotion = MotionState.Relocating;
                        break;
                    }

                    ThisEnemy.stoppingDistance = 5f;
                    //look in player's direction
                    transform.LookAt(player.transform.position);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                    //if the player is outside breakDist swap to chasing
                    if (Vector3.Distance(player.transform.position, transform.position) > breakDist + 1f)
                    {
                        EnemyMotion = MotionState.Chasing;
                        //-> going into chasing what do we need to reset before then
                            //reset spot in surrounding to be true [not taken anymore]
                            //empty movementQueue
                        resetSurround();
                        break;
                    }

                    // if they dont have a path generate one
                    if (!(GetComponent<CharacterStats>().isDying) && movementQueue.Count == 0 && surroundSpot == Vector3.zero) {
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
                    if (path.status == NavMeshPathStatus.PathComplete) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                        ThisEnemy.SetDestination(hit.position);
                    }
                    break;
                case MotionState.Waiting:
                    Log("Waiting for Next Movement Action");
                    if (exitedArena)
                    {
                        ThisEnemy.ResetPath();
                        exitedArena = false;
                        EnemyMotion = MotionState.Relocating;
                        break;
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            alertBox.SetActive(true);
            alertBox.GetComponent<MeshRenderer>().material = patrolMat;
            Log("Stunned");
            stunnedLastFrame = true;
        }
    }

    public void resetSurround()
    {
        if (surroundIndex != -1)
        {
            movementQueue.Clear();
            ai.surroundSpotAvailability[surroundIndex] = true;
            surroundSpot = Vector3.zero;
            nextSpot = Vector3.zero;
            surroundTarget = Vector3.zero;
            surroundIndex = -1;
        }
        ThisEnemy.ResetPath();
        ai.enemiesReadyToAttack.Remove(gameObject);
        EnemyAttack = AttackState.Waiting;
        currentRecoveryTime = 0;
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
        // currentAttack = enemyAttacks[0];
        ai.enemiesReadyToAttack.Remove(gameObject);

        Vector3 dirToPlayer = player.transform.position - transform.position;
        float viewingAngleToPlayer = Vector3.Angle(dirToPlayer, transform.forward);
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        int maxPriority = 0; //the total value of all priorities from attacks that can be used

        //loop through attacks and consider attacks which are within the distance + viewing angle
            //select one randomly using attackPriority
        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            if (distanceToPlayer > enemyAttacks[i].minimumAttackRange && distanceToPlayer < enemyAttacks[i].maximumAttackRange)
            {
                if (viewingAngleToPlayer > enemyAttacks[i].minimumAttackAngle && viewingAngleToPlayer < enemyAttacks[i].maximumAttackAngle)
                {
                    maxPriority += enemyAttacks[i].attackPriority;
                }
            }
        }

        int rand = Random.Range(0, maxPriority);
        int tempPriority = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            if (distanceToPlayer > enemyAttacks[i].minimumAttackRange && distanceToPlayer < enemyAttacks[i].maximumAttackRange)
            {
                if (viewingAngleToPlayer > enemyAttacks[i].minimumAttackAngle && viewingAngleToPlayer < enemyAttacks[i].maximumAttackAngle)
                {
                    if (currentAttack != null)
                        return;
                    
                    tempPriority += enemyAttacks[i].attackPriority;

                    if (tempPriority > rand)
                    {
                        currentAttack = enemyAttacks[i];
                    }
                }
            }
        }
        if (currentAttack == null)
        {
            ai.attackingEnemy = null;
            EnemyAttack = AttackState.NotAttacking;
        } else {
            Log(currentAttack.name);
        }
        
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
                //play the animation
                    //while animation is going move enemy forward + activate raycast for hitting player
                swipeAttack();
                Log("Swipe Attack");
            }
            //this is where we would do the animation
                //but instead have to handle it with code
        }
    }

    float yellowTime = 0.25f;
    float orangeTime = 0.15f;
    bool hasHitPlayer = false;

    private void chargeAttack() //-> might need to be an IEnumerator
    {
        alertBox.SetActive(true);
        if (attackTimer < yellowTime) 
        {
            //aim at player
            transform.LookAt(player.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            //set mat to yellow
            GetComponentInChildren<MeshRenderer>().material = alertedMat;
        } 
        else if (attackTimer < yellowTime + orangeTime)
        {
            //aim at player
            transform.LookAt(player.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            //set mat to orange
            GetComponentInChildren<MeshRenderer>().material = enemyAttackingTwoMat;
        }
        else if (attackTimer > yellowTime + orangeTime + 0.05f)
        {
            if (timeCharging == 0)
                generateChargePath();
            GetComponentInChildren<MeshRenderer>().material = enemyAttackingMat;
            timeCharging += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, timeCharging/durationOfCharge);

            RaycastHit hit;
            if (!hasHitPlayer && Physics.Raycast(transform.position, dirVec, out hit, 1f))
            {
                if (hit.collider.tag == "Player")
                {
                    Log("Hit Player, now deal damage");
                    player.GetComponent<PlayerStats>().TakeDamage(GetComponent<CharacterStats>().damage.GetValue());
                    hasHitPlayer = true;
                }
            }

            //exit case
            if (Vector3.Distance(transform.position, endPosition) < 0.1f || attackTimer > yellowTime + orangeTime + 1.5f) {
                alertBox.SetActive(false);
                EnemyAttack = AttackState.NotAttacking;
                ai.attackingEnemy = null;
                timeCharging = 0;
                currentRecoveryTime = currentAttack.recoveryTime;
                enemyCollider.isTrigger = false;
                Log("Lerp Completed");
            }
        }
    }

    private void generateChargePath()
    {
        //turn enemy collider off
        enemyCollider.isTrigger = true;
        dirVec = player.transform.position - transform.position;
        startPosition = transform.position;
        endPosition = transform.position + (dirVec.normalized * chargeLength);
        endPosition.y = transform.position.y;
    }

    private void generateLungePath()
    {
        dirVec = player.transform.position - transform.position;
        startPosition = transform.position;
        endPosition = transform.position + (dirVec.normalized * lungeLength);
        endPosition.y = transform.position.y;
        timeLunging = 0;
        // Note - to lerp, we need a bit more time than in a function
        //transform.LookAt(player.transform.position); //this should LERP not happen instantly
    }

    //Wide swiping arc to punish player for trying to kite enemies
        //compared to the direct and straight pathing of the charge
    
    //play animation
        //activate a raycast out of the enemies hand/arm to detect for the player
        //nudge enemy forward so they lunge at the player
    //once animation is finished set recoveryTime to the current attacks recovery time
        //when the recovery time is reached it auto calls finishAttack();
    private void swipeAttack()
    {
        /*
        //Current Implementation is lacking:
            //a lunge with each swipe
            //a raycast to see if the player has been hit (use hasHitPlayer bool to check if the player has been hit so it doesnt register 100 times)
            //facing the player when swiping (sometimes it faces, but other times its off to the side or something)

        //Next steps:
            //adjust the distance the attack can be activated from
            //adjust the amount of damage the swipes deal compared to the charge
            //adjust the recovery time to ensure enemies dont barrage the player
            //change implementation to swap based off of the animations finishing rather than the attackTimer

        //ITS ALSO POSSIBLE THAT THE ANIMATION NEEDS TO BE TOUCHED UP TO BE A BIT QUICKER BUT FOR NOW I SET THE SPEED TO 4

        //BAD IMPLEMENTATION -> GOT TO DO IT BASED OFF OF WHEN THE ANIMATIONS END
        //GOOD ENOUGH FOR NOW
        //right swipe then left swipe
        */
        RaycastHit hit;
        if (attackTimer < 1.5f)
        {
            if (!right) generateLungePath();
            right = true;
            left = false;
            leftSwipeTrail.SetActive(false);

            // Attack detection starts after windup
            if (attackTimer > 0.75f && attackTimer < 1f)
            {
                //lunge motion
                timeLunging += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, timeLunging/durationOfLunge);

                //hit detection of swipe
                rightSwipeTrail.SetActive(true);
                foreach (Transform originPoint in rightSwipeOriginPoints) {
                    Debug.DrawRay(originPoint.position, originPoint.TransformDirection(Vector3.forward) * 10f, Color.red);
                    if (Physics.SphereCast(originPoint.position, 1f, originPoint.TransformDirection(Vector3.forward), out hit, 10f, swipeLayerMask))
                    {
                        if (!hasHitPlayer) {
                            hasHitPlayer = true;
                            Log("Hit the Player with Right Swipe Attack");
                            hit.transform.gameObject.GetComponent<CharacterStats>().TakeDamage(FindObjectOfType<CharacterStats>().damage.GetValue());
                        }
                    }
                }
            }
            else 
            {
                // Look lerp logic here
            }
        }
        else if (attackTimer < 3.0f)
        {
            if (!left && hasHitPlayer) hasHitPlayer = false; // Reset hitcheck in case the prior swipe hit
            if (!left) generateLungePath();
            left = true;
            right = false;
            rightSwipeTrail.SetActive(false);

            // Attack detection starts after windup
            if (attackTimer > 2.25f && attackTimer < 2.5f)
            {
                //lunge motion
                timeLunging += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, timeLunging/durationOfLunge);                

                //hit detection of swipe
                leftSwipeTrail.SetActive(true);
                foreach (Transform originPoint in leftSwipeOriginPoints) {
                    Debug.DrawRay(originPoint.position, originPoint.TransformDirection(Vector3.forward) * 10f, Color.red);
                    if (Physics.SphereCast(originPoint.position, 1f, originPoint.TransformDirection(Vector3.forward), out hit, 10f, swipeLayerMask))
                    {
                        if(!hasHitPlayer) {
                            hasHitPlayer = true;
                            Log("Hit the Player with Left Swipe Attack");
                            hit.transform.gameObject.GetComponent<CharacterStats>().TakeDamage(FindObjectOfType<CharacterStats>().damage.GetValue());
                        }
                    }
                }
            }
            else 
            {
                // Look lerp logic here
                // Note: LookAt(player) logic doesnt quite work here.
            }
        }
        else if (attackTimer >= 3.0f)
        {
            
            rightSwipeTrail.SetActive(false);
            leftSwipeTrail.SetActive(false);
            left = false;
            hasHitPlayer = false;
            EnemyAttack = AttackState.NotAttacking;
            ai.attackingEnemy = null;
            currentRecoveryTime = currentAttack.recoveryTime;
        }
    }

    private void finishAttack()
    {
        Log("Recovered and ready to attack again");
        EnemyMotion = MotionState.Surrounding;
        EnemyAttack = AttackState.Waiting;
        currentAttack = null;
        attackTimer = 0.0f;
        hasHitPlayer = false;

        //reset surround spot so they dont try to run back through player
        surroundTarget = Vector3.zero;
        surroundSpot = Vector3.zero;
        ai.surroundSpotAvailability[surroundIndex] = true;
        surroundIndex = -1;
        nextSpot = Vector3.zero;
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

    IEnumerator unstuckTimer()
    {
        unstuckingCheck = transform.position;
        yield return new WaitForSeconds(0.5f);
        if(Vector3.Distance(unstuckingCheck, transform.position) < 1.0f)
        {
            // Move enemy towards the shrine in some way
            Log("Help, I'm stuck!");
            unstuckingCheck = Vector3.zero;
            Log(path.status);
        }
        else
        {
            unstuckingCheck = Vector3.zero;
        }
    }

/////////////////////////////////////////////////SPHERECASTING
    private void checkForPlayer()
    {
        foreach (Transform originPoint in rightSwipeOriginPoints) 
        {
            Debug.DrawRay(originPoint.position, originPoint.TransformDirection(Vector3.forward) * targetDetectionRange, Color.red);
            if (Physics.SphereCast(originPoint.position, 1f, originPoint.TransformDirection(Vector3.forward), out hitInfo, targetDetectionRange, swipeLayerMask))
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
