using UnityEngine;
using System.Collections;

public class AngelCorpseController : MonoBehaviour {

    [SerializeField]
    private int pose = 0;

    private Animator anim;

	void Awake () {
        anim = GetComponentInChildren<Animator>();
        anim.SetInteger("Pose", pose);
	}
}