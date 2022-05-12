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
    [SerializeField] public float DashSpeed = 20f;
    [SerializeField] public float DashTime = 1f;
    [SerializeField] public float AttackDelay = 0.5f;
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
    public GameObject[] DashInvisibleObjects;
    private float input_x, input_y;
    private int input_invert = 1;
    private float targetRotation = 0.0f;
    private float rotationVelocity = 10f;
    public float Gravity = -30f;
    public float GravityPullRange = 5f;
    private float animationBlend;
    private Vector3 moveDirection;
    private CharacterController controller;
    private SwordCollision swordScript;
    public Animator animator;
	private GameObject mainCamera;
    
    public Transform A1RayCast, A2RayCast, A3RayCast, A4RayCast, A5RayCast;
    public ParticleSystem[] AttackVFX;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        State = "Idle";
        animator.SetFloat("Attack Number", 1);
        CinemachineTargetYaw = 180;
        CinemachineTargetPitch = 0;
        swordScript = GetComponent<SwordCollision>();
    }
    void Update()
    {
        if(!animator.GetBool("Death")) {
            PlayerInput(); 
            RotateCamera();
        }
        // If the player is in the move tree state machine, allow ilde movement.
        // else, base movement off of attack when attack allows movement
        if(Animator.StringToHash("Base.Move Tree") == animator.GetCurrentAnimatorStateInfo(0).fullPathHash && animator.GetLayerWeight(1) != 1)
            Movement();
        else if(animator.GetBool("Attack Movement"))
            AttackMovement();
        else if(animator.GetBool("Dash Movement"))
            DashMovement();
    }
    //===========================================================
    // Input getter
    //===========================================================
    private void PlayerInput(){
        input_x = Input.GetAxis("Horizontal") * input_invert;
        input_y = Input.GetAxis("Vertical") * input_invert;
        animator.SetFloat("X Direction Input", Mathf.Abs(input_x));
        animator.SetFloat("Z Direction Input", Mathf.Abs(input_y));
        if(Input.GetButtonDown("X Button") || Input.GetKeyDown(KeyCode.Mouse0))
            animator.SetBool("X Pressed", true);
        if(Input.GetButtonDown("Y Button") || Input.GetKeyDown(KeyCode.Mouse1))
           animator.SetBool("Y Pressed", true);
        if(Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.Space) && !animator.GetBool("Dash Movement"))
            animator.SetBool("A Pressed", true);
    }
    //===========================================================
    // Animation events and triggers
    //===========================================================
    public void AnimationStart() {
        // Set Animator Param
        animator.SetBool("Dash End", false);
        animator.SetBool("Dash Movement", false);
        animator.SetBool("A Pressed", false);
        animator.SetBool("X Pressed", false);
        animator.SetBool("Y Pressed", false);
        animator.SetBool("Attack Movement", false);
        animator.SetBool("Attack Cancel", false);
        animator.SetBool("Move Cancel", false);
        animator.SetBool("Combo Reset", false);
        animator.SetBool("Attack End", false);
        DisableHitRay();
    }
    private void AttackStart(){
       AnimationStart();
    }
    private void DashStart(){
       AnimationStart();
    }
    private void SetAttackCancelTrue(){
        animator.SetBool("Attack Cancel", true);
    }
    private void SetMoveCancelTrue(){
        animator.SetBool("Move Cancel", true);
        animator.SetFloat("Attack Delay", AttackDelay);
    }
    private void SetComboResetTrue(){
        animator.SetBool("Combo Reset", true);   
    }
    public void EnableHitRay(){
        string attackName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        switch(attackName){
            case "Attack1_K":
                // sets attack num to the next attack for delay 
                animator.SetInteger("Attack Number", 2);
                swordScript.AttackOriginPoints = A1RayCast;
                AttackVFX[0].Play();
                //sound play
            break;
            case "Attack2_K":
                animator.SetInteger("Attack Number", 3);
                swordScript.AttackOriginPoints = A1RayCast;
                AttackVFX[1].Play();
                //sound play
            break;
            case "Attack3_K":
                animator.SetInteger("Attack Number", 4);
                swordScript.AttackOriginPoints = A1RayCast;
                AttackVFX[2].Play();
                //sound play
            break;
            case "Attack4_K":
                animator.SetInteger("Attack Number", 5);
                swordScript.AttackOriginPoints = A1RayCast;
                AttackVFX[3].Play();
                //sound play
            break;
            case "Attack5_K":
                animator.SetInteger("Attack Number", 1);
                swordScript.AttackOriginPoints = A1RayCast;
                AttackVFX[4].Play();
                //sound play
                speed = 80f;
            break;
             case "Dash Attack":
                animator.SetFloat("Attack Number", 1);
            break;
        }
        swordScript.immuneEnemies.Clear();
        swordScript.RaycastOn = true;

    }
    public void DisableHitRay(){
        swordScript.RaycastOn = false;
    }
    private void AttackEnd(){
        speed = 0.0f;
        animator.SetBool("Attack End", true);
    }
    private void StartAttackMovement(){
        animator.SetBool("Attack Movement", true);
    }
    private void SetDashAttackTrue(){
        animator.SetBool("Dash Attack", true);
    }
    private void StartDashMovement(){
        animator.SetBool("Dash Movement", true);
        // resets attack delay in the case that the player dashes before the delay window closes
        animator.SetFloat("Attack Delay", 0);
        animator.SetInteger("Attack Number", 1);
        inputDirection = new Vector2(input_x, input_y);
        if (inputDirection != Vector2.zero){
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            transform.GetChild(0).transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);
        }
    }
    private void DashEnd(){
        animator.SetBool("Dash End", true);
        animator.SetBool("Dash Movement", false);
        animator.SetFloat("Dash Cooldown", 0.2f);
        speed = 0.0f;
    }
    //====================================================
    // Attack/Dash movment logic, effects, sounds, and execution
    //====================================================
    private void DashMovement(){
        moveDirection = transform.GetChild(0).transform.forward.normalized;
        speed = DashSpeed;
        controller.Move(moveDirection * speed * Time.deltaTime);
    }

    private void DashInvisiblityOn(){
        for(int i = 0; i < DashInvisibleObjects.Length; i++){
            DashInvisibleObjects[i].SetActive(false);
        }
        StartCoroutine(DashInvisibleTime());
        StartDashMovement();
    }
    IEnumerator DashInvisibleTime () {
        animator.speed = 0;
        yield return new WaitForSeconds(DashTime);
        animator.speed = 1;
        animator.SetBool("Dash Movement", false);
        yield return null;
    }
    private void DashInvisiblityOff(){
        for(int i = 0; i < DashInvisibleObjects.Length; i++){
            DashInvisibleObjects[i].SetActive(true);
        }
    }
    
    private void AttackMovement(){
        //Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        string attackName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        switch(attackName){
            case "Attack1_K":
                AttackOneMovement();
            break;
            case "Attack2_K":
                AttackOneMovement();
            break;
            case "Attack3_K":
                AttackOneMovement();
            break;
            case "Attack4_K":
                AttackOneMovement();
            break;
            case "Attack5_K":
                AttackFiveMovement();
            break;
        }
    }
    private void AttackOneMovement(){
        inputDirection = new Vector2(input_x, input_y);
        if(inputDirection != Vector2.zero)
            speed = Mathf.Clamp(speed, 2f, float.MaxValue);
        else
            speed = 0f;
        speed = Mathf.Lerp(speed, WalkSpeed/1.5f, Time.deltaTime * 20f);
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
        speed =  Mathf.Lerp(speed, 0f, Time.deltaTime * 3f);
        moveDirection = transform.GetChild(0).gameObject.transform.forward;
        moveDirection.y = Gravity;
        moveDirection = moveDirection.normalized * (speed * GetComponent<PlayerStats>().speed.GetValue());
        controller.Move(moveDirection * Time.deltaTime);
    }
    //===========================================================
    // Idle movement, and camera rotation
    //===========================================================
    private void Movement(){
        animatorParamReset();
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
        if(inputDirection != Vector2.zero) {
            speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * SpeedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }else {
            speed = 0;
        }
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
        animationBlend = Mathf.Lerp(animationBlend, Mathf.Abs(speed) * GetComponent<PlayerStats>().speed.GetValue(), Time.deltaTime * 100f);
        animator.SetFloat("Speed", animationBlend);
        // adjusting the motion speed variable with the input magnitude allows
        // the player to slowly creep up to a full speed on their controller
        if(inputDirection.magnitude > 0){
            animator.SetFloat("MotionSpeed", inputDirection.magnitude);
        }else{
            animator.SetFloat("MotionSpeed", 1f);
        }
    }
    private void animatorParamReset() {
        // Prevents any movement from attacks from happening
        if(animator.GetBool("Attack Movement")) {
            animator.SetBool("Attack Movement", false);
        }
        // Delays attack sequence
        if(animator.GetFloat("Attack Delay") > 0) {
            animator.SetFloat("Attack Delay", animator.GetFloat("Attack Delay") - Time.deltaTime);
        } else if (animator.GetInteger("Attack Number") != 1) {
            animator.SetInteger("Attack Number", 1);
        }
        if (animator.GetFloat("Dash Cooldown") >= 0) {
            animator.SetFloat("Dash Cooldown", animator.GetFloat("Dash Cooldown") - Time.deltaTime);
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
            CinemachineTargetPitch = ClampAngle(CinemachineTargetPitch, -40.0f, 70.0f);
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
    public void Stun() {
        AnimationStart();
        animator.SetBool("Stunned", true);
        animator.SetLayerWeight(1, 1);
        animator.SetLayerWeight(0, 0);
        AttackEnd();
    }
    private void StunEnd() {
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(0, 1);
        animator.SetBool("Stunned", false);
    }
    private void SpecialStart() {
        AnimationStart();
        animator.SetBool("Special End", false);
    }
    private void PullEnemies() {
        GameObject[] enemies =  GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log(enemies.Length);
        for(int i = 0; i < enemies.Length; i++) {
            //Vector3.Distance(other.position, transform.position);
            if(Vector3.Distance(transform.position, enemies[i].transform.position) <= GravityPullRange) {
                if(enemies[i].GetComponent<CharacterStats>())
                    enemies[i].GetComponent<CharacterStats>().TakeDamage(0, -10f);
            }
        }
    }
     private void SpecialEnd() {
        animator.SetBool("Special End", true);
    }
    public void InvertControls(bool invert) {
        if(invert) 
            input_invert = -1;
        else
            input_invert = 1;
    }
    public bool getInverted () {
        return input_invert < 0;
    }
}
