using UnityEngine;
using System.Collections;

public class AngelKingTargetZoneController : MonoBehaviour {

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
            angelKing.Attack(nextAttack);
        }
    }

}
