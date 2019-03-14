using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // The Camera Target
    [SerializeField]
    Transform m_PlayerTransform;

    // The Z Distance from the Camera Target
    [SerializeField]
    float m_CameraDistanceZ = 15.0f;

    GameController gameController;
    // Use this for initialization
    void Start ()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (gameController.gameStarted)
        {
            transform.position = new Vector3(m_PlayerTransform.position.x, 15f, m_PlayerTransform.position.z - m_CameraDistanceZ);
        }
	}
}
