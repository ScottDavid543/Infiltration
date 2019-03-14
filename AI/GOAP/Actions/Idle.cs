using UnityEngine;
using System.Collections;


namespace GOAP {
    public class Idle: Action{
   public Idle(string name, int cost, StateDrivenBrain brain, StateDrivenBrain.TacticalStates moveToState) : base(name, cost, brain, moveToState) { }
        public override ActionStates Initialise() {
            Debug.Log("Start Action : Idle");
            brain.alertImage.SetActive(false);
            brain.chaseImage.SetActive(false);
            GotoNextPoint();
            //brain.animator.SetBool("Walk", true);
            return ActionStates.Running;
        }
        public override ActionStates Update() {
            // Action still running
            //Debug.Log("IDLE");
            if (!brain.navmeshAgent.pathPending && brain.navmeshAgent.remainingDistance < 0.5f)
                GotoNextPoint();
            return ActionStates.Running;
        }
        public override void CleanUp() {
            Debug.Log("End Action : Idle");
        }
        void GotoNextPoint()
        {
            // Returns if no points have been set up
            if (brain.patrolPoints.Length == 0)
                return;
            brain.animator.SetBool("Walk", true);
            brain.animator.SetBool("Run", false);
            // Set the agent to go to the currently selected destination.
            brain.navmeshAgent.destination = brain.patrolPoints[brain.destinationPoint].position;

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.
            brain.destinationPoint = (brain.destinationPoint + 1) % brain.patrolPoints.Length;
        }
    }

}
