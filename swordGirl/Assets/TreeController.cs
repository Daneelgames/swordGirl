using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeController : MonoBehaviour {

    [SerializeField]
    private List<Transform> applesOnTree = new List<Transform>();
    [SerializeField]
    private Animator _anim;

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            PlayerControl player = coll.gameObject.GetComponent<PlayerControl>();
            if (player.timeToNextRoll > 0.5f)
                DropApple();
        }
    }

    void DropApple()
    {
        _anim.SetTrigger("ShakeTree");
    }
}
