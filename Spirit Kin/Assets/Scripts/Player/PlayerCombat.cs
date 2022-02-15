using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float DodgeTime = 0.5f;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isDodging = false;
    private float dodgeTimeItter = 0;
    private float dodgeCoolDown = 0f;
    private float comboTimeDelay;
    private float totalAnimationTime;
    private int numOfClicks = 0;
    private Animator animator;
    private PlayerController controller;

    private void Start() {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
    }
    void Update()
    {
        // If ther player is locked onto a target, they are allowed to dodge
        // After a cool down period
        if(GetComponent<LockTarget>().Target != null && dodgeCoolDown <= 0.0f && !isAttacking){
            Dodge();
        }
        if((comboTimeDelay >= totalAnimationTime - (totalAnimationTime/2)  || numOfClicks == 0) && dodgeTimeItter <= 0.0f){
            Attack();
        }
        //Dodge Timers
        if(dodgeTimeItter > 0){
            dodgeTimeItter -= Time.deltaTime;
        }else if(dodgeCoolDown > 0){
            dodgeCoolDown -= Time.deltaTime;
            isDodging = false;
        }
        // Handels animating combos
        if(FindObjectOfType<LockTarget>().Target == null){
            totalAnimationTime = animator.GetCurrentAnimatorStateInfo(0).length;
        }else{
            totalAnimationTime = animator.GetCurrentAnimatorStateInfo(1).length;
        }
        if(comboTimeDelay < totalAnimationTime){
            comboTimeDelay += Time.deltaTime;
        }else{
            isAttacking = false;
            numOfClicks = 0;
            comboTimeDelay = 0;
            animator.SetInteger("attackTicks", numOfClicks);
        }
    }
    private void Dodge(){
        if(Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.LeftShift)){
            if(isAttacking){
                isAttacking = false;
            }
            controller.TempSpeed = 80f;
            isDodging = true;
            dodgeTimeItter = DodgeTime;
            dodgeCoolDown = 0.5f;
        }
    }
    private void Attack(){
        if(Input.GetButton("X Button") || Input.GetKey(KeyCode.Mouse0)){
            isAttacking = true;
            comboTimeDelay = 0f;
            numOfClicks++;
            if(numOfClicks > 3){
                numOfClicks = 1;
            }
            animator.SetInteger("attackTicks", numOfClicks);
            // lunge movement
            if((Input.GetButton("A Button") || Input.GetKey(KeyCode.LeftShift)) && GetComponent<LockTarget>().Target == null
                && controller.speed > controller.WalkSpeed){
                // dash Attack
                controller.TempSpeed = 90f;
            }else{
                // normal Attack
                controller.TempSpeed = 20f;
            }
            if( GetComponent<LockTarget>().Target != null && dodgeCoolDown > 0.0f && dodgeTimeItter <= 0.0f){
                // critical Attack
                controller.TempSpeed = 60f;
            }
        }
    }
}
