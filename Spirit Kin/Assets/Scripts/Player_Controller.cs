using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class Player_Controller : MonoBehaviour
{
    public float WalkSpeed = 2.0f;
    public float SprintSpeed = 5.0f;
    public float RotationSmoothTime = 1f;
    public float SpeedChangeRate = 10.0f;
    public float MouseSensitivity = 300f;
    public float StickLookSensitivity = 200f;
    public float PlayerHeight = 2f;

    public bool RotateOnMoveDirection = true;

    public GameObject CinemachineCameraTarget;
    public CinemachineVirtualCamera FollowCamera;
    public CinemachineVirtualCamera AimCamera;

    public float _speed;
    private float _targetSpeed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    public float _cinemachineTargetYaw;
	public float _cinemachineTargetPitch;
    private float _cameraNoise;
    public Vector3 targetDirection;
    private Vector3 moveDirection;
    private float _trajectorySpeed = 5f;
    private float _speedChangeRateDEF;
    private float _gravity = -20f;

    private CharacterController _controller;
    private Player_Battle_Controller _battle_controller;
    private Animator _animator;
	private GameObject _mainCamera;
    private bool _isGrounded;
    private RaycastHit hitInfo;

    private void Awake() {
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }
    void Start()
    {
        _battle_controller = GetComponent<Player_Battle_Controller>();
        Cursor.lockState = CursorLockMode.Locked;
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _speedChangeRateDEF = SpeedChangeRate;
    }
    void Update()
    {
        CinemachineCameraTarget.transform.position = new Vector3(
            transform.position.x,
            transform.position.y + PlayerHeight,
            transform.position.z
        );
        // If the speed change rate has been altered due to frantic change in direction,
        // it is reset to the default rate by lerping
        if(SpeedChangeRate != _speedChangeRateDEF){
            SpeedChangeRate = Mathf.Lerp(SpeedChangeRate, _speedChangeRateDEF, Time.deltaTime * 5f);
        }
        Move();
    }
    void LateUpdate(){
        if(GetComponent<Lock_Target>().Target == null)
            RotateCamera();
    }
    public void Move(){
        float input_x = Input.GetAxis("Horizontal");
        float input_y = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(input_x, 0.0f, input_y).normalized;
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetButton("A Button")){
            _targetSpeed = SprintSpeed * new Vector2(input_x, input_y).magnitude;
        }else{                           
            _targetSpeed = WalkSpeed * new Vector2(input_x, input_y).magnitude;
        }if(inputDirection == Vector3.zero){
            _targetSpeed = 0.0f;
        }
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        if (currentHorizontalSpeed < _targetSpeed - speedOffset || currentHorizontalSpeed > _targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, _targetSpeed, Time.deltaTime * SpeedChangeRate);
            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = _targetSpeed;
        }
        _animationBlend = Mathf.Lerp(_animationBlend, _targetSpeed, Time.deltaTime * SpeedChangeRate);
        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (inputDirection != Vector3.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            // rotate to face input direction relative to camera position
            if(RotateOnMoveDirection && !_battle_controller.isAttacking)
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        // move the player
        // If the player is performing any kind of action, the direction and speed is modified
        // before teh controller moves the characrter
        moveDirection = Vector3.Lerp(moveDirection, targetDirection, Time.deltaTime * _trajectorySpeed);
        OverrideDirection();
        moveDirection.Normalize();
        _controller.Move(new Vector3(moveDirection.x * _speed, _gravity, moveDirection.z * _speed) * Time.deltaTime);
        _animator.SetFloat("Speed", _animationBlend);
        float inputMagnitude = inputDirection.magnitude;
        if(inputMagnitude > 0)
        _animator.SetFloat("MotionSpeed", inputDirection.magnitude);
        else
        _animator.SetFloat("MotionSpeed", 1f);
    }
    private void OverrideDirection(){
         if((moveDirection - targetDirection).magnitude >= 1.5f){
            _speed = 0;
            moveDirection = targetDirection;
        }
        if(_battle_controller.DodgeTime > _battle_controller.TotalDodgeTime/2){
            _speed = Mathf.Lerp(_speed, 50f, Time.deltaTime * 20f);
            moveDirection = targetDirection = _battle_controller.DodgeDirection;
        }else if(_battle_controller.DodgeTime > 0){
            _speed = Mathf.Lerp(_speed, 0.1f, Time.deltaTime * 20f);
            moveDirection = targetDirection = _battle_controller.DodgeDirection;
            SpeedChangeRate = 0f;
        }
        if(_battle_controller.isAttacking){
            moveDirection = _battle_controller.LungeDirection;
            _speed = 50f - _battle_controller.ComboTimeDelay * 100f;
            _speed = Mathf.Clamp(_speed, 0f, 10f);
        }
        if((moveDirection - targetDirection).magnitude >= 1.5f && 
            _battle_controller.DodgeTime <= 0 &&
            !_battle_controller.isAttacking){
            _speed = 0;
            moveDirection = targetDirection;
        }
    }

    private void RotateCamera(){
        _cinemachineTargetYaw += Input.GetAxis("RightStick X") * Time.deltaTime * StickLookSensitivity;
		_cinemachineTargetPitch += -1 * Input.GetAxis("RightStick Y") * Time.deltaTime * StickLookSensitivity;
        _cinemachineTargetYaw += Input.GetAxis("Mouse X") * Time.deltaTime * MouseSensitivity;
		_cinemachineTargetPitch += -1 * Input.GetAxis("Mouse Y") * Time.deltaTime * MouseSensitivity;

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
		_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, -30.0f, 70.0f);

        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
