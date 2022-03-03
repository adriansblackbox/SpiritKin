using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float DodgeTime = 0.5f;
    [SerializeField] private float DodgeSpeed = 20f;
    [SerializeField] private float AnimationCancelFactor = 3f;
    [SerializeField] public float CombatWalkSpeedDropoff = 5f;
    [SerializeField] public float CombatRunSpeedDropoff = 1f;
    [SerializeField] private float DashAttackSpeed = 45f;
    [SerializeField] private float LungeSpeed = 20f;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isDodging = false;
    [HideInInspector] public float CombatSpeedDropoff;
    private float dodgeTimeItter = 0;
    private float dodgeCoolDown = 0f;
    private float comboTimeDelay;
    private float totalAnimationTime;
    private float cancelAnimationTime;
    private int numOfClicks = 0;
    private Animator animator;
    private PlayerController controller;
    public GameObject sword0, sword1, sword2, sword3;
    private string bufferButton;

    private void Start() {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        bufferButton = "";
    }
    void Update()
    {
        hadnleBuffer();
        // Gets he total animation time per animation and stores it
        totalAnimationTime = animator.GetCurrentAnimatorStateInfo(2).length;
        // calculates the time that will allow animation cancel to happen
        cancelAnimationTime = totalAnimationTime - (totalAnimationTime/AnimationCancelFactor);
        // If ther player is locked onto a target, they are allowed to dodge
        // After a cool down period
        if(dodgeCoolDown <= 0.0f && !isAttacking){
            Dodge();
        }
        if((comboTimeDelay >= cancelAnimationTime || numOfClicks == 0) && !isDodging){
            Attack();
        }
        if(comboTimeDelay < totalAnimationTime){
            comboTimeDelay += Time.deltaTime;
        }
         // Handels animating combos
        if(comboTimeDelay >= totalAnimationTime && isAttacking){
            isAttacking = false;
            numOfClicks = 0;
            comboTimeDelay = 0;
            animator.SetInteger("attackTicks", 0);
            controller.speed = 0.0f;
            controller.targetSpeed = 0.0f;
            CombatSpeedDropoff = 0.0f;
        }
        if(isAttacking){
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1, Time.deltaTime * 20f));
        }else{
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0f, Time.deltaTime * 20f));
        }
         //Dodge Timers
        if(dodgeTimeItter > 0){
            dodgeTimeItter -= Time.deltaTime;
        }else if(dodgeCoolDown > 0){
            isDodging = false;
            dodgeCoolDown -= Time.deltaTime;
        }
        animator.SetBool("isDodging", isDodging);
    }
    private void Dodge(){
        if(bufferButton == "Dodge"){
            bufferButton = "";
            controller.TempSpeed = DodgeSpeed;
            isDodging = true;
            dodgeTimeItter = DodgeTime;
            dodgeCoolDown = 0.5f;
        }
    }
    private void Attack(){
        if(bufferButton == "Attack"){
            bufferButton = "";
            FindObjectOfType<SwordCollision>().immuneEnemies.Clear();
            isAttacking = true;
            comboTimeDelay = 0f;
            numOfClicks++;
            controller.TempSpeed =0;
            if(numOfClicks > 3){
                numOfClicks = 1;
            }
            animator.SetInteger("attackTicks", numOfClicks);
            controller.TempSpeed = controller.targetSpeed;
            controller.TempSpeed = LungeSpeed;
            CombatSpeedDropoff = CombatWalkSpeedDropoff;
            if((Input.GetKey(KeyCode.LeftShift) || Input.GetButton("A Button")) && controller.speed > controller.WalkSpeed){
                CombatSpeedDropoff = CombatRunSpeedDropoff;
                controller.TempSpeed = DashAttackSpeed;
            }
        }
    }
    private void hadnleBuffer(){
        if(Input.GetButtonDown("X Button") || Input.GetKeyDown(KeyCode.Mouse0)){
            if(comboTimeDelay >= totalAnimationTime/4f)
                bufferButton = "Attack";
        }
        if(Input.GetButtonDown("B Button") || Input.GetKeyDown(KeyCode.Space)){
            bufferButton = "Dodge";
        }
    }
    public void activateSword(){
        GetComponent<CurseMeter>().ActiveSword.GetComponent<SwordCollision>().activateSword();
    }
    public void deactivateSword(){
        GetComponent<CurseMeter>().ActiveSword.GetComponent<SwordCollision>().deactivateSword();
    }
}
