using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{

	public float walkSpeed = 0.15f;
	public float runSpeed = 1.0f;
	public float sprintSpeed = 2.0f;

    public float maxVelocity = 10;
    
	public float turnSmoothing = 3.0f;
    [HideInInspector]
    public float aimTurnSmoothing = 15.0f;
    [HideInInspector]
    public float speedDampTime = 1f;

	public float rollLength = 5.0f;
	public float rollCooldown = 1.0f;

    public float attackCooldown = 0f;

    public float timeToNextRoll = 0;
	
	private float speed;

	private Vector3 lastDirection;

    [SerializeField]
	private Animator anim;
    [SerializeField]
    private Rigidbody _rb;
	private int speedFloat;
	private int hFloat;
	private int vFloat;
	private int aimBool;
	private int flyBool;
	private int groundedBool;
	private Transform cameraTransform;

	private float h;
	private float v;

	private bool aim;

	private bool run;
	private bool sprint;

	private bool isMoving;

	private float distToGround;
//	private float sprintFactor;

	void Awake()
	{
        if (anim == null)
		    anim = GetComponent<Animator> ();
		cameraTransform = Camera.main.transform;

		speedFloat = Animator.StringToHash("Speed");
		hFloat = Animator.StringToHash("H");
		vFloat = Animator.StringToHash("V");
		aimBool = Animator.StringToHash("Aim");
		groundedBool = Animator.StringToHash("Grounded");
		distToGround = GetComponent<Collider>().bounds.extents.y;
		//sprintFactor = sprintSpeed / runSpeed;
	}

	bool IsGrounded() {
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}

	void Update()
	{
		aim = Input.GetButton("Aim");
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");
		run = Input.GetButton ("Run");
		sprint = Input.GetButton ("Sprint");
		isMoving = Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1;

        AttackManagment();
    }

	void FixedUpdate()
	{
		anim.SetBool (aimBool, IsAiming());
		anim.SetFloat(hFloat, h);
		anim.SetFloat(vFloat, v);
		
		anim.SetBool (groundedBool, IsGrounded ());
		MovementManagement (h, v, run, sprint);
		JumpManagement ();
        LimitVelocity();
	}

    void LimitVelocity()
    {
        if (!sprint && _rb.velocity.magnitude > maxVelocity && timeToNextRoll <= 0)
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxVelocity);
    }

    void AttackManagment()
    {
        if (attackCooldown > 0)
            attackCooldown -= 1 * Time.deltaTime;

        /*
        if (attackCooldown <= 0 && Input.GetButtonDown("Attack1"))
            anim.SetBool("Attack1", true); 
        */

        if (Input.GetButtonDown("Attack1"))
            anim.SetBool("Attack1", true);
    }

	void JumpManagement()
	{
		if(timeToNextRoll > 0)
            timeToNextRoll -= Time.deltaTime;

		if (Input.GetButtonDown ("Roll"))
        {
            anim.SetBool("Roll", true);
            if (timeToNextRoll <= 0 && !aim)
			{
                _rb.AddRelativeForce(Vector3.forward * rollLength, ForceMode.Impulse);
                timeToNextRoll = rollCooldown;
			}
		}
	}

	void MovementManagement(float horizontal, float vertical, bool running, bool sprinting)
	{
        if (attackCooldown <= 0)
        {
            if (timeToNextRoll <= 0)
                Rotating(horizontal, vertical);

            if (isMoving)
            {
                if (sprinting)
                {
                    speed = sprintSpeed;
                }
                else if (running)
                {
                    speed = runSpeed;
                }
                else
                {
                    speed = walkSpeed;
                }

                anim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
            }
            else
            {
                speed = 0f;
                anim.SetFloat(speedFloat, 0f);
            }
            _rb.AddRelativeForce(Vector3.forward * speed);
        }
	}

	Vector3 Rotating(float horizontal, float vertical)
	{
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0.0f;
		forward = forward.normalized;

		Vector3 right = new Vector3(forward.z, 0, -forward.x);

		Vector3 targetDirection;

		float finalTurnSmoothing;

		if(IsAiming())
		{
			targetDirection = forward;
			finalTurnSmoothing = aimTurnSmoothing;
		}
		else
		{
			targetDirection = forward * vertical + right * horizontal;
			finalTurnSmoothing = turnSmoothing;
		}

		if((isMoving && targetDirection != Vector3.zero) || IsAiming())
		{
			Quaternion targetRotation = Quaternion.LookRotation (targetDirection, Vector3.up);

			Quaternion newRotation = Quaternion.Slerp(_rb.rotation, targetRotation, finalTurnSmoothing * Time.deltaTime);
			_rb.MoveRotation (newRotation);
			lastDirection = targetDirection;
		}
		//idle - fly or grounded
		if(!(Mathf.Abs(h) > 0.9 || Mathf.Abs(v) > 0.9))
		{
			Repositioning();
		}

		return targetDirection;
	}	

	private void Repositioning()
	{
		Vector3 repositioning = lastDirection;
		if(repositioning != Vector3.zero)
		{
			repositioning.y = 0;
			Quaternion targetRotation = Quaternion.LookRotation (repositioning, Vector3.up);
			Quaternion newRotation = Quaternion.Slerp(_rb.rotation, targetRotation, turnSmoothing * Time.deltaTime);
			_rb.MoveRotation (newRotation);
		}
	}

	public bool IsAiming()
	{
		return aim;
	}

	public bool isSprinting()
	{
		return sprint && !aim && (isMoving);
	}
}
