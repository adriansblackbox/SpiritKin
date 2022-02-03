using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Battle_Controller : MonoBehaviour
{
    public float DodgeTime = 0;
    private float DodgeCoolDown = 0f;
    public float TotalDodgeTime = 0.2f;
    public float distanceFromTarget;
    public Vector3 DodgeDirection;
    public GameObject Sword;
    public int numOfClicks = 0;
    private Animator animator;
    public float ComboTimeDelay;
    private float TotalAnimationTime;
    public bool isAttacking = false;
    public Vector3 LungeDirection;
    private Player_Controller _controller;

    private void Start() {
        Sword.GetComponent<Collider>().enabled = false;
        animator = GetComponent<Animator>();
        _controller = GetComponent<Player_Controller>();
    }
    void Update()
    {
        // If ther player is locked onto a target, they are allowed to dodge
        // After a cool down period
        if(GetComponent<Lock_Target>().Target != null && DodgeCoolDown <= 0 && !isAttacking)
            Dodge();
        if((ComboTimeDelay >= TotalAnimationTime - (TotalAnimationTime/2) || numOfClicks == 0) && DodgeTime <= 0)
            Attack();
        //Dodge Timers
        if(DodgeTime > 0){
            DodgeTime -= Time.deltaTime;
        }else if(DodgeCoolDown > 0){
            DodgeCoolDown -= Time.deltaTime;
        }
        // Tracking distance away from target
        if(GetComponent<Lock_Target>().Target != null){
            distanceFromTarget = (GetComponent<Lock_Target>().Target.position - this.transform.position).magnitude;
        }else{
            distanceFromTarget = 100f;
        }

        // Handels animating combos
        if(FindObjectOfType<Lock_Target>().Target == null){
            TotalAnimationTime = animator.GetCurrentAnimatorStateInfo(0).length;
        }else{
            TotalAnimationTime = animator.GetCurrentAnimatorStateInfo(1).length;
        }
        if(ComboTimeDelay < TotalAnimationTime){
            ComboTimeDelay += Time.deltaTime;
        }else{
            isAttacking = false;
            numOfClicks = 0;
            ComboTimeDelay = 0;
            animator.SetInteger("attackTicks", numOfClicks);
        }
    }
    private void Dodge(){
        if(Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.LeftShift)){
            DodgeTime = TotalDodgeTime;
            DodgeCoolDown = 1f;
            DodgeDirection = GetComponent<Player_Controller>().targetDirection;
        }
    }
    private void Attack(){
        if(Input.GetButton("X Button") || Input.GetKey(KeyCode.Mouse0)){
            if((Input.GetButton("A Button") || Input.GetKey(KeyCode.LeftShift)) && GetComponent<Lock_Target>().Target == null){
                //Dash Attack
                _controller.TempSpeed = 100f;
            }else{
                //Normal Attack
                _controller.TempSpeed = 20f;
            }
            if( GetComponent<Lock_Target>().Target != null && DodgeCoolDown >= 0.8f && DodgeTime <=0){
                _controller.TempSpeed = 100f;
            }
            LungeDirection = transform.forward;
            isAttacking = true;
            ComboTimeDelay = 0f;
            Sword.GetComponent<Collider>().enabled = true;
            numOfClicks++;
            if(numOfClicks > 3){
                numOfClicks = 1;
            }
            animator.SetInteger("attackTicks", numOfClicks);
        }
    }
}
