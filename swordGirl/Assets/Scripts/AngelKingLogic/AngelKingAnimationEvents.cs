using UnityEngine;
using System.Collections;

public class AngelKingAnimationEvents : MonoBehaviour {

    [SerializeField]
    private AngelKingController angel;

    [SerializeField]
    private AngelKingBodyColliderController[] bodyColliders;

    void Start()
    {
        bodyColliders = GetComponentsInChildren<AngelKingBodyColliderController>();
    }

    public void AttackOver()
    {
        angel.AttackOver();
    }

    public void SetFlyTime(float playerDamageFlyTime)
    {
        angel.playerFlyTime = playerDamageFlyTime;
    }

    public void Impact (string colliderName)
    {
        for (int i = 0; i < bodyColliders.Length; i++ )
        {
            if (bodyColliders[i].name == colliderName && bodyColliders[i].localHealth > 0)
            {
                bodyColliders[i].Impact();
                break;
            }
        }
    }
}
