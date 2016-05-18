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
    [SerializeField]
    private ParticleSystem impactParticles;

    private bool isTrail = false;

    [SerializeField]
    private AudioClip[] audioClip;

    private float pitch;

    private Animator camTarget;

    private float timer = 0f;

    void Start()
    {
        camTarget = transform.parent.Find("/CamHolder/CamTarget").GetComponent<Animator>();
    }

    void PlaySound(int clip)
    {
        if (timer <= 0)
        {
            pitch = Random.Range(.75f, 1.25f);
            PlayClipAtPoint(audioClip[clip], new Vector3(transform.position.x, transform.position.y, 0), 1f, pitch);
            timer = 0.025f;
        }
    }

    GameObject PlayClipAtPoint(AudioClip clip, Vector3 position, float volume, float pitch)
    {
        GameObject obj = new GameObject();
        obj.transform.position = position;
        obj.AddComponent<AudioSource>();
        obj.GetComponent<AudioSource>().pitch = pitch;
        obj.GetComponent<AudioSource>().PlayOneShot(clip, volume);
        Destroy(obj, clip.length / pitch);
        return obj;
    }

    void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        trail.time = 0f;
    }

    void Update()
    {
        if (!isTrail)
            trail.time = Mathf.Lerp(trail.time, 0, 5 * Time.deltaTime);
        else
            trail.time = Mathf.Lerp(trail.time, 1, 5 * Time.deltaTime);

        if (timer > 0)
            timer -= 1 * Time.deltaTime;

    }

	public void SetCoolDown(float coolDown)
    {
        PlaySound(2);
        player.attackCooldown = coolDown;
        anim.SetBool("Attack1", false);
    }

    public void Roll()
    {
        player.Roll();
        PlaySound(1);
        rollParticles.time = 0;
        rollParticles.Play();
    }

    public void SetRollFalse()
    {
        anim.SetBool("Roll", false);
    }

    public void StepLeft()
    {
        PlaySound(0);
        leftStep.time = 0;
        leftStep.Play();
    }

    public void StepRight()
    {
        PlaySound(0);
        rightStep.time = 0;
        rightStep.Play();
    }

    public void TrailStart(float damage)
    {
        int random = Random.Range(3, 7);

        PlaySound(random);
        //trail.time = 0.5f;
        //trail.enabled = true;
        isTrail = true;
        sword.dangerous = true;
        sword.dmg = damage;
    }

    public void TrailStop()
    {
        //trail.time = 0.05f;
        //trail.enabled = false;
        isTrail = false;
        sword.dangerous = false;
    }

    public void FlyStart()
    {
        rollParticles.time = 0;
        rollParticles.loop = true;
        rollParticles.Play();
    }

    public void FlyOver()
    {
        rollParticles.loop = false;
        rollParticles.Stop();
    }

    public void Impact(int attacked)
    {
        int random = Random.Range(7, 12);
        float randomDamage = Random.Range(0.15f, 0.3f);

        camTarget.SetTrigger("QuickShake");

        if (gm.playerHealth > 0)
        {
            if (attacked > 0)
                gm.playerHealth -= randomDamage;
            else
                gm.playerHealth -= 0.1f;
        }

        PlaySound(random);

        impactParticles.time = 0;
        impactParticles.Play();
    }

    public void StaminaUse(float amount)
    {

        camTarget.SetTrigger("AttackShake");

        if (gm.playerStamina > 0)
            gm.playerStamina -= amount;
    }

    public void SetCanControl()
    {
        player.SetCanControl();
    }

    public void RollInvicibleStart()
    {
        StartCoroutine("Invicibility");
    }
    IEnumerator Invicibility()
    {
        player.invicible = true;
        yield return new WaitForSeconds(0.25f);
        player.invicible = false;
    }

    public void MoveForwardAtAttack(float multiplyForce)
    {
        player.StepForward(multiplyForce);
    }
}