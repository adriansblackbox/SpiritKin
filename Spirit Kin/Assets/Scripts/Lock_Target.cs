using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Lock_Target : MonoBehaviour
{
    private float _defaultSensitivity;
    private float _defaultSprintSpeed;
    private Animator _animator;
    public Transform Target = null;
    public CinemachineVirtualCamera FollowCamera;
    private float inputX;
    private float inputY;
    
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
        inputX = Mathf.Lerp(inputX, Input.GetAxis("Horizontal"), Time.deltaTime*10f);
        inputY = Mathf.Lerp(inputY, Input.GetAxis("Vertical"), Time.deltaTime*10f);
        _animator.SetFloat("X Direction", inputX * 2f);
        _animator.SetFloat("Z Direction", inputY * 2f);
    }
    
    private void LockOnTarget(){
        _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
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

        GetComponent<Player_Controller>()._cinemachineTargetPitch = GetComponent<Player_Controller>().CinemachineCameraTarget.transform.rotation.eulerAngles.x;
        GetComponent<Player_Controller>()._cinemachineTargetYaw = GetComponent<Player_Controller>().CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        
        // Cancel lock
        if(Input.GetButtonDown("R3 Button") || Input.GetKeyDown(KeyCode.Mouse2)){
            Target = null;
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
