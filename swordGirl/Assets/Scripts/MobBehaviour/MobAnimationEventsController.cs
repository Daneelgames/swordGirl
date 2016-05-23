using UnityEngine;
using System.Collections;

public class MobAnimationEventsController : MonoBehaviour {

    MobController mob;

    void Start ()
    {
        mob = transform.parent.GetComponent<MobController>();
    }

    public void SetPlayerFlyTime(float time)
    {
        mob.playerFlyTime = time;
    }

    public void AttackOver()
    {
        mob.AttackOver();
    }
}
