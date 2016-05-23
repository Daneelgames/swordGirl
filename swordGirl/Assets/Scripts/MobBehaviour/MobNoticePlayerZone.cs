using UnityEngine;
using System.Collections;

public class MobNoticePlayerZone : MonoBehaviour {

    private MobController mob;

    void Start()
    {
        mob = transform.parent.GetComponent<MobController>();
    }

	void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
            mob.NoticePlayer();
    }
}
