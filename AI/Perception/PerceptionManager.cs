using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perception
{
    public class PerceptionManager : MonoBehaviour
    {
        public List<GameObject> registrants;
        private List<Stimulus> stimuliBuffer;
        private List<GameObject> characters;
        [HideInInspector]
        public List<Configuration> configs;

        private void Start()
        {
            StartCoroutine(Process());
        }
        void Awake()
        {
            registrants = new List<GameObject>();
            stimuliBuffer = new List<Stimulus>();
            characters = GetAllCharacters();
            configs = Configuration.GetConfigurations();
        }
        private void Update()
        {
            // for(int i = 0; i<characters.Count;i++)
            // Debug.Log(characters[i]);
            //ebug.Log(stimuliBuffer[0].);
            for(int i = 0; i<stimuliBuffer.Count; i++)
            {
                Debug.Log(stimuliBuffer[i].type);
            }
        }
        private List<GameObject> GetAllCharacters()
        {
           // GameObject[] tempFriendly = GameObject.FindGameObjectsWithTag(Tags.FriendlyAI);
            GameObject[] tempAI = GameObject.FindGameObjectsWithTag(Tags.AI);
            GameObject[] tempPlayer = GameObject.FindGameObjectsWithTag(Tags.Player);
            List<GameObject> characters = new List<GameObject>(tempPlayer);
            characters.AddRange(tempPlayer);
           // characters.Add(tempPlayer);
            return characters;

        }
        public void Register(GameObject registry)
        {
            registrants.Add(registry);
        }
        public void AcceptStimulus(Stimulus stimulus)
        {
            stimuliBuffer.Add(stimulus);
        }
        IEnumerator Process()
        {
            yield return new WaitForSeconds(1.0f);
            GenerateVisualStimulations();
            ProcessStimulationBuffer();
            StartCoroutine(Process());
        }
        private void ProcessStimulationBuffer()
        {
            TargetTrackingManager ttm;
            foreach (GameObject r in registrants)
            {
                //Debug.Log(r);
                ttm = r.GetComponent<TargetTrackingManager>();
                foreach (Stimulus s in stimuliBuffer)
                {
                  //  Debug.Log(s.type, s.source);
                    if (Filter(ttm, r, s))
                    {
                        if (s.type == Stimulus.StimulusTypes.AudioWeapon || s.type == Stimulus.StimulusTypes.AudioMovement || s.type == Stimulus.StimulusTypes.AudioCollision)
                        {
                            float range = Mathf.Abs((s.location - r.transform.position).magnitude);
                            if (range < s.radius)
                            {
                                ttm.AcceptFilteredStimulus(s);
                            }
                        }
                        else if (s.type == Stimulus.StimulusTypes.VisualCanSee && s.secondarySource == r)
                        {
                          //  Debug.Log(s.source);
                           // Debug.Log(s.type);
                            ttm.AcceptFilteredStimulus(s);
                        }
                    }
                }
            }
            stimuliBuffer.Clear();
        }

        private void GenerateVisualStimulations()
        {
            Vector3 direction;
            foreach (GameObject r in registrants)
            {
                foreach (GameObject c in characters)
                {
                    if (CanSeeCharacter(r.transform, c.transform, out direction) && r != c)
                    {
                        stimuliBuffer.Add(new Stimulus(Stimulus.StimulusTypes.VisualCanSee, c, c.transform.position, direction, 0f, r));
                    }
                }
            }
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
                    float targetHeight = (character.GetComponent<CharacterController>().height);
                    Vector3 registrantEyePosition = new Vector3(registrant.position.x, registrant.position.y + registrantHeight, registrant.position.z);
                    Vector3 targetBodyPosition = new Vector3(character.transform.position.x, character.transform.position.y , character.transform.position.z);
                    Vector3 rayDirection = (targetBodyPosition - registrantEyePosition).normalized;
                    bool hit = Physics.Raycast(registrantEyePosition, rayDirection, out hitData, senses.sightRange, mask.value);
                    Debug.DrawRay(registrantEyePosition, rayDirection * senses.sightRange, Color.cyan);
                    if (hit)
                    {
                        // Ignore cover
                        if ( hitData.collider.tag == Tags.Player)
                        {
                           // Debug.Log(hitData.collider.gameObject.name);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

          private bool Filter(TargetTrackingManager tsm, GameObject registrant, Stimulus stimulus)
          {
              // Don't send an agent's own stimuli to itself
              if (stimulus.source != registrant)
              {
                  TargetTrackingManager.AgentTypes destinationAgentType = tsm.agentType;
                  // Who is the agent interested in
                  int validTypes = tsm.agentFilter;

                  TargetTrackingManager.AgentTypes sourceAgentType;
                  if (stimulus.source.GetComponent<TargetTrackingManager>() == null)
                  {
                      sourceAgentType = TargetTrackingManager.AgentTypes.Friendly;
                  }
                  else
                  {
                      sourceAgentType = stimulus.source.GetComponent<TargetTrackingManager>().agentType;
                  }

                  if ((sourceAgentType == TargetTrackingManager.AgentTypes.Hostile) && (destinationAgentType == TargetTrackingManager.AgentTypes.Friendly) && ((validTypes & TargetTrackingManager.HOSTILE) != 0)) return true;
                  if ((sourceAgentType == TargetTrackingManager.AgentTypes.Hostile) && (destinationAgentType == TargetTrackingManager.AgentTypes.Hostile) && ((validTypes & TargetTrackingManager.FRIENDLY) != 0)) return true;
                  if ((sourceAgentType == TargetTrackingManager.AgentTypes.Hostile) && (destinationAgentType == TargetTrackingManager.AgentTypes.Neutral) && ((validTypes & TargetTrackingManager.NEUTRAL) != 0)) return true;

                  if ((sourceAgentType == TargetTrackingManager.AgentTypes.Friendly) && (destinationAgentType == TargetTrackingManager.AgentTypes.Friendly) && ((validTypes & TargetTrackingManager.FRIENDLY) != 0)) return true;
                  if ((sourceAgentType == TargetTrackingManager.AgentTypes.Friendly) && (destinationAgentType == TargetTrackingManager.AgentTypes.Hostile) && ((validTypes & TargetTrackingManager.HOSTILE) != 0)) return true;
                  if ((sourceAgentType == TargetTrackingManager.AgentTypes.Friendly) && (destinationAgentType == TargetTrackingManager.AgentTypes.Neutral) && ((validTypes & TargetTrackingManager.NEUTRAL) != 0)) return true;

                  if ((sourceAgentType == TargetTrackingManager.AgentTypes.Neutral) && (destinationAgentType == TargetTrackingManager.AgentTypes.Friendly) && ((validTypes & TargetTrackingManager.NEUTRAL) != 0)) return true;
                  if ((sourceAgentType == TargetTrackingManager.AgentTypes.Neutral) && (destinationAgentType == TargetTrackingManager.AgentTypes.Hostile) && ((validTypes & TargetTrackingManager.NEUTRAL) != 0)) return true;
                  if ((sourceAgentType == TargetTrackingManager.AgentTypes.Neutral) && (destinationAgentType == TargetTrackingManager.AgentTypes.Neutral) && ((validTypes & TargetTrackingManager.NEUTRAL) != 0)) return true;
                  return false;
              }
              else
              {
                  return false;
              }
        }

    }
}
