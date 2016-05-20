using UnityEngine;
using System.Collections;

public class AngelKingBodyColliderController : MonoBehaviour {

    public float localHealth = 0.3f;

    public bool isTarget = false;

    public AngelKingBodyColliderController childBodypart;

    public bool isDangerous = false;
    public AudioClip[] impactSounds;
    public AudioClip[] hurtSounds;
    private AudioSource _audio;

    [SerializeField]
    private GameObject bloodParticles;
    [SerializeField]
    private GameObject limbDestroyedParticles;

    [SerializeField]
    private GameObject impactParticles;

    private GameManager gm;

    private Animator anim;

    public float timer = -1;

    AngelKingController king;

    bool canTimer = false;

    void Start()
    {
        king = GameObject.Find("AngelKing").GetComponent<AngelKingController>();

        anim = GameObject.Find("AngelKing").GetComponentInChildren<Animator>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (localHealth <= 0)
            BreakCollider();

        if (!anim.GetBool("Standing") && king.kingState == AngelKingController.State.Sleep)
        {
            if  (timer == -1)
            {
                timer = 3f;
                canTimer = true;
            }

            if (timer > 0)
                timer -= 1 * Time.deltaTime;

            if (canTimer && timer < 0)
                timer = 0;
        }

    }

    public void Impact()
    {
        if (localHealth > 0)
        {
            int randomClip = Random.Range(0, impactSounds.Length);
            float randomPitch = Random.Range(0.75f, 1.25f);
            _audio.PlayOneShot(impactSounds[randomClip]);
            _audio.pitch = randomPitch;

            Instantiate(impactParticles, transform.position, new Quaternion(0, 0, 0, 0));
        }
    }

    public void Damage(Vector3 dmgPosition, float damage)
    {
        gm.bossHealth += damage;
        
        localHealth -= damage;

        if (childBodypart != null)
            childBodypart.localHealth -= damage;

        BloodSplatter(dmgPosition);

        if (!anim.GetBool("Standing") && king.kingState == AngelKingController.State.Sleep && timer == 0)
        {
            king.kingState = AngelKingController.State.Idle;
            anim.SetBool("Awake", true);
        }
    }

    void BloodSplatter(Vector3 dmgPosition)
    {
        int randomClip = Random.Range(0, hurtSounds.Length);
        float randomPitch = Random.Range(0.75f, 1.25f);
        _audio.PlayOneShot(hurtSounds[randomClip]);
        _audio.pitch = randomPitch;

        Vector3 direcion = dmgPosition + (transform.position - dmgPosition).normalized * 2;
        // pivot.position + (target.position - pivot.position).normalized * unobstructed

        GameObject blood = Instantiate(bloodParticles, dmgPosition, transform.rotation) as GameObject;
        blood.transform.parent = transform;
        blood.transform.LookAt(direcion);
    }

    public void BreakCollider()
    {
        localHealth = 0;

        //GameObject.Find("CamHolder").GetComponent<CameraController>().BrokeTarget(this);

        if (!anim.GetBool("Standing") && king.kingState == AngelKingController.State.Sleep)
        {
            king.kingState = AngelKingController.State.Idle;
        }

        if (childBodypart != null)
            childBodypart.BreakCollider();

        GetComponent<Collider>().enabled = false;

        SkinnedMeshRenderer mesh = GetComponentInChildren<SkinnedMeshRenderer>();

        if (mesh != null)
            mesh.gameObject.SetActive(false);

        if (anim.GetBool("Standing"))
        {
            anim.SetBool("Standing", false);
            anim.SetBool("Awake", false);
            anim.SetTrigger("LoseLimb");
            king.kingState = AngelKingController.State.Sleep;
            king.FallAsleep();
        }

        Instantiate(limbDestroyedParticles, transform.position, transform.rotation);

        this.enabled = false;
    }


}