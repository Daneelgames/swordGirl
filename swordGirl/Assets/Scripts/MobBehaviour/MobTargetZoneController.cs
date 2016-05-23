using UnityEngine;
using System.Collections;

public class MobTargetZoneController : MonoBehaviour {

    public float playerDamageFlyTime = 0.75f;

    [SerializeField]
    private string nextAttack;
    private MobController mob;

    void Start()
    {
        mob = transform.parent.GetComponent<MobController>();
    }

    void OnTriggerEnter (Collider player)
    {
        if (player.tag == "Player" && mob.monsterState != MobController.State.Dead)
        {
            mob.AddTarger(nextAttack);
            //angelKing.Attack(nextAttack);
        }
    }

    void OnTriggerExit(Collider player)
    {
        if (player.tag == "Player")
        {
            mob.RemoveTarget(nextAttack);
        }
    }

}
