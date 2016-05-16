using UnityEngine;
using System.Collections;

public class AngelKingBodyColliderController : MonoBehaviour {

    public bool isDangerous = false;

    [SerializeField]
    private GameObject impactParticles;

    public void Impact()
    {
        Instantiate(impactParticles, transform.position, new Quaternion(0,0,0,0));
    }

}
