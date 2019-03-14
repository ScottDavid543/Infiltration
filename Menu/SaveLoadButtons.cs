using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadButtons : MonoBehaviour {
    public Button saveButton;
    public Button loadButton;

    PlayerProgressController progControl;

	// Use this for initialization
	void Start () {
        progControl = FindObjectOfType<PlayerProgressController>();
	}
	
	// Update is called once per frame
	void Update () {
        saveButton.onClick.AddListener(progControl.SavePlayer);
        loadButton.onClick.AddListener(progControl.LoadPlayer);
    }
}
