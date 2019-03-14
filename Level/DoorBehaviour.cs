using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour {

    public bool doorsLocked;
    public GameObject[] doors;
    GameObject Player;
    public GameObject requiredKey;
	// Use this for initialization
	void Start () {
        Player = GameObject.FindGameObjectWithTag(Tags.Player);
        doorsLocked = true;
    }
	
	// Update is called once per frame
	void Update () {
		if(doorsLocked == false)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].SetActive(false);
            }
        }
	}
}
