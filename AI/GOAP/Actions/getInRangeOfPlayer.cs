using UnityEngine;
using System.Collections;
using Perception;
namespace GOAP
{
    public class getInRangeOfPlayer : Action
    {
        PerceptionManager perceptionManager;
        Vector3 direction;
        public getInRangeOfPlayer(string name, int cost, StateDrivenBrain brain, StateDrivenBrain.TacticalStates moveToState) : base(name, cost, brain, moveToState)
        {
            angleOffset = -20f;
             perceptionManager = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<PerceptionManager>();
        }
        public override ActionStates Initialise()
        {
            brain.searchPos.position = destination.position;            
            Debug.Log("Start Action : Get In Range of Player");
            brain.alertImage.SetActive(true);
            brain.chaseImage.SetActive(false);
            // Get a reference to the player and store it within Senses so the Weapons System can access it
            //brain.target = GameObject.FindGameObjectWithTag(Tags.Player);
            // Was the player found
            if (brain.target != null)
            {
                brain.animator.SetBool("Walk", true);
                brain.animator.SetBool("Idle", false);
                return ActionStates.Running;
            }
            else
            {
                return ActionStates.Failed;
            }
        }
        public override ActionStates Update()
        {
            // Turn to face the player
            // Debug.Log("Getting In Range");     
            brain.navmeshAgent.destination = brain.searchPos.transform.position;            
            if (Vector3.Distance(brain.transform.position, brain.searchPos.position) < 20f)
            {
               // Debug.Log(Vector3.Distance(brain.transform.position, brain.searchPos.position));
                RaycastHit objectHit;
                Vector3 fwd = brain.transform.TransformDirection(Vector3.forward);
                Debug.DrawRay(brain.transform.position, fwd * 50, Color.red);
                if (brain.target != null)
                {
                    if (CanSeeCharacter(brain.transform, brain.target.transform, out direction))
                    {
                        //do something if hit object ie
                        // if (objectHit.transform.tag == Tags.FriendlyAI)
                        // {
                        // Debug.Log("Facing target");
                        if (Physics.Raycast(brain.transform.position, fwd, out objectHit, 50))
                        {
                            return ActionStates.Success;
                        }                       
                        // }
                        //else
                        // {
                        // if (Vector3.Distance(brain.transform.position, destination.position) < 0.5f)
                        //  {
                        //      return ActionStates.Success;
                        //  }
                        // return ActionStates.Running;
                        // }
                    }
                    else
                    {
                        if (Vector3.Distance(brain.transform.position, brain.searchPos.position) < 1.5f)
                        {
                            brain.animator.SetBool("Walk", false);
                        }
                        else
                        {
                            brain.animator.SetBool("Walk", true);
                        }
                        return ActionStates.Running;
                    }
                }
                else
                {
                    return ActionStates.Failed;
                }
            }
            //Debug.Log(Vector3.Distance(brain.transform.position, brain.searchPos.position));
            if (Vector3.Distance(brain.transform.position, brain.searchPos.position) < 1f)
            {
                return ActionStates.Success;
            }
            else
            {
                return ActionStates.Running;
            }
        }
        public override void CleanUp()
        {
            Debug.Log("End Action : Get in Range of Player");
            brain.animator.SetBool("Walk", false);
            //brain.animator.SetLayerWeight(1, 0f);
        }
        private static bool CanSeeCharacter(Transform registrant, Transform character, out Vector3 direction)
        {
            float registrantHeight = registrant.GetComponent<CharacterController>().height;
            Senses senses = registrant.GetComponent<Senses>();
            direction = character.position - registrant.position;
            // Get distance to character
            float distanceToTarget = Vector3.Distance(registrant.position, character.position);
            // Check in visible range
            if (senses.sightRange > distanceToTarget)
            {              
                // Angle between character and registrant
                float angle = Vector3.Angle(direction, registrant.forward);
                // Convert to positive value
                angle = System.Math.Abs(angle);
                // Is the target within the viewing angle. Ignores obstacles between target and shooter
                if (angle < (senses.viewingAngle / 2))
                {
                    RaycastHit hitData;
                    // Create a layer mask for the ray. Look for players only (player should be configured to layer 8).
                    LayerMask playerMask = 1 << 8;
                    // Player may be obscurred by cover so ensure ray picks up cover too
                    LayerMask coverMask = 1 << 9;
                    // AI 
                    LayerMask aiMask = 1 << 10;
                    LayerMask mask = coverMask | playerMask | aiMask;
                    // Aim for the upper part of the body. A rather grued check that could be fixed later
                    //float targetHeight = (character.GetComponent<CharacterController>().height);
                    Vector3 registrantEyePosition = new Vector3(registrant.position.x, registrant.position.y + registrantHeight, registrant.position.z);
                    Vector3 targetBodyPosition = new Vector3(character.transform.position.x, character.transform.position.y, character.transform.position.z);
                    Vector3 rayDirection = (targetBodyPosition - registrantEyePosition).normalized;
                    bool hit = Physics.Raycast(registrantEyePosition, rayDirection, out hitData, senses.sightRange, mask.value);
                    Debug.DrawRay(registrantEyePosition, rayDirection * senses.sightRange, Color.yellow);
                    if (hit)
                    {
                        // Ignore cover
                        if (hitData.collider.tag == Tags.Player)
                        {
                             //Debug.Log("HIT THE AI");
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}
