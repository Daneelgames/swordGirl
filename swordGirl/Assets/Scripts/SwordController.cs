using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour {

    public bool dangerous = false;

    [SerializeField]
    private PlayerControl player;
    [SerializeField]
    private Animator anim;

    void OnTriggerEnter (Collider other)
    {
        if (dangerous)
        {
            if (other.tag == "Obstacle")
                anim.SetTrigger("HitObstacle");

        }
    }

}
