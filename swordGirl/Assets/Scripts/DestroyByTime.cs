using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour {

    [SerializeField]
    private float time;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, time);
	}
}
