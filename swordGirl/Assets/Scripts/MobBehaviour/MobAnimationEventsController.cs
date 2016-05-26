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

    void Start ()
    {
        mob = transform.parent.GetComponent<MobController>();
        bodyColliders = GetComponentsInChildren<AngelKingBodyColliderController>();
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

    void PlaySound(int clip)
    {
        float pitch = Random.Range(.75f, 1.25f);
        PlayClipAtPoint(audioClip[clip], new Vector3(transform.position.x, transform.position.y, 0), 1f, pitch);
    }

    GameObject PlayClipAtPoint(AudioClip clip, Vector3 position, float volume, float pitch)
    {
        GameObject obj = new GameObject();
        obj.transform.position = position;
        AudioSource _aidio = obj.AddComponent<AudioSource>();
        _aidio.pitch = pitch;
        _aidio.PlayOneShot(clip, volume);
        Destroy(obj, clip.length / pitch);
        return obj;
    }
}
