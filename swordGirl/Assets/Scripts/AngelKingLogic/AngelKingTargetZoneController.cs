using UnityEngine;
using System.Collections;

public class AngelKingTargetZoneController : MonoBehaviour {

    public float playerDamageFlyTime = 0.75f;

    [SerializeField]
    private string nextAttack;
    private AngelKingController angelKing;

    void Start()
    {
        angelKing = GameObject.Find("AngelKing").GetComponent<AngelKingController>();
    }

    void OnTriggerEnter (Collider player)
    {
        if (player.tag == "Player")
        {
            angelKing.AddTarger(nextAttack);
            //angelKing.Attack(nextAttack);
        }
    }

    void OnTriggerExit(Collider player)
    {
        if (player.tag == "Player")
        {
            angelKing.RemoveTarget(nextAttack);
        }
    }

}
