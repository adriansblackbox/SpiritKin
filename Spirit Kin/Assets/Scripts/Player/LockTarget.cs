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
    [SerializeField] private CinemachineVirtualCamera FollowCamera;
    [HideInInspector] public Transform Target = null;
    private float defaultSprintSpeed;
    private float inputX;
    private float inputY;
    private Transform playerBody;
    private Animator animator;
    private void Start() {
        defaultSprintSpeed =  GetComponent<PlayerController>().SprintSpeed;
        animator = GetComponent<Animator>();
        playerBody = transform.GetChild(0).gameObject.transform;
    }

    private void Update(){
        if(Target != null){
            LockOnTarget();
        }else{
            DelockTarget();
            FindTarget();
        }
        // updates the plaeyr's aniamtion according to input direction. Lerping is
        // used for smooth and organic transitioning between animations
        inputX = Mathf.Lerp(inputX, Input.GetAxis("Horizontal"), Time.deltaTime*10f);
        inputY = Mathf.Lerp(inputY, Input.GetAxis("Vertical"), Time.deltaTime*10f);
        animator.SetFloat("X Direction", inputX * 2f);
        animator.SetFloat("Z Direction", inputY * 2f);
    }
    
    private void LockOnTarget(){
        // switched animation sets to strafing
        animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * NormToCombatSpeed));
        // we switch what the camera is looking at to the target
        FollowCamera.LookAt = Target;
        // creates a seperate vector that holds the targets position, but
        // changes its y value to match the player. This prevents the
        // player from tilting upward when moving in closer to the target
        Vector3 aimTarget = Target.position;
        aimTarget.y = transform.position.y;
        Vector3 focusDirection = (aimTarget - transform.position).normalized;
        // rotates the player to the target at adjustable speeds
        playerBody.forward = Vector3.Lerp(playerBody.forward, focusDirection, Time.deltaTime * RotateToTargetSpeed);
        // posiotions the camera slightly above the player's body, and rotates to face the target at adjustable speeds
        Vector3 focusTarget = (Target.position - GetComponent<PlayerController>().CinemachineCameraTarget.transform.position).normalized;
        focusTarget = new Vector3(focusTarget.x, focusTarget.y - 0.15f, focusTarget.z);
        GetComponent<PlayerController>().CinemachineCameraTarget.transform.forward =
        Vector3.Lerp(GetComponent<PlayerController>().CinemachineCameraTarget.transform.forward, focusTarget, Time.deltaTime * CamRotateToTargetSpeed);
        // makes sure that the camera will be consistent with the locked rotation when delocked
        GetComponent<PlayerController>().CinemachineTargetPitch = GetComponent<PlayerController>().CinemachineCameraTarget.transform.rotation.eulerAngles.x;
        GetComponent<PlayerController>().CinemachineTargetYaw = GetComponent<PlayerController>().CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        // Cancel lock
        if(Input.GetButtonDown("R3 Button") || Input.GetKeyDown(KeyCode.Mouse2)){
            Target = null;
        }
    }
    private void DelockTarget(){
        transform.forward =  playerBody.forward;
        playerBody.forward = transform.forward;
        FollowCamera.LookAt = GetComponent<PlayerController>().CinemachineCameraTarget.transform;
        GetComponent<PlayerController>().RotateOnMoveDirection = true;
        GetComponent<PlayerController>().SprintSpeed = defaultSprintSpeed;
        animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
    }

    private void FindTarget(){
        if(Input.GetButtonDown("R3 Button") || Input.GetKeyDown(KeyCode.Mouse2)){
            Target = FindObjectOfType<LockableTargets>().AssessTarget();
            GetComponent<PlayerController>().RotateOnMoveDirection = false;
            GetComponent<PlayerController>().SprintSpeed = GetComponent<PlayerController>().WalkSpeed;

        }
    }
}
