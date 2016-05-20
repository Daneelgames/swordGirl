using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FaceController : MonoBehaviour {

    public List<GameObject> faces = new List<GameObject>();
    
    public void ChangeFace()
    {
        int random = Random.Range(0, faces.Count);

        foreach (GameObject face in faces)
        {
           face.SetActive(false);
        }
        faces[random].SetActive(true);
    }
}
