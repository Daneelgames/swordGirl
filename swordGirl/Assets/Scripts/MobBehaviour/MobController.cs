using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobController : MonoBehaviour {

    public enum State { Calm, Agressive, Dead };

    public State monsterState = State.Calm;

    public float health = 0.1f;

    public float playerFlyTime = 0.5f;

    [SerializeField]
    List<string> activeZones = new List<string>();

    public float maxWalkingRange = 20f;
    public float walkImer = 10f;

    public float walkSpeed = 2f;
    public float walkTurnSpeed = 15f;

    public float runSpeed = 2f;
    public float runTurnSpeed = 15f;

    public float maxVelocity = 2;

    private Vector3 target;

    private Rigidbody _rb;
    private Animator _anim;

    private float timer = 0;
    private float attackCooldown = 2f;

    [SerializeField]
    private bool attacking = false;
    [SerializeField]
    private bool move = false;
    private string attack;

    [SerializeField]
    private bool flyUp = false;

    private GameObject player;

    private float dirV = 0f;

    private Vector3 impactPosition;
    [SerializeField]
    private bool grounded = true;
    [SerializeField]
    private bool damaged = false;

    public bool logicOff = false;

    private Collider[] colliders;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player");
        colliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        if (!logicOff)
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            if (monsterState == State.Calm)
                CalmBehaviour();
            else if (monsterState == State.Agressive)
                AgressiveBehaviour();
            else if (monsterState == State.Dead)
            {
                if (!grounded)
                {
                    if (flyUp && dirV < 20)
                        dirV += 20;
                    else if (!flyUp && dirV > -20)
                        dirV -= 20;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!logicOff)
        {
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxVelocity);

            if (move)
            {
                if (monsterState == State.Calm)
                {
                    MoveToTarget(walkSpeed);
                    TurnToTarget(walkTurnSpeed);
                }
                else if (monsterState == State.Agressive)
                {
                    MoveToTarget(runSpeed);
                    TurnToTarget(runTurnSpeed);
                }
            }
            else
            {
                if (!grounded)
                {
                    KickedToSky();

                    if (!flyUp)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 0.3f) && hit.collider.gameObject.tag == "Ground")
                        {
                            //set rotate to zero
                            //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                            _anim.SetTrigger("Dead");
                            grounded = true;
                            _rb.isKinematic = true;
                            logicOff = true;
                        }
                    }
                }
            }
        }
    }

    public void NoticePlayer()
    {
        if (monsterState == State.Calm)
        {
            StartCoroutine("WaitBeforeAgressive");
        }
    }

    IEnumerator WaitBeforeAgressive()
    {
        move = false;
        _anim.SetBool("InBattle", true);

        yield return new WaitForSeconds(2f);

        move = true;
        monsterState = State.Agressive;
    }

    void CalmBehaviour()
    {
        if (timer <= 0)
        {
            //get new position
            target = WalkPosition();
            timer = Random.Range(2, 5);
        }
        else if(Vector3.Distance(transform.position, target) > 5)
        {
            //wandering around
            _anim.SetBool("Run", true);
            move = true;
        }
        else 
        {
            //waiting for new position
            move = false;
            //set rotate to zero
            //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            _rb.velocity = Vector3.zero;
            timer -= 1 * Time.deltaTime;
            _anim.SetBool("Run", false);
        }
    }

    void AgressiveBehaviour()
    {
        target = player.transform.position;
        if (Vector3.Distance(transform.position, target) > 1)
        {
            //run towards player
            _anim.SetBool("Run", true);
            move = true;
        }
        else
        {
            //wait
            _anim.SetBool("Run", false);
            move = false;
        }
        Attack();
    }

    public void AddTarger(string targetZone)
    {
        bool alreadyInList = false;

        if (activeZones.Count != 0)
        {
            foreach (string i in activeZones)
            {
                if (i == targetZone)
                    alreadyInList = true;

            }
            if (!alreadyInList)
                activeZones.Add(targetZone);
        }
        else
            activeZones.Add(targetZone);
    }

    public void RemoveTarget(string targetZone)
    {
        if (activeZones.Count != 0)
        {
            foreach (string i in activeZones)
            {
                if (i == targetZone)
                {
                    activeZones.Remove(targetZone);
                    break;
                }
            }
        }
    }

    void Attack()
    {
        if (attackCooldown > 0)
            attackCooldown -= 1 * Time.deltaTime;

        if (activeZones.Count > 0 && attackCooldown <= 0 && !attacking)
        {
            int random = Random.Range(0, activeZones.Count);
            
            attack = activeZones[random];
            _anim.SetBool(attack, true);
            _anim.SetBool("Run", false);
            attacking = true;

            //set rotate to zero
            //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }

    public void AttackOver()
    {
        attackCooldown = Random.Range(5, 10);
        _anim.SetBool(attack, false);
        attacking = false;

        //set rotate to zero
        //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    public void Damage(Vector3 impactOrigin, float dmg)
    {
        if (monsterState != State.Dead)
        {
            health -= dmg;
            StartCoroutine("Damaged");

            if (health <= 0)
                Dead(impactOrigin, dmg);
        }
    }

    IEnumerator Damaged()
    {
        damaged = true;
        //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        _anim.SetBool("Damage", true);
        yield return new WaitForSeconds(2f);
        damaged = false;
        _anim.SetBool("Damage", false);
    }

    void Dead(Vector3 impactOrigin, float damage)
    {
        if (_anim.GetBool("Damage"))
            _anim.SetBool("Damage", false);

        foreach (Collider coll in colliders)
        {
            coll.isTrigger = true;
        }

        move = false;
        monsterState = State.Dead;
        StartCoroutine("Fall", damage);
        _anim.SetTrigger("FlyUp");
        transform.LookAt(impactOrigin);
        impactPosition = impactOrigin;
        
        GetComponent<Collider>().isTrigger = true;
    }

    IEnumerator Fall(float time)
    {
        //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        grounded = false;
        flyUp = true;
        _anim.SetTrigger("FlyUp");
        yield return new WaitForSeconds(0.75f);
        flyUp = false;
        _anim.SetTrigger("FlyDown");
    }
    
    void KickedToSky()
    {
        _rb.AddForce((new Vector3(transform.position.x, dirV, transform.position.z) - new Vector3(impactPosition.x, transform.position.y, impactPosition.z)) * 20, ForceMode.Force);
    }

    Vector3 WalkPosition()
    {
        float randomX = Random.Range(transform.position.x - maxWalkingRange, transform.position.x + maxWalkingRange);
        float randomZ = Random.Range(transform.position.z - maxWalkingRange, transform.position.z + maxWalkingRange);
        
        Vector3 randomPosition = new Vector3(randomX, transform.position.y, randomZ);

        return randomPosition;
    }

    void MoveToTarget (float moveSpeed)
    {
        if (!damaged)
        {
            _rb.AddRelativeForce(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxVelocity * moveSpeed);
        }
    }

    void TurnToTarget (float turnSpeed)
    {
        if (!damaged)
        {
            Vector3 targetVector = new Vector3(target.x, transform.rotation.y, target.z);
            Quaternion targetRotation = Quaternion.LookRotation(targetVector - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
        }
    }


    void OnCollisionEnter(Collision other)
    {
        MobBodyColliderController activeCollider;
        if (other.gameObject.tag == "Player")
        {
            foreach (ContactPoint _cp in other.contacts)
            {
                if (_cp.thisCollider.tag == "EnemyActionColl")
                {
                    activeCollider = _cp.thisCollider.GetComponent<MobBodyColliderController>();
                    if (activeCollider.isDangerous)
                    {
                        //other.gameObject.GetComponent<PlayerControl>().Damage(new Vector3(_cp.point.x, _cp.point.y - 2, _cp.point.z));
                        other.gameObject.GetComponent<PlayerControl>().Damage(transform.position, playerFlyTime);
                        activeCollider.isDangerous = false;

                        break;
                    }
                }
            }
        }
        if (other.gameObject.tag == "PlayerWeapon")
        {
            foreach (ContactPoint _cp in other.contacts)
            {
                SwordController sword = _cp.otherCollider.GetComponent<SwordController>();
                if (_cp.thisCollider.tag == "EnemyActionColl" && sword.dangerous)
                {
                    StartCoroutine(CoroutineWithMultipleParameters(_cp.otherCollider, _cp.thisCollider));

                    _cp.thisCollider.gameObject.GetComponent<MobBodyColliderController>().Damage(_cp.point, sword.dmg);
                    sword.dangerous = false;
                    break;
                }
            }
        }
    }

    IEnumerator CoroutineWithMultipleParameters(Collider swordCollider, Collider bodyCollider)
    {
        Physics.IgnoreCollision(swordCollider, bodyCollider, true);
        yield return new WaitForSeconds(0.5f);
        Physics.IgnoreCollision(swordCollider, bodyCollider, false);
    }
}