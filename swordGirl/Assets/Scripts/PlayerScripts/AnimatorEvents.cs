using UnityEngine;
using System.Collections;

public class AnimatorEvents : MonoBehaviour {

    [SerializeField]
    private PlayerControl player;

    [SerializeField]
    private TrailRenderer trail;

    private Animator anim;

    [SerializeField]
    private ParticleSystem leftStep;
    [SerializeField]
    private ParticleSystem rightStep;

    private GameManager gm;

    [SerializeField]
    private SwordController sword;

    [SerializeField]
    private ParticleSystem rollParticles;

    void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        trail.enabled = false;
    }

	public void SetCoolDown(float coolDown)
    {
        player.attackCooldown = coolDown;
        anim.SetBool("Attack1", false);
    }

    public void Roll()
    {

        rollParticles.time = 0;
        rollParticles.Play();
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

    public void TrailStart()
    {
        trail.enabled = true;
        sword.dangerous = true;
    }

    public void TrailStop()
    {
        trail.enabled = false;
        sword.dangerous = false;
    }

    public void StaminaUse(float amount)
    {
        if (gm.playerStamina > 0)
            gm.playerStamina -= amount;
    }
}
