using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AngelKingController : MonoBehaviour {

    public enum State {Sleep, Idle, Run, Attack};
    public State kingState = State.Idle;

    [SerializeField]
    List<string> activeZones = new List<string>();

    [SerializeField]
    private float runSpeed = 2;
    [SerializeField]
    private float defaultTurnSpeed = 2;

    private Animator anim;
    private Rigidbody _rb;

    private Transform player;

    private string attack;

    private Quaternion targetRotation;

    private float attackCooldown = 1f;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").transform;
    }

    void FixedUpdate()
    {
        if (kingState == State.Run)
            MovementController();
        if (kingState != State.Idle && kingState != State.Sleep)
            TurnController();
    }

    void Update()
    {
        if (kingState == State.Idle)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance > 2)
                kingState = State.Run;
            else if (kingState != State.Attack)
                kingState = State.Idle;
        }

        Attack();
    }

    void MovementController()
    {
        if (!anim.GetBool("Run"))
            anim.SetBool("Run", true);

        _rb.AddRelativeForce(Vector3.forward * runSpeed * 100000f * Time.fixedDeltaTime);
    }
    
    void TurnController()
    {
        Vector3 targetVector = new Vector3(player.position.x, transform.rotation.y, player.position.z);

        targetRotation = Quaternion.LookRotation(targetVector - transform.position);

        if (kingState != State.Attack)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, defaultTurnSpeed * Time.fixedDeltaTime);
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

    public void RemoveTarget (string targetZone)
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

        if (activeZones.Count > 0 && attackCooldown <= 0 && kingState != State.Attack)
        {
            int random = Random.Range(0, activeZones.Count);
            //print(random);
            foreach (string str in activeZones)
            {
               //print(str);
            }
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            attack = activeZones[random];
            anim.SetBool(attack, true);
            anim.SetBool("Run", false);
            kingState = State.Attack;
        }
    }

    public void AttackOver()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        attackCooldown = Random.Range(0, 3);
        kingState = State.Idle;
        anim.SetBool(attack, false);
    }
    
    void OnCollisionEnter(Collision other)
    {
        AngelKingBodyColliderController activeCollider;
        if (other.gameObject.tag == "Player")
        {
            foreach (ContactPoint _cp in other.contacts)
            {
                if (_cp.thisCollider.tag == "EnemyActionColl")
                {
                    activeCollider = _cp.thisCollider.GetComponent<AngelKingBodyColliderController>();
                    if (activeCollider.isDangerous && activeCollider.localHealth > 0)
                    {
                        //other.gameObject.GetComponent<PlayerControl>().Damage(new Vector3(_cp.point.x, _cp.point.y - 2, _cp.point.z));
                        other.gameObject.GetComponent<PlayerControl>().Damage(transform.position);
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
                if (_cp.thisCollider.tag == "EnemyActionColl" && _cp.otherCollider.GetComponent<SwordController>().dangerous)
                {
                    print("hit enemy");
                    float dmg = Random.Range(0.02f, 0.01f);
                    StartCoroutine(CoroutineWithMultipleParameters(_cp.otherCollider, _cp.thisCollider));

                    _cp.thisCollider.gameObject.GetComponent<AngelKingBodyColliderController>().Damage(_cp.point, dmg);


                    break;
                }
            }
        }
    }

    IEnumerator CoroutineWithMultipleParameters(Collider swordCollider, Collider bodyCollider)
    {
        //Physics.IgnoreCollision(swordCollider, bodyCollider, true);
        Physics.IgnoreLayerCollision(15, 10, true);
        yield return new WaitForSeconds(0.5f);
        //Physics.IgnoreCollision(swordCollider, bodyCollider, false);
        Physics.IgnoreLayerCollision(15, 10, false);
    }
}