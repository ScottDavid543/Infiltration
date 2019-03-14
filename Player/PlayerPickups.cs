using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPickups : MonoBehaviour {

    public List<GameObject> Keys;
    PlayerProgressController progController;
    string sceneName;
	// Use this for initialization
	void Start () {
        Keys = new List<GameObject>();
        progController = GameObject.FindObjectOfType<PlayerProgressController>();

        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Key"))
        {
            Keys.Add(col.gameObject);
            col.gameObject.SetActive(false);
        }
        if (col.gameObject.CompareTag("SkillPoint"))
        {
            for (int i = 0; i < progController.collectedPoints.Length; i++)
            {
                if (sceneName == "Level" + i)
                {
                    progController.collectedPoints[i] = true;
                }
            }
            col.gameObject.SetActive(false);
            progController.SkillPoints = progController.SkillPoints + 2;
        }
    }
}
