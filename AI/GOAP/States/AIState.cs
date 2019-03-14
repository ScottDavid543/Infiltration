using UnityEngine;
using System.Collections;

public class AIState<T> : State<T> {
    protected StateDrivenBrain brain;
    protected Vector3 moveTarget;
    protected Vector3 moveDirection;
    protected Vector3 moveRotation;
    protected Quaternion lookAtRotation;
    private float gravityFactor = -50.0f;
    private float gravityForce = 0f;
    protected float angleToTurn;

    public AIState(T stateName, StateDrivenBrain brain, float minDuration): base(stateName, brain, minDuration) {
        this.brain = brain;
    }

    public override void OnEnter() {
        base.OnEnter();
    }

    public override void OnLeave() {
        base.OnLeave();
    }

    public override void OnStateTriggerEnter(Collider collider) {
    }

    public override void Monitor() {

    }

    public override void Act() {
    }

    protected Vector3 ApplyGravity(Vector3 directionToMove) {
        if (brain.characterController.isGrounded) {
            gravityForce = 0.0f;
        }
        else {
            // Force player down
            gravityForce = gravityFactor * Time.deltaTime;
        }
        // Apply gravity
        directionToMove.y += gravityForce;
        return directionToMove;
    }



}
