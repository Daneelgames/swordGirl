using UnityEngine;
using System.Collections;

public class AngelEventsController : MonoBehaviour {

    [SerializeField]
    private Animator angelKingAnimator;
    [SerializeField]
    private AngelKingController angelKingController;

    void OnTriggerEnter(Collider player)
    {
        if (player.tag == "Player")
        {
            StartCoroutine("AwakeKing");
        }
    }

    IEnumerator AwakeKing()
    {
        angelKingAnimator.SetBool("Awake", true);
        GameObject.Find("GameManager").GetComponent<GameManager>().ShowBossHealth();
        yield return new WaitForSeconds(5f);
        angelKingController.kingState = AngelKingController.State.Idle;

        GetComponent<Collider>().enabled = false;
    }
}
