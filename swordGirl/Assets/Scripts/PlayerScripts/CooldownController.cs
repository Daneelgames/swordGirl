using UnityEngine;
using System.Collections;

public class CooldownController : MonoBehaviour {

    [SerializeField]
    private PlayerControl player;

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

	public void SetCoolDown(float coolDown)
    {
        player.attackCooldown = coolDown;
        anim.SetBool("Attack1", false);
    }

    public void SetRollFalse()
    {
        anim.SetBool("Roll", false);
    }
}
