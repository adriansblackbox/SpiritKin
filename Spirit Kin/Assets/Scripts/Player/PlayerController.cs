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
    [SerializeField] private float SpeedChangeRate = 10.0f;
    [SerializeField] private float MouseSensitivity = 200f;
    [SerializeField] private float StickLookSensitivity = 200f;
    [HideInInspector] public float TempSpeed = 0f;
    [HideInInspector] public float CinemachineTargetYaw;
	[HideInInspector] public float CinemachineTargetPitch;
    [HideInInspector] public bool RotateOnMoveDirection = true;
    [HideInInspector] public GameObject CinemachineCameraTarget;
    [HideInInspector] public Vector2 inputDirection;
    [HideInInspector] public Vector3 targetMoveDirection;
    [HideInInspector] public float targetSpeed;
    [HideInInspector] public float speed;
    [HideInInspector] public string State;
    private float input_x, input_y;
    private float targetRotation = 0.0f;
    private float rotationVelocity = 10f;
    public float Gravity = -30f;
    private float animationBlend;
    private Vector3 moveDirection;
    private CharacterController controller;
    private Animator animator;
	private GameObject mainCamera;
    

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        State = "Idle";
    }
    void Update()
    {
        PlayerInput();
        RotateCamera();
        // If the player is in the move tree state machine, allow ilde movement.
        // else, base movement off of attack when attack allows movement
        if(Animator.StringToHash("Base.Move Tree") == animator.GetCurrentAnimatorStateInfo(0).fullPathHash)
            Movement();
        else if(animator.GetBool("Attack Movement"))
            AttackMovement();
    }
    //===========================================================
    // Input getter
    //===========================================================
    private void PlayerInput(){
        input_x = Input.GetAxis("Horizontal");
        input_y = Input.GetAxis("Vertical");
        animator.SetFloat("X Direction Input", Mathf.Abs(input_x));
        animator.SetFloat("Z Direction Input", Mathf.Abs(input_y));
        if(Input.GetButtonDown("X Button") || Input.GetKeyDown(KeyCode.Mouse0))
            animator.SetBool("X Pressed", true);
        //if(Input.GetButtonDown("Y Button"))
        //   animator.SetBool("Y Pressed", true);
        if(Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.Space))
            animator.SetBool("A Pressed", true);
    }
    //===========================================================
    // Animation events and triggers
    //===========================================================
    private void AttackStart(){
        // Set Animator Param
        animator.SetBool("X Pressed", false);
        //Y_Pressed = false;
        animator.SetBool("A Pressed", false);
        animator.SetBool("Attack Movement", false);
        animator.SetBool("Attack Cancel", false);
        animator.SetBool("Move Cancel", false);
        animator.SetBool("Combo Reset", false);
        animator.SetBool("Attack End", false);
        // Movement Prep
        //if(Mathf.Abs(input_x) > 0 || Mathf.Abs(input_y) > 0){
        //    targetRotation = Mathf.Atan2(input_x, input_y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
        //    targetMoveDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        //    transform.GetChild(0).gameObject.transform.forward = targetMoveDirection;
        //}
    }
    private void SetAttackCancelTrue(){
        animator.SetBool("Attack Cancel", true);
    }
    private void SetMoveCancelTrue(){
        animator.SetBool("Move Cancel", true);
    }
    private void SetComboResetTrue(){
        animator.SetBool("Combo Reset", true);   
    }
    public void EnableHitRay(){
        GetComponent<CurseMeter>().ActiveSword.GetComponent<SwordCollision>().EnableHitRay();
    }
    public void DisableHitRay(){
        GetComponent<CurseMeter>().ActiveSword.GetComponent<SwordCollision>().DisableHitRay();
    }
    private void AttackEnd(){
        speed = 0.0f;
        animator.SetBool("Attack End", true);
    }
    private void SetDashAttackTrue(){
        
    }
    private void StartAttackMovement(){
        animator.SetBool("Attack Movement", true);
    }
    //===========================================================
    // Attack movment logic, and execution
    //===========================================================
    private void AttackMovement(){
        //Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        string attackName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        switch(attackName){
            case "Attack 1":
                AttackOneMovement();
            break;
            case "Attack 2":
            break;
            case "Attack 3":
            break;
            case "Attack 4":
            break;
            case "Attack 5":
            break;
        }
    }
    private void AttackOneMovement(){
        inputDirection = new Vector2(input_x, input_y);
        if(inputDirection != Vector2.zero)
            speed = Mathf.Clamp(speed, 2f, float.MaxValue);
        else
            speed = 0f;
        speed = Mathf.Lerp(speed, WalkSpeed/2f, Time.deltaTime * 20f);
        inputDirection.Normalize();
        if (inputDirection != Vector2.zero){
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotationSpeed = RotationSmoothTime * 100f;
            float rotation = Mathf.SmoothDampAngle(transform.GetChild(0).transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSpeed);
            // rotate to face input direction relative to camera position
            transform.GetChild(0).transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        moveDirection = transform.GetChild(0).gameObject.transform.forward;
        moveDirection.y = Gravity;
        moveDirection = moveDirection.normalized * (speed * GetComponent<PlayerStats>().speed.GetValue());
        controller.Move(moveDirection * Time.deltaTime);
    }
    private void AttackTwoMovement(){
        
    }
    private void AttackThreeMovement(){
        
    }
    private void AttackFourMovement(){
        
    }
    private void AttackFiveMovement(){
        
    }
    //===========================================================
    // Idle movement, and camera rotation
    //===========================================================
    private void Movement(){
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
            float rotation = Mathf.SmoothDampAngle(transform.GetChild(0).transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSpeed);
            // rotate to face input direction relative to camera position
            if(RotateOnMoveDirection)
                transform.GetChild(0).transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        targetMoveDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        moveDirection = transform.GetChild(0).gameObject.transform.forward;
        // if the player is locked onto a target, they should move according to their input
        // instead of the player's forward vector
        if(GetComponent<LockTarget>().Target != null){
            moveDirection = targetMoveDirection;
        }
        moveDirection.y = Gravity;
        moveDirection = moveDirection.normalized * (speed * GetComponent<PlayerStats>().speed.GetValue());
        controller.Move(moveDirection * Time.deltaTime);
        //Animation
        animationBlend = Mathf.Lerp(animationBlend, speed * GetComponent<PlayerStats>().speed.GetValue(), Time.deltaTime * 100f);
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
        if(GetComponent<LockTarget>().Target == null){
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
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    //===========================================================
}
