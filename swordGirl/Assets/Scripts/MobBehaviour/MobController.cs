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

    private bool attacking = false;
    private bool move = false;
    private string attack;

    private GameObject player;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (monsterState == State.Calm)
            CalmBehaviour();
        else if (monsterState == State.Agressive)
            AgressiveBehaviour();
        else if (monsterState == State.Dead)
        { }
    }

    void FixedUpdate()
    {
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
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
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
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }

    public void AttackOver()
    {
        attackCooldown = Random.Range(3, 7);
        _anim.SetBool(attack, false);
        attacking = false;

        //set rotate to zero
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    public void Damage(float dmg)
    {
        if (monsterState != State.Dead)
        {
            health -= dmg;

            if (health <= 0)
                Dead(dmg);
        }
    }

    void Dead(float damage)
    {
        monsterState = State.Dead;
        _anim.SetTrigger("FlyUp");
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
        _rb.AddRelativeForce(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxVelocity * moveSpeed);
    }

    void TurnToTarget (float turnSpeed)
    {
        Vector3 targetVector = new Vector3(target.x, transform.rotation.y, target.z);
        Quaternion targetRotation = Quaternion.LookRotation(targetVector - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
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