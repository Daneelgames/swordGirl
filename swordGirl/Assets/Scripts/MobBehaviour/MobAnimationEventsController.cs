using UnityEngine;
using System.Collections;

public class MobAnimationEventsController : MonoBehaviour {

    MobController mob;

    [SerializeField]
    private AngelKingBodyColliderController[] bodyColliders;


    [SerializeField]
    private ParticleSystem flyParticles;

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
}
