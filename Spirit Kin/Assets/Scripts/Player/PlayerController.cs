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
    [SerializeField] private float RotationSmoothTime = 1f;
    [SerializeField] private float CombatRotationSmoothTime = 1f;
    [SerializeField] private float SpeedChangeRate = 10.0f;
    [SerializeField] private float MouseSensitivity = 200f;
    [SerializeField] private float StickLookSensitivity = 200f;
    [SerializeField] private float MoveToTargetSpeed = 10f;
    [SerializeField] private float AnimationBlendTime = 20f;
    [HideInInspector] public float TempSpeed = 0f;
    [HideInInspector] public float CinemachineTargetYaw;
	[HideInInspector] public float CinemachineTargetPitch;
    [HideInInspector] public bool RotateOnMoveDirection = true;
    [HideInInspector] public GameObject CinemachineCameraTarget;
    [HideInInspector] public Vector2 inputDirection;
    [HideInInspector] public Vector3 targetMoveDirection;
    [HideInInspector] public float targetSpeed;
    [HideInInspector] public float speed;
    private float targetRotation = 0.0f;
    private float rotationVelocity = 10f;
    public float Gravity = -30f;
    private float input_x;
    private float input_y;
    private float animationBlend;
    private Vector3 moveDirection;
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
        // if the player is initiating a mechanic besides basic movement,
        // their input is ignored until movement dependant mechanic is done
        InputMovement();
        Animation();
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
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetAxisRaw("Right Trigger") > 0.1){
            targetSpeed = SprintSpeed * inputDirection.magnitude;
            targetSpeed = Mathf.Clamp(targetSpeed, 0.0f, SprintSpeed);
        }else{                          
            targetSpeed = WalkSpeed * inputDirection.magnitude;
            targetSpeed = Mathf.Clamp(targetSpeed, 0.0f, WalkSpeed);
        }if(inputDirection == Vector2.zero){
            targetSpeed = 0.0f;
        }
        if(targetSpeed != 0.0f){
            speed = Mathf.Clamp(speed, 2f, float.MaxValue);
        }
        // creates curved result rather than a linear one giving a more organic speed change
        // round speed to 3 decimal places
        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * SpeedChangeRate);
        speed = Mathf.Round(speed * 1000f) / 1000f;
        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        inputDirection.Normalize();
        if (inputDirection != Vector2.zero){
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotationSpeed = RotationSmoothTime;
            if(combatScript.isAttacking)
                rotationSpeed = CombatRotationSmoothTime;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSpeed);
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
        float moveSpeed = speed;
        if(combatScript.isDodging)
            moveSpeed = TempSpeed;
        else if(combatScript.isAttacking && !combatScript.isDodging){
            TempSpeed = Mathf.Lerp(TempSpeed, 0.0f, combatScript.CombatRunSpeedDropoff * Time.deltaTime);
            moveSpeed = TempSpeed * inputDirection.magnitude;
            moveDirection = combatScript.attackDirection;
        }
        moveDirection.y = Gravity;
        moveDirection = moveDirection.normalized * (moveSpeed * GetComponent<PlayerStats>().speed.GetValue());
        controller.Move(moveDirection * Time.deltaTime);
    }
    private void Animation(){
        animationBlend = Mathf.Lerp(animationBlend, speed * GetComponent<PlayerStats>().speed.GetValue(), Time.deltaTime * 100f);
        //if(inputDirection == Vector2.zero)
            //animationBlend = 0;
        animator.SetFloat("Speed", animationBlend);
        if(combatScript.isAttacking)
            speed = 0;
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
