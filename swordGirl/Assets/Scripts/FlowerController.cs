using UnityEngine;
using System.Collections;

public class FlowerController : MonoBehaviour {

    [SerializeField]
    Animator _anim;

    [SerializeField]
    AudioSource _audio;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            _anim.SetTrigger("Action");
            _audio.pitch = Random.Range(0.75f, 1.25f);
            _audio.Play();
        }
    }
}