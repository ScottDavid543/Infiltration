using UnityEngine;
using System.Collections;

    // Plays an animation and exits the state when the animation is finished.
    public class Animate<T> : AIState<T> {

        public Animate(T stateName, StateDrivenBrain controller, float minDuration) : base(stateName, controller, minDuration) { }

        public override void OnEnter() {
            base.OnEnter();
            // Use root motion
           // brain.animator.applyRootMotion = true;
            // Assumes no movement
            //brain.animator.SetFloat("Speed", 0f);
            // Initialisation will fail if the action cann't be completed
            actionStatus = brain.currentAction.Initialise();
        }
        public override void OnLeave() {
            base.OnLeave();
            brain.currentAction.CleanUp();
            // On successful completion of the task the effects are applied to the agent's WS
            if (actionStatus == GOAP.ActionStates.Success) {
                brain.startWS.ApplyEffects(brain.currentAction.effects);
            }
            brain.currentAction = brain.plan.Pop();
        }

        public override void OnAnimationEnded() {
            base.OnAnimationEnded();
            // State exits when animation is complete
            stateFinished = true;
        }
    }


