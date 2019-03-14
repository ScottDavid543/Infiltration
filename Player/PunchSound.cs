using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchSound : StateMachineBehaviour {
    BoxCollider punchCollider;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        AudioSource audioSource = player.GetComponent<AudioSource>();
        AudioClip punchSound = player.GetComponent<PlayerController>().m_roll;
        punchCollider = player.GetComponent<PlayerController>().punchCol;
        punchCollider.enabled = true;
        audioSource.volume = 1f;
        audioSource.PlayOneShot(punchSound);
       // Debug.Log("PUNCH SOUND IN ANIMATOR");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        punchCollider.enabled = false;
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
