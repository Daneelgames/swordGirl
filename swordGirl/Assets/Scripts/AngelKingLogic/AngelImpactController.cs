using UnityEngine;
using System.Collections;

public class AngelImpactController : MonoBehaviour {

    void Start()
    {
        StartCoroutine("DisableCollider");
    }

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(0.05f);
        GetComponent<Collider>().enabled = false;
    }

	void OnTriggerEnter (Collider coll)
    {
        if (coll.tag == "Player")
        {
            coll.GetComponent<PlayerControl>().Damage(new Vector3(transform.position.x, coll.transform.position.y, transform.position.z));
            GetComponent<Collider>().enabled = false;
        }
    }
}
