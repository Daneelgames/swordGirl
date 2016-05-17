using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour {

    public bool dangerous = false;

    [SerializeField]
    private PlayerControl player;
    [SerializeField]
    private Animator anim;

    void OnCollisionEnter (Collision other)
    {
        print("hit collision");
        if (dangerous)
        {
            print(other.gameObject.name);

            if (other.gameObject.tag == "Obstacle")
            {
                anim.SetTrigger("HitObstacle");
                dangerous = false;
            }
            else
            {
                foreach (ContactPoint _cp in other.contacts)
                {
                    if (_cp.thisCollider.tag == "EnemyActionColl")
                    {
                        print("hit enemy");
                        float dmg = Random.Range(0.025f, 0.05f);

                        _cp.thisCollider.gameObject.GetComponent<AngelKingBodyColliderController>().Damage(_cp.point, dmg);
                        dangerous = false;

                        break;
                    }
                }
            }
        }
    }
}