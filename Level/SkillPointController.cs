using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillPointController : MonoBehaviour {

    PlayerProgressController progController;

    string sceneName;

    // Use this for initialization
    void Start()
    {
        progController = FindObjectOfType<PlayerProgressController>();

        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;

        for (int i = 0; i <= progController.collectedPoints.Length; i++)
        {
            if (sceneName == "Level" + i)
            {
                if(progController.collectedPoints[i]== true)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }

    }
}
