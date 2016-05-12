using UnityEngine;
using System.Collections;

public class AnimatorEvents : MonoBehaviour {

    [SerializeField]
    private PlayerControl player;

    private Animator anim;

    [SerializeField]
    private ParticleSystem leftStep;
    [SerializeField]
    private ParticleSystem rightStep;

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

    public void StepLeft()
    {
        leftStep.time = 0;
        leftStep.Play();
    }

    public void StepRight()
    {
        rightStep.time = 0;
        rightStep.Play();
    }
}
