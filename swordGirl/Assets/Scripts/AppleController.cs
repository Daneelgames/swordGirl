using UnityEngine;
using System.Collections;

public class AppleController : MonoBehaviour {

    [SerializeField]
    private AudioSource _audio;
    [SerializeField]
    private AudioClip eatSound;
    [SerializeField]
    private Animator _anim;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Collider coll;

    private bool eaten = false;

    void OnCollisionEnter(Collision coll)
    {
        switch (coll.gameObject.tag)
        {
            case "Player":
                if (!eaten)
                {
                    eaten = true;
                    StartCoroutine("EatApple", coll.gameObject);
                }
                break;

            case "Ground":
                _audio.pitch = Random.Range(0.75f, 1.25f);
                _audio.Play();
                break;

            case "Obstacle":
                _audio.pitch = Random.Range(0.75f, 1.25f);
                _audio.Play();
                break;

            default:
                _audio.pitch = Random.Range(0.75f, 1.25f);
                _audio.Play();
                break;
        }
    }

    IEnumerator EatApple(GameObject player)
    {
        rb.AddForce(Vector3.up * 4, ForceMode.Impulse);
        coll.isTrigger = true;
        GameObject.Find("GameManager").GetComponent<GameManager>().HealPlayer();
        player.GetComponent<PlayerControl>().Heal();
        _audio.pitch = Random.Range(0.75f, 1f);
        PlayClipAtPoint(eatSound, transform.position, _audio.pitch, _audio.pitch);
        _anim.SetTrigger("Eat");
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    GameObject PlayClipAtPoint(AudioClip clip, Vector3 position, float volume, float pitch)
    {
        GameObject obj = new GameObject();
        obj.transform.position = position;
        AudioSource _audio = obj.AddComponent<AudioSource>() as AudioSource;
        _audio.spread = 0;
        _audio.minDistance = 0;
        _audio.maxDistance = 20;
        _audio.spatialBlend = 1;
        _audio.rolloffMode = AudioRolloffMode.Linear;
        _audio.pitch = pitch;
        _audio.PlayOneShot(clip, volume);
        Destroy(obj, clip.length / pitch);
        return obj;
    }
}
