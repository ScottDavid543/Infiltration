using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImageBehaviour : MonoBehaviour {
    public Transform imagePosition;
    public GameObject alertImage;
    public GameObject chaseImage;
    public Camera cam;

    // Use this for initialization
    void Start () {
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = cam.WorldToScreenPoint(imagePosition.position);
        alertImage.transform.position = pos;
        chaseImage.transform.position = pos;
    }
}
