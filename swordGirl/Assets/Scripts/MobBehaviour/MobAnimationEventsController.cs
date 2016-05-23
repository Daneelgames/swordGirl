using UnityEngine;
using System.Collections;

public class MobAnimationEventsController : MonoBehaviour {

    MobController mob;

    void Start ()
    {
        mob = transform.parent.GetComponent<MobController>();
    }

    public void AttackOver()
    {
        mob.AttackOver();
    }

}
