using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
    Using the camera's script, Lockable Targets, this script
    uses the target found from the camera's view. If a target
    is found, the player displays a set of strafing animations
    and faces the target at all times.
*/
public class LockTarget : MonoBehaviour
{
    [SerializeField] private float NormToCombatSpeed = 5f;
    [SerializeField] private float RotateToTargetSpeed = 10f;
    [SerializeField] private float CamRotateToTargetSpeed = 5f;
    [SerializeField] private Transform LookAtRoot;
    [SerializeField] float LetGoDistance = 10f;
    [SerializeField] private CinemachineVirtualCamera FollowCamera;
    [HideInInspector] public Transform Target = null;
    private PlayerController controller;
    private float inputX;
    private float inputY;
    private Transform playerBody;
    private Animator animator;
    private void Start() {
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        playerBody = transform.GetChild(0).gameObject.transform;
    }

    private void Update(){
        FindTarget();
        if(Target != null){
            LockOnTarget();
        }
        // updates the plaeyr's aniamtion according to input direction. Lerping is
        // used for smooth and organic transitioning between animations
        inputX = Mathf.Lerp(inputX, Input.GetAxis("Horizontal"), Time.deltaTime*10f);
        inputY = Mathf.Lerp(inputY, Input.GetAxis("Vertical"), Time.deltaTime*10f);
        animator.SetFloat("X Direction", inputX * 2f);
        animator.SetFloat("Z Direction", inputY * 2f);
    }
    
    private void LockOnTarget(){
        Vector3 aimTarget = Target.position;
        aimTarget.y = transform.position.y;
        Vector3 focusDirection = (aimTarget - transform.position).normalized;
        // posiotions the camera slightly above the player's body, and rotates to face the target at adjustable speeds
        Vector3 focusTarget = (Target.position -controller.CinemachineCameraTarget.transform.position).normalized;
        focusTarget = new Vector3(focusTarget.x, focusTarget.y - 0.15f, focusTarget.z);
        controller.CinemachineCameraTarget.transform.forward =
        Vector3.Lerp(GetComponent<PlayerController>().CinemachineCameraTarget.transform.forward, focusTarget, Time.deltaTime * CamRotateToTargetSpeed);
        // makes sure that the camera will be consistent with the locked rotation when delocked
        controller.CinemachineTargetPitch =controller.CinemachineCameraTarget.transform.rotation.eulerAngles.x;
        controller.CinemachineTargetYaw =controller.CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        // Cancel lock
        if((this.transform.position - Target.transform.position).magnitude > LetGoDistance){
            DelockTarget();
        }
    }
    public void DelockTarget(){
        Target = null;
        FindObjectOfType<LockableTargets>().ClearTargetList();
        transform.forward =  playerBody.forward;
        playerBody.forward = transform.forward;
    }

    private void FindTarget(){
        if(Input.GetButtonDown("Right Stick Button") || Input.GetKeyDown(KeyCode.Mouse2)){
            if(Target != null)
                DelockTarget();
            else
                Target = FindObjectOfType<LockableTargets>().AssessTarget();
        }
    }
}
