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

    private void Start() {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
    }
    void Update()
    {
        // Gets he total animation time per animation and stores it
        if(FindObjectOfType<LockTarget>().Target == null){
            totalAnimationTime = animator.GetCurrentAnimatorStateInfo(0).length;
        }else{
            totalAnimationTime = animator.GetCurrentAnimatorStateInfo(1).length;
        }
        // calculates the time that will allow animation cancel to happen
        cancelAnimationTime = totalAnimationTime - (totalAnimationTime/AnimationCancelFactor);
        // If ther player is locked onto a target, they are allowed to dodge
        // After a cool down period
        if(GetComponent<LockTarget>().Target != null && dodgeCoolDown <= 0.0f && !isAttacking){
            Dodge();
        }
        if((comboTimeDelay >= cancelAnimationTime || numOfClicks == 0) && !isDodging){
            Attack();
        }else if(comboTimeDelay < totalAnimationTime){
            comboTimeDelay += Time.deltaTime;
        }
         //Dodge Timers
        if(dodgeTimeItter > 0){
            dodgeTimeItter -= Time.deltaTime;
        }else if(dodgeCoolDown > 0){
            isDodging = false;
            dodgeCoolDown -= Time.deltaTime;
        }
    }
    private void Dodge(){
        if(Input.GetButtonDown("B Button") || Input.GetKeyDown(KeyCode.Space)){
            controller.TempSpeed = DodgeSpeed;
            isDodging = true;
            dodgeTimeItter = DodgeTime;
            dodgeCoolDown = 0.5f;
        }
    }
    private void Attack(){
        if(Input.GetButton("X Button") || Input.GetKey(KeyCode.Mouse0)){
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
            if( GetComponent<LockTarget>().Target != null){
                // lunge forward
                controller.TempSpeed = LungeSpeed;
            }
            if(controller.speed > controller.WalkSpeed){
                CombatSpeedDropoff = CombatRunSpeedDropoff;
                controller.TempSpeed = DashAttackSpeed;
            }else{
                CombatSpeedDropoff = CombatWalkSpeedDropoff;
            }
            controller.speed = 0.0f;
            controller.targetSpeed = 0.0f;
        }
        // Handels animating combos
        else if(((new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) != Vector2.zero && comboTimeDelay >= cancelAnimationTime) 
            || comboTimeDelay >= totalAnimationTime)
            && isAttacking
        ){
            isAttacking = false;
            numOfClicks = 0;
            comboTimeDelay = 0;
            animator.SetInteger("attackTicks", numOfClicks);
            controller.speed = 0.0f;
            controller.targetSpeed = 0.0f;
             CombatSpeedDropoff = 0.0f;
        }
    }
}
