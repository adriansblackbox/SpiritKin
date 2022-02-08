using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float WalkSpeed = 2.0f;
    [SerializeField] public float SprintSpeed = 5.0f;
    [SerializeField] public float RotationSmoothTime = 1f;
    [SerializeField] public float SpeedChangeRate = 10.0f;
    [SerializeField] public float MouseSensitivity = 300f;
    [SerializeField] public float StickLookSensitivity = 200f;
    [SerializeField] public float PlayerHeight = 2f;
    [HideInInspector] public float TempSpeed = 0f;
    [HideInInspector] public float CinemachineTargetYaw;
	[HideInInspector] public float CinemachineTargetPitch;
    [HideInInspector] public bool RotateOnMoveDirection = true;
    [HideInInspector] public GameObject CinemachineCameraTarget;
    [HideInInspector] public Vector3 targetMoveDirection;

    [HideInInspector] public float speed;    
    private float targetSpeed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float moveTrajectorySpeed = 5f;
    private float gravity = -20f;
    private float input_x;
    private float input_y;
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
        CinemachineCameraTarget.transform.position = new Vector3(
            transform.position.x,
            transform.position.y + PlayerHeight,
            transform.position.z
        );
        if(!combatScript.isAttacking && !combatScript.isDodging){
            InputMovement();
        }else{
            CombatMovement();
        }
        Animation();
        if(GetComponent<LockTarget>().Target == null){
            RotateCamera();
        }
    }
    private void InputMovement(){
        input_x = Input.GetAxis("Horizontal");
        input_y = Input.GetAxis("Vertical");
        inputDirection = new Vector2(input_x, input_y).normalized;
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetButton("A Button")){
            targetSpeed = SprintSpeed;
        }else{                          
            targetSpeed = WalkSpeed;
        }if(inputDirection == Vector2.zero){
            targetSpeed = 0.0f;
        }
        // creates curved result rather than a linear one giving a more organic speed change
        // round speed to 3 decimal places
        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * SpeedChangeRate);
        speed = Mathf.Round(speed * 1000f) / 1000f;
        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (inputDirection != Vector2.zero){
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);
            // rotate to face input direction relative to camera position
            if(RotateOnMoveDirection && !combatScript.isAttacking)
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        targetMoveDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        moveDirection = Vector3.Lerp(moveDirection, targetMoveDirection, Time.deltaTime * moveTrajectorySpeed);
        // adjustemnts movement for sharp turns
        if((moveDirection - targetMoveDirection).magnitude >= 1.5f){
            speed = 0f;
            moveDirection = targetMoveDirection;
        }
        // move direction is normalized, and the caharacter controller applies contstant
        // downward force for easy slope traversal
        moveDirection.Normalize();
        controller.Move(new Vector3(moveDirection.x, gravity, moveDirection.z) * speed * Time.deltaTime);
    }
     private void CombatMovement(){
        speed = 0.0f;
        // if the playe is attacking, their move direction should always be their forward direction
        if(combatScript.isAttacking){
            moveDirection = transform.forward;
        }
        // if the player is dodgin, their movement direction should be their last input target direction
        if(combatScript.isDodging){
            moveDirection = targetMoveDirection;
        }
        // TempSpeed is altered by the PlayerCombat script
        TempSpeed = Mathf.Lerp(TempSpeed, 0.0f, Time.deltaTime * 5f);
        // move direction is normalized, and the caharacter controller applies contstant
        // downward force for easy slope traversal
        moveDirection.Normalize();
        controller.Move(new Vector3(moveDirection.x, gravity, moveDirection.z) * TempSpeed * Time.deltaTime);
    }
    private void Animation(){
        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        animator.SetFloat("Speed", animationBlend);
        float inputMagnitude = inputDirection.magnitude;
        if(inputMagnitude > 0){
            animator.SetFloat("MotionSpeed", inputDirection.magnitude);
        }else{
            animator.SetFloat("MotionSpeed", 1f);
        }
    }

    private void RotateCamera(){
        CinemachineTargetYaw += Input.GetAxis("RightStick X") * Time.deltaTime * StickLookSensitivity;
		CinemachineTargetPitch += -1 * Input.GetAxis("RightStick Y") * Time.deltaTime * StickLookSensitivity;
        CinemachineTargetYaw += Input.GetAxis("Mouse X") * Time.deltaTime * MouseSensitivity;
		CinemachineTargetPitch += -1 * Input.GetAxis("Mouse Y") * Time.deltaTime * MouseSensitivity;
        CinemachineTargetYaw = ClampAngle(CinemachineTargetYaw, float.MinValue, float.MaxValue);
		CinemachineTargetPitch = ClampAngle(CinemachineTargetPitch, -30.0f, 70.0f);
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(CinemachineTargetPitch, CinemachineTargetYaw, 0.0f);
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
