using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Lock_Target : MonoBehaviour
{
    private float _defaultSensitivity;
    private float _defaultSprintSpeed;
    public Transform Target = null;
    private Animator _animator;

    public CinemachineVirtualCamera FollowCamera;

    
    private void Start() {
        _defaultSensitivity = GetComponent<Player_Controller>().MouseSensitivity;
        _defaultSprintSpeed =  GetComponent<Player_Controller>().SprintSpeed;
        _animator = GetComponent<Animator>();
    }

    private void Update(){
        if(Target != null){
            LockOnTarget();
        }else{
            DelockTarget();
            FindTarget();
        }
    }
    
    private void LockOnTarget(){
        FollowCamera.LookAt = Target;
        Vector3 _worldAimTarget = new Vector3(Target.position.x, Target.position.y, Target.position.z);
        _worldAimTarget.y = transform.position.y;
        Vector3 _focusDirection = (_worldAimTarget - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, _focusDirection, Time.deltaTime * 20f);
        _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
        
        Vector3 focusTarget = (Target.position - GetComponent<Player_Controller>().CinemachineCameraTarget.transform.position).normalized;
        focusTarget = new Vector3(focusTarget.x, focusTarget.y - 0.15f, focusTarget.z);
        GetComponent<Player_Controller>().CinemachineCameraTarget.transform.forward =
        Vector3.Lerp(GetComponent<Player_Controller>().CinemachineCameraTarget.transform.forward, focusTarget, Time.deltaTime * 20f);
        
        // Cancel lock
        if(Input.GetButtonDown("R3 Button") || Input.GetKeyDown(KeyCode.Mouse2)){
            Target = null;
            GetComponent<Player_Controller>()._cinemachineTargetPitch = GetComponent<Player_Controller>().CinemachineCameraTarget.transform.rotation.eulerAngles.x;
            GetComponent<Player_Controller>()._cinemachineTargetYaw = GetComponent<Player_Controller>().CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        }
    }
    private void DelockTarget(){
        FollowCamera.LookAt = GetComponent<Player_Controller>().CinemachineCameraTarget.transform;
        GetComponent<Player_Controller>().RotateOnMoveDirection = true;
        GetComponent<Player_Controller>().SprintSpeed = _defaultSprintSpeed;
        _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
    }

    private void FindTarget(){
        if(Input.GetButtonDown("R3 Button") || Input.GetKeyDown(KeyCode.Mouse2)){
            Target = FindObjectOfType<Lockable_Targets>().AssessTarget();
            GetComponent<Player_Controller>().RotateOnMoveDirection = false;
            GetComponent<Player_Controller>().SprintSpeed = GetComponent<Player_Controller>().WalkSpeed;

        }
    }
}
