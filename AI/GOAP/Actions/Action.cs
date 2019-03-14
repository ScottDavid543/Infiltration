using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GOAP {
    // Unity specific Action class that incudes members required by most concrete Actions
    public abstract class Action: GOAPAction {
     

        protected StateDrivenBrain brain;
        public Transform destination;
        public GameObject interactObject;
        protected Vector3 moveRotation;
        protected Quaternion lookAtRotation;
        protected float angleToTurn;
        protected float angleOffset;
        public Action(string name, int cost, StateDrivenBrain brain, StateDrivenBrain.TacticalStates moveToState):base(name,cost,moveToState) {
            this.brain = brain;
            angleOffset = 0f;
        }
 
        protected bool FaceTarget(Vector3 target) {
            if (target != null) {

                TurnToFace(target);
                // Calculate rotation increment to face target waypoint
                moveRotation = Quaternion.Slerp(brain.gameObject.transform.rotation, lookAtRotation, brain.turnSpeed * Time.deltaTime).eulerAngles;
                // Rotate y axis only
                moveRotation.x = 0;
                moveRotation.z = 0;
                // Apply rotation
                angleToTurn = lookAtRotation.eulerAngles.y - brain.gameObject.transform.rotation.eulerAngles.y;
                // Move towards new target when close to facing it
               // Debug.Log("ANGLE: " + angleToTurn);
                if (Mathf.Abs(angleToTurn) < 1.0f) {
                    // Set animation parameters
                  //  brain.animator.SetFloat("Rotation", 0f);
                    return true;
                }
                else {
                    // Set animation parameters. 
                    if (angleToTurn < 0f) {
                       // brain.animator.SetFloat("Rotation", -brain.turnSpeed);
                    }
                    else {
                        //brain.animator.SetFloat("Rotation", brain.turnSpeed);
                    }
                }
            }
            else {
               // brain.animator.SetFloat("Rotation", 0f);
            }
            return false;
        }
        // Calculate Quarternion to face target 
        protected void TurnToFace(Vector3 target) {
            if (target != null) {
                // Initially calculate rotation as a vector so that x and z components can be set to zero
                //  Vector3 lookRotation = Quaternion.LookRotation(target - brain.gameObject.transform.position).eulerAngles;
                //  lookRotation.x = 0;
                //  lookRotation.z = 0;
                // WIP : This code should not always apply.
                // If using a weapon correct rotation to ensure weapon is pointing to player
                // if (brain.aiStateWeaponController.MountedWeapon) {

                //   lookRotation.y += brain.senses.forwardWeaponRotationCorrectionFactor;
                // }
                // All rotations should be applied as Quaternion so convert back
                //lookAtRotation = Quaternion.Euler(lookRotation);

                // brain.transform.rotation = Quaternion.LookRotation(lookRotation);
                
            }
        }
    }
}