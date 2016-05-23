using UnityEngine;
using System.Collections;

public class BushController : MonoBehaviour {

    [SerializeField]
    private GameObject bushParticles;

    void OnCollisionEnter(Collision coll)
    {
        foreach (ContactPoint cp in coll.contacts)
        {
            if (cp.otherCollider.tag == "Player" || cp.otherCollider.tag == "EnemyMovementColl" || cp.otherCollider.tag == "EnemyMovementColl")
            {
                DestroyBush();
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "PlayerWeapon")
            DestroyBush();
    }

    void DestroyBush()
    {
        Instantiate(bushParticles, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), bushParticles.transform.rotation);
        AudioSource _audio = GetComponent<AudioSource>();
        _audio.pitch = Random.Range(0.75f, 1f);
        PlayClipAtPoint(_audio.clip, transform.position, _audio.pitch, _audio.pitch);
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
