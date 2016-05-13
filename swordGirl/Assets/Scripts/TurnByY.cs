using UnityEngine;
using System.Collections;

public class TurnByY : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, (transform.rotation.eulerAngles.y + 1f * Time.deltaTime), transform.rotation.eulerAngles.z);
    }
}
