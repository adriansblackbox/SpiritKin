using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Controller : MonoBehaviour
{
    #region States

    public enum MotionState {
        Idling,
        Relocating,
        Chasing,
        Surrounding,
        Stunned,
        Waiting
    }

    public enum AttackState {
        Attacking,
        Recovering,
        Waiting
    }

    public Animator enemyAnimator;
    private string selectedStunAnim;

    [Header("Attacks")]

    [SerializeField] private float recoveryTimer;

    private bool hasHitPlayer = false;
    [SerializeField] bool combo;
    public Enemy_Attack currentAttack;
    public Enemy_Attack[] enemyAttacks;
    public Material enemyAttackingMat;
    public Material enemyAttackingTwoMat;    
    public BoxCollider enemyCollider;
    public bool hitEnabled;
    public bool tracking = true;
    
    [Tooltip("Determines Speed of Charge")]
    public float durationOfCharge = 0.5f;
    [Tooltip("Determines Length of Charge")]
    public float chargeLength = 20f;
    [Tooltip("Determines Speed of Lunge")]
    public float durationOfLunge = 0.25f;
    [Tooltip("Determines Length of Lunge")]
    public float lungeLength = 7.5f;
    [Tooltip("Determines Speed of KnockBack")]
    public float durationOfKnockBack = 0.1f;
    
    private Vector3 dirVec;
    Vector3 startPosition = Vector3.zero;
    Vector3 endPosition = Vector3.zero;
    float timeCharging = 0;
    float timeLunging = 0;
    float timeSlerping = 0;
    float timeKnockingBack = 0;
    float durationOfSlerp = 0.2f;
    float knockBackStrength;

    public Transform[] rightSwipeOriginPoints;
    public Transform[] leftSwipeOriginPoints;
    public LayerMask swipeLayerMask;

    public GameObject leftSwipeTrail;
    public GameObject rightSwipeTrail;    

    [Header("States")]
    public MotionState EnemyMotion;
    public AttackState EnemyAttack;

    public bool stunned = false;

    private string[] stunAnimTriggers = { "Stun 1L", "Stun 1R", "Stun 2L", "Stun 2R"};

    private Vector3 lastLooking = Vector3.zero;
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

    public List<Vector3> movementQueue;


    private float myTime = 0.0f;
    private Vector3 startOfPath;

    public Transform shrine;

    [SerializeField] int quadrant;
    public AI_Manager ai;
    public Enemy_Spawner es;

    //check if enemy has left arena
    public bool exitedArena;

    public float surroundSpeed;
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
    public Transform[] sideAndBackDetectionOrigins;
    public float raycastRadius;
    public float targetDetectionRange;

    private RaycastHit hitInfo;

///////////////////////////////////////////////// STATE BOX FOR TESTING
    [Header("Debugging")]
    public bool showLogs = true;

    public Material alertedMat;
    public Material chasingMat;
    public Material idleMat;
    public Material patrolMat;
    public Material relocateMat;
    public Material surroundMat;
    public Material attackMat;
    public Material recoverMat;

    // Start is called before the first frame update
    public GameObject LockOnArrow;
    void Start()
    {
        path = new UnityEngine.AI.NavMeshPath();
        ThisEnemy = GetComponent<UnityEngine.AI.NavMeshAgent>();
        EnemyMotion = MotionState.Idling;
        player = GameObject.Find("Player");
        es = GameObject.Find("ShrineManager").GetComponent<Enemy_Spawner>();
        ai = shrine.GetComponent<AI_Manager>();
        enemyAnimator = GetComponent<Animator>();
        determineQuadrant();
        movementQueue = new List<Vector3>();
        LockOnArrow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        //spherecast to check for player
        checkForPlayer();
        myTime += Time.deltaTime;

        //we will want a function to handle the one time reset of values when moved to notAttacking
        switch (EnemyAttack)
        {
            case AttackState.Attacking:
                if (currentAttack == null)
                    getAttack();
                attackPlayer();
                break;
            case AttackState.Recovering:
                attackRecovery();
                break;
            case AttackState.Waiting:
                break;
        }

        switch (EnemyMotion)
        {
            case MotionState.Idling:
                ThisEnemy.speed = chaseSpeed;
                ThisEnemy.stoppingDistance = 2.5f;
                //If enemy has reached destination find next waypoint
                if (ThisEnemy.remainingDistance <= ThisEnemy.stoppingDistance) {
                    ThisEnemy.SetDestination(findNextWaypoint());
                }
                break;

            case MotionState.Relocating:
                ThisEnemy.speed = chaseSpeed + 5f;
                ThisEnemy.stoppingDistance = 5f;

                //Debugging
                if (unstuckingCheck == Vector3.zero) {
                    StartCoroutine(unstuckTimer());
                }

                //If enemy has reached destination set EnemyMotion -> MotionState.Idling
                if (path.status == NavMeshPathStatus.PathComplete && ThisEnemy.remainingDistance < ThisEnemy.stoppingDistance)
                {
                    ThisEnemy.ResetPath();
                    changeState(MotionState.Idling);
                    break;
                }

                //If relocate spot is invalid select another
                if (relocateSpot == Vector3.zero)
                    relocateSpot = es.chooseLocation(shrine).position;
                else
                    ThisEnemy.CalculatePath(relocateSpot, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                    ThisEnemy.SetDestination(relocateSpot);
                break;

            case MotionState.Chasing:
                ThisEnemy.speed = chaseSpeed;
                ThisEnemy.stoppingDistance = 10f;

                List<GameObject> nearbyEnemies = ai.getNearbyEnemies(gameObject);
                foreach (GameObject gameObj in nearbyEnemies)
                {
                    gameObj.SendMessage("changeState", MotionState.Chasing);
                }

                //If enemy has left the arena set EnemyMotion -> MotionState.Relocating
                if (exitedArena)
                {
                    ThisEnemy.ResetPath();
                    exitedArena = false;
                    changeState(MotionState.Relocating);
                    break;
                }

                //If the enemy's distance to the player is less than breakDist EnemyMotion -> MotionState.Surrounding
                if (Vector3.Distance(player.transform.position, transform.position) < breakDist - 1f)
                {
                    changeState(MotionState.Surrounding);
                    ThisEnemy.ResetPath();
                    break;
                }

                //Calculate path to the player
                ThisEnemy.CalculatePath(player.transform.position, path);
                if (path.status == NavMeshPathStatus.PathComplete) {
                    ThisEnemy.SetDestination(player.transform.position);
                }
                break;

            case MotionState.Surrounding:
                ThisEnemy.speed = chaseSpeed;
                ThisEnemy.stoppingDistance = 5f;

                //replace with slerp
                if (timeSlerping < durationOfSlerp / 2)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirVec), timeSlerping);
                    transform.rotation = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f));
                    timeSlerping += Time.deltaTime;
                }
                else
                {
                    transform.LookAt(player.transform.position);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                }

                //If enemy has left the arena set EnemyMotion -> MotionState.Relocating
                if (exitedArena)
                {
                    resetSurround();
                    exitedArena = false;
                    changeState(MotionState.Relocating);
                    break;
                }

                //If the enemy's distance to the player is greater than breakDist EnemyMotion -> MotionState.Chasing
                if (Vector3.Distance(player.transform.position, transform.position) > breakDist + 1f)
                {
                    changeState(MotionState.Chasing);
                    resetSurround();
                    break;
                }

                if (movementQueue != null) 
                {
                    //Calculate path to SurroundSpot
                    if (!(GetComponent<CharacterStats>().isDying) && movementQueue.Count == 0 && surroundSpot == Vector3.zero) {
                        movementQueue = ai.determineSurroundSpot(transform);
                        if (movementQueue.Count == 0) {
                            changeState(MotionState.Relocating);
                            break;
                        }
                    }

                    if (ThisEnemy.remainingDistance < ThisEnemy.stoppingDistance && movementQueue.Count > 0) //Direct enemy along their movementQueue
                    {
                        nextSpot = movementQueue[0];
                        movementQueue.RemoveAt(0);
                        ThisEnemy.speed = chaseSpeed;
                    } 
                    else if (ThisEnemy.remainingDistance < ThisEnemy.stoppingDistance && movementQueue.Count == 0 && !ai.enemiesReadyToAttack.Contains(gameObject))
                    {
                        ai.enemiesReadyToAttack.Add(gameObject);
                        ThisEnemy.speed = surroundSpeed / 1.4f;
                    }
                }
                else 
                {
                    Log("Movement Queue is null");
                }

                //Ensure that the enemy stays on the navmesh
                NavMeshHit hit;
                NavMesh.SamplePosition(nextSpot + player.transform.position, out hit, 400.0f, NavMesh.AllAreas);
                ThisEnemy.CalculatePath(hit.position, path); //might need to do the find spot Navmesh thing if doesnt work
                if (path.status == NavMeshPathStatus.PathComplete) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                    ThisEnemy.SetDestination(hit.position);
                }
                break;
            
            case MotionState.Stunned:
                
                Log("Stunned");
                transform.LookAt(lastLooking);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                if (timeKnockingBack < durationOfKnockBack)
                {
                    timeKnockingBack += Time.deltaTime;
                    transform.position = Vector3.Lerp(startPosition, endPosition, timeKnockingBack/durationOfKnockBack);
                }
                break;

            case MotionState.Waiting:
                break;
        }
    }

    public void changeState(MotionState targetState)
    {
        if (EnemyMotion == targetState) return;

        if (EnemyMotion == MotionState.Surrounding) 
        {
            ai.enemiesReadyToAttack.Remove(gameObject);
            dirVec = player.transform.position - transform.position;
        }

        if (targetState == MotionState.Idling)
            ai.enemiesIdling.Add(gameObject);
        else
            ai.enemiesIdling.Remove(gameObject);

        if (targetState == MotionState.Stunned && EnemyMotion == MotionState.Surrounding) resetSurround();

        if (EnemyMotion == MotionState.Stunned) resetKnockback();

        EnemyMotion = targetState;

        Log("Changed Motion State -> " + targetState);
    }

    public void beginStun()
    {
        changeState(MotionState.Stunned);
        enemyAnimator.SetBool("Stunned", true);

        //select which stun animation will be played
        int temp = Random.Range(0, stunAnimTriggers.Length);
        selectedStunAnim = stunAnimTriggers[temp];
        enemyAnimator.SetTrigger(selectedStunAnim);
        lastLooking = player.transform.position;
    }

    public void changeStun()
    {
        enemyAnimator.SetBool("InStun", false);
        enemyAnimator.ResetTrigger(selectedStunAnim);

        //select which stun animation will be played
        int temp = Random.Range(0, stunAnimTriggers.Length);
        selectedStunAnim = stunAnimTriggers[temp];
        enemyAnimator.SetTrigger(selectedStunAnim);
    }

    public void startOfStun()
    {
        enemyAnimator.SetBool("InStun", true);
    }

    public void endStun()
    {
        changeState(MotionState.Chasing);
        enemyAnimator.SetBool("InStun", false);
        enemyAnimator.SetBool("Stunned", false);
        enemyAnimator.ResetTrigger(selectedStunAnim);
    }

    public void resetKnockback()
    {
        timeKnockingBack = 0;
        dirVec = Vector3.zero;
        endPosition = Vector3.zero;
        startPosition = Vector3.zero;
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
    }

    #region Attacks

    // Animation State Machine Events//////////////////////////////////////////////////

    //0 = charge
    //1 = left swipe
    //2 = right swipe

    private void AttackStart()
    {
        enemyAnimator.SetBool("Attack", true);
        dirVec = player.transform.position - transform.position;
        tracking = true;
    }

    private void AttackEnd()
    {
        enemyAnimator.SetBool("Attack", false);
        leftSwipeTrail.SetActive(false);
        rightSwipeTrail.SetActive(false);
        timeLunging = 0;
        timeCharging = 0;
        timeSlerping = 0;

        endPosition = Vector3.zero;
        startPosition = Vector3.zero;
        dirVec = Vector3.zero;

        if (currentAttack.attackNumber == 3)
        {
            EnemyAttack = AttackState.Recovering;
            return;
        }
        else if (Vector3.Distance(player.transform.position, transform.position) < 15f && !combo)
        {
            enemyAnimator.SetBool("PlayerInRange", true);
            combo = true;
            hasHitPlayer = false;
            
            //if comboing need to change the attack
            if (currentAttack.attackNumber == 1)
                currentAttack = enemyAttacks[2];
            else
                currentAttack = enemyAttacks[1];
        }
        else 
        {
            enemyAnimator.SetBool("PlayerInRange", false);
            combo = false;
            EnemyAttack = AttackState.Recovering;
        }
    }

    private void attackRecovery()
    {
        if (recoveryTimer < currentAttack.recoveryTime)
        {
            recoveryTimer += Time.deltaTime;
        }
        else
        {
            finishAttack();
        }
    }

    private void finishAttack()
    {
        if (exitedArena)
        {
            exitedArena = false;
            changeState(MotionState.Relocating);
        }
        else
        {
            changeState(MotionState.Surrounding);
        }

        EnemyAttack = AttackState.Waiting;
        currentAttack = null;
        hasHitPlayer = false;
        ai.attackingEnemy = null;
        enemyCollider.isTrigger = false;
        recoveryTimer = 0;

        //reset surround spot so they dont try to run back through player
        ThisEnemy.ResetPath();
        surroundTarget = Vector3.zero;
        surroundSpot = Vector3.zero;
        if (surroundIndex != -1)
            ai.surroundSpotAvailability[surroundIndex] = true;
        surroundIndex = -1;
        nextSpot = Vector3.zero;
    }

    private void EnableHit()
    {
        hitEnabled = true;
        tracking = false;
    }

    private void DisableHit()
    {
        hitEnabled = false;
    }

    private void attackPlayer()
    {
        //slerp to align with player
        if (timeSlerping < durationOfSlerp)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirVec), timeSlerping);
            transform.rotation = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f));
            timeSlerping += Time.deltaTime;
        }
        else if (tracking) //issue with this is after the enemy has thier hit disabled it looks snappy and unrealistic + the first moment this is used it looks unrealistic
        {
            transform.LookAt(player.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);            
        }

        if (hitEnabled)
        {
            switch (currentAttack.attackNumber)
            {
                case 1:
                    if (Mathf.Approximately(endPosition.x, 0f))
                        generateLungePath();
                    leftSwipeAttack();
                    break;
                case 2:
                    if (Mathf.Approximately(endPosition.x, 0f))
                        generateLungePath();
                    rightSwipeAttack();
                    break;
                case 3:
                    if (Mathf.Approximately(endPosition.x, 0f))
                        generateChargePath();
                    chargeAttack();
                    break;
            }
        }
    }
        
    private void getAttack()
    {
        Log("Entered getAttack()");
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
                        break;
                    
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
            Log("Unable to Find an Attack so will do swipe");
            // ai.attackingEnemy = null;
            // EnemyAttack = AttackState.Waiting;

            currentAttack = enemyAttacks[1];

        }
        enemyAnimator.SetInteger("Attack Number", currentAttack.attackNumber);
        enemyAnimator.SetBool("Attack", true);
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
    }

    //first time through will be a consistent speed the entire time, but polish would have deccelaration
    public void GenerateKnockBack(float strength) {
        dirVec = transform.position - player.transform.position;
        startPosition = transform.position;
        endPosition = transform.position + (dirVec.normalized * strength);
        endPosition.y = transform.position.y;
        knockBackStrength = strength;
    }

    

    private void chargeAttack()
    {
        timeCharging += Time.deltaTime;
        transform.position = Vector3.Lerp(startPosition, endPosition, timeCharging/durationOfCharge);

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 2f, dirVec, out hit, 1f, swipeLayerMask)) //might need detection to be more robust
        {
            if (!hasHitPlayer)
            {
                hasHitPlayer = true;
                Log("Hit Player with Charge Attack");
                player.GetComponent<PlayerStats>().TakeDamage(GetComponent<CharacterStats>().damage.GetValue(), 0);
            } 
        }
    }

    private void rightSwipeAttack()
    {
        rightSwipeTrail.SetActive(true);
        RaycastHit hit;
        
        //Lunge motion
        timeLunging += Time.deltaTime;
        transform.position = Vector3.Lerp(startPosition, endPosition, timeLunging/durationOfLunge);

        //Hit detection of swipe
        foreach (Transform originPoint in rightSwipeOriginPoints) {
            Debug.DrawRay(originPoint.position, originPoint.TransformDirection(Vector3.forward) * 7.5f, Color.red);
            if (Physics.SphereCast(originPoint.position, 1f, originPoint.TransformDirection(Vector3.forward), out hit, 7.5f, swipeLayerMask))
            {
                if (!hasHitPlayer) 
                {
                    hasHitPlayer = true;
                    Log("Hit the Player with Right Swipe Attack");
                    hit.transform.gameObject.GetComponent<CharacterStats>().TakeDamage(FindObjectOfType<CharacterStats>().damage.GetValue(), 0);
                }
            }
        }
    }

    private void leftSwipeAttack()
    {
        leftSwipeTrail.SetActive(true);
        RaycastHit hit;

        //Lunge motion
        timeLunging += Time.deltaTime;
        transform.position = Vector3.Lerp(startPosition, endPosition, timeLunging/durationOfLunge);                

        //Hit detection of swipe
        foreach (Transform originPoint in leftSwipeOriginPoints) {
            Debug.DrawRay(originPoint.position, originPoint.TransformDirection(Vector3.forward) * 7.5f, Color.red);
            if (Physics.SphereCast(originPoint.position, 1f, originPoint.TransformDirection(Vector3.forward), out hit, 7.5f, swipeLayerMask))
            {
                if(!hasHitPlayer) 
                {
                    hasHitPlayer = true;
                    Log("Hit the Player with Left Swipe Attack");
                    hit.transform.gameObject.GetComponent<CharacterStats>().TakeDamage(FindObjectOfType<CharacterStats>().damage.GetValue(), 0);
                }
            }
        }
    }

    #endregion

    private void DieAndDestroy()
    {
        player.GetComponent<PlayerStats>().coins += GetComponent<CharacterStats>().coins;
        player.GetComponent<CurseMeter>().curseMeter += (float) GetComponent<CharacterStats>().fillAmount / player.GetComponent<CurseMeter>().fillRate;
        Destroy(this.gameObject);
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

        return es.chooseLocation(shrine).position;        
    }

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
        foreach (Transform originPoint in visionFanOrigins)
        {
            Debug.DrawRay(originPoint.position, originPoint.TransformDirection(Vector3.forward) * targetDetectionRange, Color.red);
            if (Physics.SphereCast(originPoint.position, 1f, originPoint.TransformDirection(Vector3.forward), out hitInfo, targetDetectionRange, swipeLayerMask))
            {
                if (EnemyMotion == MotionState.Idling)
                {
                    Log("Player Detected!");
                    ThisEnemy.ResetPath();
                    EnemyMotion = MotionState.Chasing;
                }
            }
        }

        foreach (Transform originPoint in sideAndBackDetectionOrigins)
        {
            Debug.DrawRay(originPoint.position, originPoint.TransformDirection(Vector3.forward) * targetDetectionRange / 2.5f, Color.red);
            if (Physics.SphereCast(originPoint.position, 1f, originPoint.TransformDirection(Vector3.forward), out hitInfo, targetDetectionRange / 2.5f, swipeLayerMask))
            {
                if (EnemyMotion == MotionState.Idling)
                {
                    Log("Player Detected!");
                    ThisEnemy.ResetPath();
                    EnemyMotion = MotionState.Chasing;
                }
            }
        }
    }

    private void Log(object message)
    {
        if (showLogs)
            Debug.Log(message);
    }
}
