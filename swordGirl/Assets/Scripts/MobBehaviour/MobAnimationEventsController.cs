using UnityEngine;
using System.Collections;

public class MobAnimationEventsController : MonoBehaviour {

    MobController mob;

    [SerializeField]
    private AngelKingBodyColliderController[] bodyColliders;


    [SerializeField]
    private ParticleSystem flyParticles;

    [SerializeField]
    private AudioClip[] audioClip;

    [SerializeField]
    private ParticleSystem leftStep;
    [SerializeField]
    private ParticleSystem rightStep;

    [SerializeField]
    AudioClip[] chickenClips;
    AudioSource _audio;

    void Start ()
    {
        mob = transform.parent.GetComponent<MobController>();
        bodyColliders = GetComponentsInChildren<AngelKingBodyColliderController>();
        _audio = GetComponent<AudioSource>();
    }

    public void SetPlayerFlyTime(float time)
    {
        mob.playerFlyTime = time;
    }

    public void AttackOver()
    {
        mob.AttackOver();
    }

    public void Impact(string colliderName)
    {
        for (int i = 0; i < bodyColliders.Length; i++)
        {
            if (bodyColliders[i].name == colliderName && bodyColliders[i].localHealth > 0)
            {
                bodyColliders[i].Impact();
                break;
            }
        }
    }

    public void FlyStart()
    {
        flyParticles.Play();
    }

    public void FlyOver()
    {
        flyParticles.Stop();
    }

    void ChickenSound()
    {
        float pitch = Random.Range(.75f, 1.25f);
        _audio.clip = chickenClips[Random.Range(0, chickenClips.Length)];
        _audio.Play();
        _audio.pitch = pitch;
    }
}
