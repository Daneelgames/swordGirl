using UnityEngine;
using System.Collections;

public class AngelCorpseController : MonoBehaviour {

    [SerializeField]
    private int pose = 0;

	void Awake () {
        GetComponentInChildren<Animator>().SetInteger("Pose", pose);
	}
}
