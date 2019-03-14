using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    public int currentArea;

    public Transform[] spawnPoints;
    public Transform currentSpawnPoint;

    public GameObject[] tutorialTriggers;
    public GameObject[] tutorialText;

    public GameObject[] endZones;

    Health playerHealth;
    Animator animator;
    GameController gameControl;

    // Use this for initialization
    void Start () {
        playerHealth = GetComponent<Health>();
        animator = GetComponent<Animator>();
        gameControl = FindObjectOfType<GameController>();

        currentSpawnPoint = spawnPoints[currentArea];
	}
	

    public void Respawn()
    {
        playerHealth.m_Health = playerHealth.startingHealth;
        playerHealth.isDead = false;
        animator.SetTrigger("Respawn");
        transform.position = currentSpawnPoint.transform.position;
        gameObject.SetActive(true);
    }

    public void OnTriggerEnter(Collider col)
    {
        for (int i = 0; i < endZones.Length; i++)
        {
            if (col.gameObject == endZones[i])
            {
                if (currentArea == 7)
                {
                    gameControl.gameWin = true;
                }
                else
                {
                    currentArea = i + 1;
                    currentSpawnPoint = spawnPoints[currentArea];
                    this.transform.position = currentSpawnPoint.position;
                }
            }
        }
        for (int i = 0; i < tutorialTriggers.Length; i++)
        {
            if (col.gameObject == tutorialTriggers[i])
            {
                tutorialText[i].SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider col)
    {
        for (int i = 0; i < tutorialTriggers.Length; i++)
        {
            if (col.gameObject == tutorialTriggers[i])
            {
                tutorialText[i].SetActive(false);
            }
        }
    }
}
