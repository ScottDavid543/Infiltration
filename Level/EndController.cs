using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndController : MonoBehaviour {

    GameController gameController;
    PlayerProgressController progController;

    string sceneName;

    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        progController = FindObjectOfType<PlayerProgressController>();
        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            gameController.gameWin = true;

            for (int i = 0; i <= progController.LevelsUnlocked.Length; i++)
            {
                if (sceneName == "Level" + i)
                {
                    progController.LevelsUnlocked[i] = true;
                }
            }
        }
    }
}
