using UnityEngine;
using System.Collections;

public class CheckpointController : MonoBehaviour {

    [SerializeField]
    Transform checkpoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GlobalManager.SetPlayerRestartPosition(checkpoint.position);
        }
    }

}
