using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject[] playerGeo;
    public GameObject playerTrail;
    [SerializeField] private float DodgeTime = 0.5f;
    [SerializeField] private float DodgeSpeed = 20f;
    [SerializeField] private float AnimationCancelFactor = 3f;
    [SerializeField] public float CombatWalkSpeedDropoff = 5f;
    [SerializeField] public float CombatRunSpeedDropoff = 1f;
    [SerializeField] private float DashAttackSpeed = 45f;
    [SerializeField] private float LungeSpeed = 20f;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isDodging;
    [HideInInspector] public float CombatSpeedDropoff;
    private float dodgeTimeItter = 0;
    private float dodgeCoolDown = 0f;
    private float comboTimeDelay;
    private float totalAnimationTime;
    private int numOfClicks = 0;
    private Animator animator;
    private PlayerController controller;
    private string bufferButton;
    private bool isDead = false;
    private bool animationCancel = false;
    public GameObject BaseSword;
    public Transform[] AttackOriginPoints;
    public List<GameObject> immuneEnemies = new List<GameObject>();
    public Vector3 attackDirection;
    public ParticleSystem Slash;
    private void Start() {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        bufferButton = "";
        playerTrail.SetActive(false);
        
    }
    void Update()
    {
        hadnleBuffer();
        // Gets he total animation time per animation and stores it
        totalAnimationTime = animator.GetCurrentAnimatorStateInfo(2).length;
        // calculates the time that will allow animation cancel to happen
        // If ther player is locked onto a target, they are allowed to dodge
        // After a cool down period
        if(comboTimeDelay < totalAnimationTime){
            comboTimeDelay += Time.deltaTime;
        }
        if((animationCancel || (!isAttacking && dodgeCoolDown <= 0f)) && !isDodging){
            Dodge();
        }
        if((animationCancel || numOfClicks == 0) && !isDodging){
            Attack();
        }
        if(comboTimeDelay >= totalAnimationTime && isAttacking){
            resetAttack();
        }
        if(isAttacking){
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1, Time.deltaTime * 200f));
        }else{
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0f, Time.deltaTime * 200f));
        }
         //Dodge Timers
        if(isDodging && dodgeTimeItter <= 0){
            //makes the player invisible
            controller.RotateOnMoveDirection = true;
            playerTrail.SetActive(false);
            foreach(GameObject parts in playerGeo){
                parts.SetActive(true);
            }
            BaseSword.SetActive(true);
            dodgeCoolDown = 0.5f;
            isDodging = false;
        }else{
            dodgeTimeItter -= Time.deltaTime;
        }
        if(dodgeCoolDown > 0){
            dodgeCoolDown -= Time.deltaTime;
        }
    }
    private void Dodge(){
        if(bufferButton == "Dodge"){
            controller.RotateOnMoveDirection = false;
            resetAttack();
            // makes the player invisible
             foreach(GameObject parts in playerGeo){
                parts.SetActive(false);
            }
            playerTrail.SetActive(true);
            BaseSword.SetActive(false);
            animationCancel = false;
            isDodging = true;
            bufferButton = "";
            controller.TempSpeed = DodgeSpeed;
            dodgeTimeItter = DodgeTime;
        }
    }
    private void Attack(){
        if(bufferButton == "Attack"){
            Slash.Play();
            transform.GetChild(0).gameObject.transform.forward = controller.targetMoveDirection;;
            animationCancel = false;
            bufferButton = "";
            FindObjectOfType<SwordCollision>().immuneEnemies.Clear();
            comboTimeDelay = 0f;
            numOfClicks++;
            if(numOfClicks > 3){
                numOfClicks = 1;
            }
            animator.SetInteger("attackTicks", numOfClicks);
            isAttacking = true;
            if(!isDodging){
                if(isDodging){
                    controller.TempSpeed = controller.speed;
                    CombatSpeedDropoff = CombatRunSpeedDropoff;
                }else{
                    controller.TempSpeed = LungeSpeed;
                    CombatSpeedDropoff = CombatWalkSpeedDropoff;
                }
            }
        }
    }
    private void hadnleBuffer(){
        if(Input.GetButtonDown("X Button") || Input.GetKeyDown(KeyCode.Mouse0)){
            if(comboTimeDelay >= totalAnimationTime/3f){
                bufferButton = "Attack";
            }
            if(isDodging || (!isDodging && dodgeCoolDown >0) || Input.GetButton("A Button") || Input.GetKey(KeyCode.Space)){
                animator.SetBool("isDodging", true);
                bufferButton = "Attack";
            }
        }
        if(Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.Space)){
            if(!isDodging){
                bufferButton = "Dodge";
            }
        }
    }
    private void resetAttack(){
        animator.SetBool("isDodging", false);
        isAttacking = false;
        animationCancel = false;
        numOfClicks = 0;
        comboTimeDelay = 0;
        animator.SetInteger("attackTicks", 0);
        controller.speed = 0.0f;
        controller.targetSpeed = 0.0f;
        CombatSpeedDropoff = 0.0f;
    }
    public void activateSword(){
        GetComponent<CurseMeter>().ActiveSword.GetComponent<SwordCollision>().activateSword();
    }
    public void deactivateSword(){
        GetComponent<CurseMeter>().ActiveSword.GetComponent<SwordCollision>().deactivateSword();
    }
    public void AnimationCancel(){
        animationCancel = true;
    }
}
