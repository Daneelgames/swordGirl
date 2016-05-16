using UnityEngine;
using System.Collections;

public class AngelKingBodyColliderController : MonoBehaviour {

    public bool isDangerous = false;
    public AudioClip[] sounds;
    private AudioSource _audio;

    [SerializeField]
    private GameObject impactParticles;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void Impact()
    {
        int randomClip = Random.Range(0, sounds.Length);
        float randomPitch = Random.Range(0.75f, 1.25f);
        _audio.PlayOneShot(sounds[randomClip]);
        _audio.pitch = randomPitch;
        
        Instantiate(impactParticles, transform.position, new Quaternion(0,0,0,0));
    }

}
