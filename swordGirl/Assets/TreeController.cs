using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeController : MonoBehaviour {

    [SerializeField]
    private AudioSource _audio;
    [SerializeField]
    private Animator _anim;
    [SerializeField]
    private GameObject applePrefab;
    [SerializeField]
    private List<Transform> applesOnTree = new List<Transform>();

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            PlayerControl player = coll.gameObject.GetComponent<PlayerControl>();
            if (player.timeToNextRoll > 0.3f)
                ShakeTree();
        }
    }

    void ShakeTree()
    {
        _audio.Play();
        _anim.SetTrigger("Shake");

        DropApple();
    }

    void DropApple()
    {
        if (applesOnTree.Count > 0)
        {
            int random = Random.Range(0, applesOnTree.Count);

            Instantiate(applePrefab, applesOnTree[random].transform.position, applesOnTree[random].transform.rotation);

            applesOnTree[random].gameObject.SetActive(false);
            applesOnTree.RemoveAt(random);
        }
    }
}
