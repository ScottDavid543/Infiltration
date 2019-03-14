using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRunSound : StateMachineBehaviour {

    GameObject AI;
    AudioSource audioSource;
    AIController aiControl;
    AudioClip WalkSound;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.gameObject;
        audioSource = AI.GetComponent<AudioSource>();
        aiControl = AI.GetComponent<AIController>();
        WalkSound = aiControl.m_footStep;
        audioSource.volume = 0.025f;
        //audioSource.PlayOneShot(WalkSound);
        aiControl.audioDelayReset = 0.4f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        aiControl.audioDelay -= Time.deltaTime;
        if (aiControl.audioDelay <= 0)
        {
            audioSource.volume = 0.015f;
            audioSource.PlayOneShot(WalkSound);
            aiControl.audioDelay = aiControl.audioDelayReset;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

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
