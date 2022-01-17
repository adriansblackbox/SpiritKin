using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Battle_Controller : MonoBehaviour
{
    public float DodgeTime = 0;
    private float DodgeCoolDown = 0f;
    public float TotalDodgeTime = 0.2f;
    public float LungeTime = 0;
    private float TotalLungeTime = 0.3f;
    public float LungeCoolDown = 0f;
    public Vector3 DodgeDirection;
    public GameObject Sword;

    private void Start() {
        Sword.GetComponent<Collider>().enabled = false;
    }
    void Update()
    {
        if(DodgeTime > 0){
            DodgeTime -= Time.deltaTime;
        }else if(DodgeCoolDown > 0){
            DodgeCoolDown -= Time.deltaTime;
        }
        if(LungeTime > 0){
            LungeTime -= Time.deltaTime;
        }else if(LungeCoolDown > 0){
            LungeCoolDown -= Time.deltaTime;
            Sword.GetComponent<Collider>().enabled = false;
        }

        if(GetComponent<Lock_Target>().Target != null && DodgeCoolDown <= 0)
            Dodge();
        if(LungeCoolDown <= 0)
            Attack();
    }
    private void Dodge(){
        if(Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.LeftShift)){
            DodgeTime = TotalDodgeTime;
            DodgeCoolDown = 1f;
            DodgeDirection = GetComponent<Player_Controller>().moveDirection;
        }
    }
    private void Attack(){
        if(Input.GetButtonDown("X Button") || Input.GetKeyDown(KeyCode.Mouse0)){
            LungeTime = TotalLungeTime;
            LungeCoolDown = 0.1f;
            Sword.GetComponent<Collider>().enabled = true;
        }
    }
}
