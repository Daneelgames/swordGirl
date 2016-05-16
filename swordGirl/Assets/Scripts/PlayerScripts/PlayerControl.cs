using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private float impactForce = 350;

    public float walkSpeed = 0.15f;
	public float runSpeed = 1.0f;
	public float sprintSpeed = 2.0f;

    public float maxVelocity = 10;
    
	public float turnSmoothing = 3.0f;
    [HideInInspector]
    public float speedDampTime = 1f;
    
	public float rollLength = 5.0f;

	public float rollCooldown = 1.0f;

    public float attackCooldown = 0f;

    public float timeToNextRoll = 0;
	

    [SerializeField]
	private Animator anim;
    [SerializeField]
    private Rigidbody _rb;

    private float speed;

    private Vector3 lastDirection;

    private int speedFloat;
	private int hFloat;
	private int vFloat;
	private int flyBool;
	private int groundedBool;
	private Transform cameraTransform;

	private float h;
	private float v;


	private bool run;
	private bool sprint;

	private bool isMoving;
    
    private bool grounded = true;
    private bool canControl = true;
    private bool flyUp = false;
    private Vector3 impactPosition;

    private GameManager gm;

    float dirV = 0f;
    private bool damaged = false;

    void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (anim == null)
		    anim = GetComponent<Animator> ();
		cameraTransform = Camera.main.transform;

		speedFloat = Animator.StringToHash("Speed");
		hFloat = Animator.StringToHash("H");
		vFloat = Animator.StringToHash("V");
		groundedBool = Animator.StringToHash("Grounded");
		//sprintFactor = sprintSpeed / runSpeed;
	}
	void Update()
	{
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");
		run = Input.GetButton ("Run");
		sprint = Input.GetButton ("Sprint");
		isMoving = Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1;
        if (grounded && canControl)
        {
            AttackManagment();
            RollManagement();
        }
        else
        {
            if (flyUp && dirV < 10)
                dirV += 1;
            else if (!flyUp)
                dirV -= 1;
        }
    }

	void FixedUpdate()
	{
        LimitVelocity();

        if (grounded && canControl)
            MovementManagement(h, v, run, sprint);
        else if (!grounded && !canControl)
        {
            KickedToSky();
        }

        if (!grounded && !flyUp)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, 0.3f) && hit.collider.gameObject.tag == "Ground")
            {
                grounded = true;
                anim.SetBool("HitGround", true);
                Physics.IgnoreLayerCollision(10, 13, false);
            }
        }

        if (grounded && !canControl)
            _rb.velocity = Vector3.zero;
    }

    public void SetCanControl()
    {
        canControl = true;
    }
    
    void KickedToSky()
    {
        _rb.AddForce((new Vector3(transform.position.x, dirV, transform.position.z) - new Vector3(impactPosition.x, 0, impactPosition.z)) * impactForce, ForceMode.Force);
    }

    void LimitVelocity()
    {
        if (_rb.velocity.magnitude > maxVelocity)
        {
            if (!sprint && timeToNextRoll <= 0)
                _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxVelocity);
            else
                _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxVelocity * 2f);
        }
    }

    void AttackManagment()
    {
        if (attackCooldown > 0)
            attackCooldown -= 1 * Time.deltaTime;
        
        if (attackCooldown <= 0 && gm.playerStamina > .1f && Input.GetButtonDown("Attack1"))
            anim.SetBool("Attack1", true);
    }

	void RollManagement()
    {
        if (timeToNextRoll > 0)
            timeToNextRoll -= Time.deltaTime;
        
        if (attackCooldown <= 0 && gm.playerStamina > 0 && Input.GetButtonDown ("Roll"))
        {
            anim.SetBool("Roll", true);
		}
	}

    public void Roll()
    {
        _rb.AddRelativeForce(Vector3.forward * rollLength, ForceMode.Impulse);
        timeToNextRoll = rollCooldown;
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
        
		targetDirection = forward * vertical + right * horizontal;
		finalTurnSmoothing = turnSmoothing;

		if((isMoving && targetDirection != Vector3.zero))
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
    public void Damage(Vector3 impactOrigin)
    {
        if (!damaged)
        {
            transform.LookAt(impactOrigin);
            impactPosition = impactOrigin;
            StartCoroutine("Fall");
            print("Fly up");
            damaged = true;
        }
    }

    IEnumerator Fall()
    {
        Physics.IgnoreLayerCollision(10, 13, true);
        dirV = 10;
        canControl = false;
        flyUp = true;
        grounded = false;
        anim.SetBool("HitGround", false);
        anim.SetBool("Attack1", false);
        anim.SetBool("Roll", false);
        anim.SetTrigger("FlyUp");
        yield return new WaitForSeconds(1f);
        transform.Rotate(0, transform.rotation.y, 0);
        damaged = false;
        flyUp = false;
        anim.SetTrigger("FlyDown");
    }

    public bool isSprinting()
	{
		return sprint && (isMoving);
	}
}