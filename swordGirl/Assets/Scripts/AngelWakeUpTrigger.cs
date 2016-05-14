using UnityEngine;
using System.Collections;

public class AngelWakeUpTrigger : MonoBehaviour {

    [SerializeField]
    private Animator angelKingAnimator;

	void OnTriggerEnter(Collider player)
    {
        if (player.tag == "Player")
        {
            angelKingAnimator.SetBool("Awake", true);
            Destroy(gameObject);
        }
    }
}
