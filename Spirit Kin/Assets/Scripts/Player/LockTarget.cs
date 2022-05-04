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
    public LayerMask EnemyLayer;
    private void Start() {
        controller = GetComponent<PlayerController>();
        playerBody = transform.GetChild(0).gameObject.transform;
    }

    private void Update(){
        FindTarget();
        if(Target != null){
            LockOnTarget();
        }
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
        controller.CinemachineCameraTarget.transform.eulerAngles = new Vector3(
            ClampAngle(controller.CinemachineCameraTarget.transform.eulerAngles.x, -40f, 20f),
            controller.CinemachineCameraTarget.transform.eulerAngles.y,
            controller.CinemachineCameraTarget.transform.eulerAngles.z
            );
        // makes sure that the camera will be consistent with the locked rotation when delocked
        controller.CinemachineTargetPitch =controller.CinemachineCameraTarget.transform.rotation.eulerAngles.x;
        controller.CinemachineTargetYaw =controller.CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        // Cancel lock
        if((this.transform.position - Target.transform.position).magnitude > LetGoDistance){
            Target.GetComponent<Enemy_Controller>().LockOnArrow.SetActive(false);
            Target = null;
        }
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void FindTarget(){
        if(Input.GetButtonDown("Right Stick Button") || Input.GetKeyDown(KeyCode.Mouse2)){
            if(Target != null) {
                Target.GetComponent<Enemy_Controller>().LockOnArrow.SetActive(false);
                Target = null;
                
            }else {
                float rayLength = 200f;
                RaycastHit hit;
                if(Physics.SphereCast(this.transform.position, 20f, FindObjectOfType<PlayerController>().CinemachineCameraTarget.transform.forward, out hit, rayLength, EnemyLayer)) {
                    Target = hit.transform;
                    Target.GetComponent<Enemy_Controller>().LockOnArrow.SetActive(true);
                } else {
                    Target = null;
                }
            }
        }
    }
}
