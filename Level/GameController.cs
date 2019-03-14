using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public float startTimer;
    public bool gameStarted = false;
    public bool gameWin = false;
    public bool gameOver = false;

    public Camera cam;

    public Transform startPos;
    public Transform endPos;

    // Speed in units per sec.
    public float speed;

    public GameObject startText;
    public GameObject successText;
    public GameObject failedText;
    public GameObject retryButton;

    // Use this for initialization
    void Start () {
        cam = Camera.main;
        if (startPos)
        {
            cam.transform.position = new Vector3(startPos.position.x, startPos.position.y, startPos.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted)
        {
            if (gameWin) { return; }
            float dist = Vector3.Distance(endPos.position, cam.transform.position);
            if (dist < 0.1)
            {
                startText.SetActive(true);
                startTimer -= Time.deltaTime;
                if (startTimer <= 0)
                {
                    gameStarted = true;
                    startText.SetActive(false);
                }
            }
            else
            {
                // The step size is equal to speed times frame time.
                float step = speed * Time.deltaTime;

                // Move our position a step closer to the target.
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, endPos.position, step);
            }
        }
        if (gameWin)
        {
            gameStarted = false;
            successText.SetActive(true);
        }
    }
    public void Retry()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
