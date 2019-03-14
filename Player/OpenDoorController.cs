using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorController : MonoBehaviour {
    public DoorBehaviour Door;
    public PlayerPickups playerPickups;

	// Use this for initialization
	void Start () {
        playerPickups = GetComponent<PlayerPickups>();
    }
	
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Door"))
        {
            for (int i = 0; i < playerPickups.Keys.Count; i++)
            {
                Door = col.gameObject.GetComponentInParent<DoorBehaviour>();
                if ( playerPickups.Keys[i] == Door.requiredKey )
                {
                    playerPickups.Keys.Remove(playerPickups.Keys[i]);                 
                    Door.doorsLocked = false;
                    return;
                }
            }   
        }
    }
}
