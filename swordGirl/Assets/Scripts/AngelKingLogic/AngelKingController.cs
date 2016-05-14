using UnityEngine;
using System.Collections;

public class AngelKingController : MonoBehaviour {

    public enum State {Sleep, Idle, Run, Attack};
    public State kingState = State.Idle;

    [SerializeField]
    private float runSpeed = 2;

    private Animator anim;
    private Rigidbody _rb;

    private Transform player;

    private string attack;

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

    }

    void MovementController()
    {
        if (!anim.GetBool("Run"))
            anim.SetBool("Run", true);

        _rb.AddRelativeForce(Vector3.forward * runSpeed);
    }
    
    void TurnController()
    {
        transform.LookAt(new Vector3(player.position.x, 0, player.position.z));
    }

    public void Attack(string boolName)
    {
        if (kingState != State.Attack)
        {
            attack = boolName;
            anim.SetBool(attack, true);
            anim.SetBool("Run", false);
            kingState = State.Attack;
        }
    }

    public void AttackOver()
    {
        kingState = State.Idle;
        anim.SetBool(attack, false);
    }
    
    void OnCollisionEnter(Collision other)
    {
        AngelKingBodyColliderController activeCollider;
        if (other.gameObject.tag == "Player")
            foreach (ContactPoint _cp in other.contacts)
            {
                if (_cp.thisCollider.tag == "EnemyActionColl")
                {
                    activeCollider = _cp.thisCollider.GetComponent<AngelKingBodyColliderController>();
                    if (activeCollider.isDangerous)
                    {
                        //other.gameObject.GetComponent<PlayerControl>().Damage(new Vector3(_cp.point.x, _cp.point.y - 2, _cp.point.z));
                        other.gameObject.GetComponent<PlayerControl>().Damage(transform.position);
                        activeCollider.isDangerous = false;
                    }
                }
            }
    }
}
