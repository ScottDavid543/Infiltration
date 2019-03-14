using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perception;

public class ThrowableObject : MonoBehaviour {
    public BoxCollider bCollider;
    public BoxCollider PickupZone;

    public AudioSource audioSource;
    public AudioClip collideSound;

    public PerceptionManager pm;
    public Rigidbody rb;

    public bool makeNoise = false;

    public float startDelay = 2;

	// Use this for initialization
	void Start ()
    {
        audioSource = this.GetComponent<AudioSource>();
        pm = GameObject.FindGameObjectWithTag("GameController").GetComponent<PerceptionManager>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
	}
    private void Update()
    {
         // sets delay at start of game so stimulus isnt created upon spawning
        if (makeNoise != true)
        {
            startDelay -= Time.deltaTime;
            if (startDelay <= 0)
            {
                makeNoise = true;
            }
        }
    }
    // Upon colliding with something, make noise and create a stimulus within a range
    private void OnCollisionEnter(Collision col)
    {
        if (makeNoise)
        {
            audioSource.PlayOneShot(collideSound);
            // AI within the range of this stimulus will recieve it in their perception system
            pm.AcceptStimulus(new Stimulus(Stimulus.StimulusTypes.AudioCollision, this.gameObject, this.transform.position, transform.forward, 10f, null));
        }
    }
}
