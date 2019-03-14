using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour {

    public GameObject[] locks;
    public GameObject[] levelButtons;

    public GameObject skillPointAlert;

    PlayerProgressController progController;

    private void Awake()
    {
        progController = FindObjectOfType<PlayerProgressController>();
    }
    private void Update()
    {
        for (int i = 0; i < progController.LevelsUnlocked.Length; i++)
        {
            if (progController.LevelsUnlocked[i] == true)
            {
                locks[i].SetActive(false);
                levelButtons[i].SetActive(true);
            }
        }
        if (progController.SkillPoints > 0)
        {
            skillPointAlert.SetActive(true);
        }
        else
        {
            skillPointAlert.SetActive(false);
        }
    }
    public void LoadTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void LoadLevel(int levelNum)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + levelNum);
    }
    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
