using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
    This script controls the player's movement, and utilizes 
    Unity's built in Character Controller. Certain variables
    taken from the player's combat script (PlayerCombat) are taken
    into consideration when applying movement to the controller.
*/
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float WalkSpeed = 2.0f;
    [SerializeField] public float SprintSpeed = 5.0f;
    [SerializeField] public float MinimumSpeed = 5f;
    [SerializeField] private float RotationSmoothTime = 1f;
    [SerializeField] private float SpeedChangeRate = 10.0f;
    [SerializeField] private float MouseSensitivity = 200f;
    [SerializeField] private float StickLookSensitivity = 200f;
    [SerializeField] private float MoveToTargetSpeed = 10f;
    [SerializeField] private float CombatSpeedDropoff = 5f;
    [HideInInspector] public float TempSpeed = 0f;
    [HideInInspector] public float CinemachineTargetYaw;
	[HideInInspector] public float CinemachineTargetPitch;
    [HideInInspector] public bool RotateOnMoveDirection = true;
    [HideInInspector] public GameObject CinemachineCameraTarget;

    [HideInInspector] public float speed;    
    private float targetSpeed;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float gravity = -15f;
    private float input_x;
    private float input_y;
    private float animationBlend;
    private Vector3 targetMoveDirection;
    private Vector3 moveDirection;
    private Vector2 inputDirection;
    private CharacterController controller;
    private PlayerCombat combatScript;
    private Animator animator;
	private GameObject mainCamera;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        combatScript = GetComponent<PlayerCombat>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        Animation();
        // if the player is initiating a mechanic besides basic movement,
        // their input is ignored until movement dependant mechanic is done
        if(!combatScript.isAttacking && !combatScript.isDodging){
            InputMovement();
        }else{
            CombatMovement();
        }
        // so long as the player is not locked onto a target, they can rotate their
        // camera freely
        if(GetComponent<LockTarget>().Target == null){
            RotateCamera();
        }
    }
    private void InputMovement(){
        input_x = Input.GetAxis("Horizontal");
        input_y = Input.GetAxis("Vertical");
        inputDirection = new Vector2(input_x, input_y);
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetButton("A Button")){
            targetSpeed = SprintSpeed * inputDirection.magnitude;
        }else{                          
            targetSpeed = WalkSpeed * inputDirection.magnitude;
        }if(inputDirection == Vector2.zero){
            targetSpeed = 0.0f;
        }
        // creates curved result rather than a linear one giving a more organic speed change
        // round speed to 3 decimal places
        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * SpeedChangeRate);
        speed = Mathf.Round(speed * 1000f) / 1000f;
        if(targetSpeed != 0.0f){
            speed = Mathf.Clamp(speed, MinimumSpeed, float.MaxValue);
        }
        speed = Mathf.Clamp(speed, 0.0f, SprintSpeed);
        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        inputDirection.Normalize();
        if (inputDirection != Vector2.zero){
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);
            // rotate to face input direction relative to camera position
            if(RotateOnMoveDirection)
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        targetMoveDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        moveDirection = transform.GetChild(0).gameObject.transform.forward;
        // if the player is locked onto a target, they should move according to their input
        // instead of the player's forward vector
        if(GetComponent<LockTarget>().Target != null){
            moveDirection = targetMoveDirection;
        }
        moveDirection = moveDirection.normalized * speed;
        controller.Move(new Vector3(moveDirection.x, gravity, moveDirection.z) * Time.deltaTime);
    }
     private void CombatMovement(){
        speed = 0.0f;
        // if the player is attacking, their move direction should always be their forward direction
        if(combatScript.isAttacking){
            // note: the player's body is a child of the player game object for camera locking
            // reasons. When the player attacks, they should attack along the forward vector of
            // the player's body
            moveDirection = transform.GetChild(0).gameObject.transform.forward;
        }
        if(combatScript.isDodging){
            moveDirection = targetMoveDirection;
        }
        // TempSpeed is altered by the PlayerCombat script
        TempSpeed = Mathf.Lerp(TempSpeed, 0.0f, Time.deltaTime * CombatSpeedDropoff);
        // move direction is normalized, and the caharacter controller applies contstant
        // downward force for easy slope traversal
        moveDirection.Normalize();
        controller.Move(new Vector3(moveDirection.x, gravity, moveDirection.z) * TempSpeed * Time.deltaTime);
    }
    private void Animation(){
        animationBlend = Mathf.Lerp(animationBlend, speed, Time.deltaTime * 10f);
        animator.SetFloat("Speed", animationBlend);
        // adjusting the motion speed variable with the input magnitude allows
        // the player to slowly creep up to a full speed on their controller
        if(inputDirection.magnitude > 0){
            animator.SetFloat("MotionSpeed", inputDirection.magnitude);
        }else{
            animator.SetFloat("MotionSpeed", 1f);
        }
    }

    private void RotateCamera(){
        // both controller and keyboard/mouse inputs are taken into account. The one
        // that the player uses should be the only one that will effect camera rotation
        CinemachineTargetYaw += Input.GetAxis("RightStick X") * Time.deltaTime * StickLookSensitivity;
		CinemachineTargetPitch += -1 * Input.GetAxis("RightStick Y") * Time.deltaTime * StickLookSensitivity;
        CinemachineTargetYaw += Input.GetAxis("Mouse X") * Time.deltaTime * MouseSensitivity;
		CinemachineTargetPitch += -1 * Input.GetAxis("Mouse Y") * Time.deltaTime * MouseSensitivity;
        // with a helper fucntion, the player's camera rotation is clmaped with the given angles
        CinemachineTargetYaw = ClampAngle(CinemachineTargetYaw, float.MinValue, float.MaxValue);
		CinemachineTargetPitch = ClampAngle(CinemachineTargetPitch, -30.0f, 70.0f);
        // rotates the camera
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(CinemachineTargetPitch, CinemachineTargetYaw, 0.0f);
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
