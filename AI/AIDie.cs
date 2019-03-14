using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDie : StateMachineBehaviour {
    GameObject thisObject;
    AudioSource audioSource;
    AudioClip die;
    StateDrivenBrain brain;
     // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        thisObject = animator.gameObject;
        brain = thisObject.GetComponent<StateDrivenBrain>();       
        audioSource = thisObject.GetComponent<AudioSource>();
        if (thisObject.GetComponent<AIController>() != null)
        {
            die = thisObject.GetComponent<AIController>().m_Die;
        }
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(die);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (brain != null)
        {
            brain.alertImage.SetActive(false);
            brain.chaseImage.SetActive(false);
        }
    }

	 //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!thisObject.CompareTag("Player"))
        {
            thisObject.SetActive(false);
        }
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
