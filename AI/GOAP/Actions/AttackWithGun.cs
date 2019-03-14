using UnityEngine;
using System.Collections;

namespace GOAP {
    public class AttackWithGun : Action
    {
        public float delayReset;
        public float fireDelay;
        public AttackWithGun(string name, int cost, StateDrivenBrain brain, StateDrivenBrain.TacticalStates moveToState) : base(name, cost, brain, moveToState)
        {
            angleOffset = -20f;
        }
        public override ActionStates Initialise()
        {
            Debug.Log("Start Action : Attack With Gun");
            brain.alertImage.SetActive(false);
            brain.chaseImage.SetActive(true);
            // Get a reference to the player and store it within Senses so the Weapons System can access it
            //brain.target = GameObject.FindGameObjectWithTag(Tags.FriendlyAI);
            //brain.animator.applyRootMotion = true;
            //brain.animator.SetLayerWeight(1, 1f);
            // Was the player found
            delayReset = 0.5f;
            fireDelay = delayReset;
            if (brain.target != null)
            {
                brain.animator.SetBool("Idle", false);
                brain.animator.SetBool("Run", true);
                return ActionStates.Running;
            }
            else
            {
                return ActionStates.Failed;
            }
        }
        public override ActionStates Update()
        {
            Vector3 direction;                        
            if (brain.target != null)
            {
                Debug.Log("setting destination");
            }
            brain.navmeshAgent.destination = brain.searchPos.transform.position;
                               
            if (brain.target != null) // If target exists
            {
                if (CanSeeCharacter(brain.transform, brain.target.transform, out direction)) // if targets visible
                {   // search position is the targets position and chase target
                    brain.searchPos.position = brain.target.transform.position;
                    brain.animator.SetBool("Run", false);
                    brain.animator.SetBool("Attack", true);
                    brain.navmeshAgent.isStopped = true;
                    int speed = 10;
                    fireDelay -= Time.deltaTime;
                    if (fireDelay <= 0)
                    {
                        brain.aiGunLogic.Fire();
                        fireDelay = delayReset;
                    }
                    Vector3 targetDir = brain.target.transform.position - brain.transform.position;
                    // The step size is equal to speed times frame time.
                    float step = speed * Time.deltaTime;
                    Vector3 newDir = Vector3.RotateTowards(brain.transform.forward, targetDir, step, 0.0f);
                    Debug.DrawRay(brain.transform.position, newDir, Color.red);
                    // Move our position a step closer to the target.
                    brain.transform.rotation = Quaternion.LookRotation(newDir);
                    //  Stop firing if the Player is dead
                    if (brain.target.GetComponent<Health>().isDead)
                    {
                        Debug.Log("Player dead");
                        // Action completed
                        return ActionStates.Success;
                    }
                }
                else
                {
                    if (Vector3.Distance(brain.transform.position, brain.searchPos.position) < 1f)
                    {
                        brain.animator.SetBool("Run", false);
                    }
                    else
                    {
                        brain.animator.SetBool("Run", true);
                        brain.animator.SetBool("Attack", false);
                        brain.navmeshAgent.isStopped = false;
                    }
                }
            }
            else
            {
                return ActionStates.Failed;
            }
            // Action still running
            return ActionStates.Running;
        }
        public override void CleanUp()
        {
            Debug.Log("End Action : Attack With Gun");
            brain.animator.SetBool("Run", false);
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
                    Debug.DrawRay(registrantEyePosition, rayDirection * senses.sightRange, Color.magenta);
                    if (hit)
                    {
                        // Ignore cover
                        if (hitData.collider.tag == Tags.Player)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}