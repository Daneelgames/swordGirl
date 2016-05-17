using UnityEngine;
using System.Collections;

public class AngelKingBodyColliderController : MonoBehaviour {

    public float localHealth = 0.3f;

    public bool isDangerous = false;
    public AudioClip[] impactSounds;
    public AudioClip[] hurtSounds;
    private AudioSource _audio;

    [SerializeField]
    private GameObject bloodParticles;

    [SerializeField]
    private GameObject impactParticles;

    private GameManager gm;

    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _audio = GetComponent<AudioSource>();
    }

    public void Impact()
    {
        int randomClip = Random.Range(0, impactSounds.Length);
        float randomPitch = Random.Range(0.75f, 1.25f);
        _audio.PlayOneShot(impactSounds[randomClip]);
        _audio.pitch = randomPitch;
        
        Instantiate(impactParticles, transform.position, new Quaternion(0,0,0,0));
    }

    public void Damage(Vector3 dmgPosition, float damage)
    {
        if (gm.bossHealth > damage)
            gm.bossHealth -= damage;
        else if (gm.bossHealth > 0)
            gm.bossHealth = 0;

        if (localHealth > damage)
            localHealth -= damage;
        else if (localHealth > 0)
            BreakCollider();

        print("damage");

        BloodSplatter(dmgPosition);
    }

    void BloodSplatter(Vector3 dmgPosition)
    {
        int randomClip = Random.Range(0, hurtSounds.Length);
        float randomPitch = Random.Range(0.75f, 1.25f);
        _audio.PlayOneShot(hurtSounds[randomClip]);
        _audio.pitch = randomPitch;

        Vector3 direcion = transform.position - dmgPosition;

        GameObject blood = Instantiate(bloodParticles, dmgPosition, transform.rotation) as GameObject;
        blood.transform.LookAt(direcion);
        blood.transform.parent = transform;
    }

    void BreakCollider()
    {
        localHealth = 0;
    }
}
