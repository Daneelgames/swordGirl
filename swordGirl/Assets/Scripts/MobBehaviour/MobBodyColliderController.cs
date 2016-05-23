using UnityEngine;
using System.Collections;

public class MobBodyColliderController : MonoBehaviour {
    
    public bool isDangerous = false;

    public AudioClip[] impactSounds;
    public AudioClip[] hurtSounds;
    
    [SerializeField]
    private GameObject bloodParticles;

    [SerializeField]
    private GameObject impactParticles;

    [SerializeField]
    private Animator _anim;

    [SerializeField]
    private MobController mob;

    private AudioSource _audio;

    bool canTimer = false;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void Impact()
    {
        if (mob.monsterState != MobController.State.Dead)
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
        if (mob.monsterState != MobController.State.Dead)
        {
            mob.Damage(dmgPosition, damage);
            BloodSplatter(dmgPosition);
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
}