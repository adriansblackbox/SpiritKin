using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Battle_Controller : MonoBehaviour
{
    public float DodgeTime = 0;
    private float DodgeCoolDown = 0f;
    public float TotalDodgeTime = 0.2f;
    public float LungeTime = 0;
    private float TotalLungeTime = 0.16f;
    public float LungeCoolDown = 0f;
    private float distanceFromTarget;
    public Vector3 DodgeDirection;
    public GameObject Sword;
    public float AttackCoolDownTime = 1f;
    public int numOfClicks = 0;
    private Animator animator;
    private float ComboTimeDelay;

    private void Start() {
        Sword.GetComponent<Collider>().enabled = false;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if(DodgeTime > 0){
            DodgeTime -= Time.deltaTime;
        }else if(DodgeCoolDown > 0){
            DodgeCoolDown -= Time.deltaTime;
        }

        if(GetComponent<Lock_Target>().Target != null && DodgeCoolDown <= 0)
            Dodge();
        if(GetComponent<Lock_Target>().Target != null){
            distanceFromTarget = (GetComponent<Lock_Target>().Target.position - this.transform.position).magnitude;
        }else{
            distanceFromTarget = 100f;
        }

        if(ComboTimeDelay < 0.9){
            ComboTimeDelay += Time.deltaTime;
        }else{
            numOfClicks = 0;
            animator.SetInteger("attackTicks", numOfClicks);
        }
        if(ComboTimeDelay >= 0.7f )
            Attack();
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
            ComboTimeDelay = 0f;
            Sword.GetComponent<Collider>().enabled = true;
            numOfClicks++;
            animator.SetInteger("attackTicks", numOfClicks);
        }
    }
}
