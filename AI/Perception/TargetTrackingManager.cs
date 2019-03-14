using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perception
{
    public class TargetTrackingManager : MonoBehaviour
    {
        public enum TResponses { DoNothing, SeekCover, EngageEnemy };
        private const float PROCESS_TIME = 1f;
        [HideInInspector]
        public PerceptionManager perceptionManager;

        public const int NEUTRAL = 1;
        public const int FRIENDLY = 2;
        public const int HOSTILE = 4;
        public enum AgentTypes { Neutral, Friendly, Hostile };
        public AgentTypes agentType = AgentTypes.Hostile;
        //[HideInInspector]
        public int agentFilter;

        private List<Envelope> envelopes;

        [HideInInspector]
        public TResponses response;
       // [HideInInspector]
        public GameObject target;
        public StateDrivenBrain brain;

        void Awake()
        {
            perceptionManager = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<PerceptionManager>();
            agentFilter = HOSTILE;
            envelopes = new List<Envelope>();
            response = TResponses.DoNothing;
            target = null;
            brain = this.gameObject.GetComponent<StateDrivenBrain>();
        }

        // Use this for initialization
        void Start()
        {
            perceptionManager.Register(this.gameObject);
            StartCoroutine(Process());
        }

        // Update is called once per frame
        void Update()
        {
           // for(int i = 0; i < perceptionManager.registrants.Count; i++)
            //{
              //  Debug.Log(i, perceptionManager.registrants[i]);
          //  }
           // GetResponse();
            Debug.Log(GetResponse());
        }
        public void AcceptFilteredStimulus(Stimulus stimulus)
        {
             Debug.Log("Type : " + stimulus.type + " Source : " + stimulus.source);
            foreach (Envelope e in envelopes)
            {
                // Has an envelope been created for this source
                if (e.source == stimulus.source)
                {
                    // This is an update stimulus
                    if (e.Exists(stimulus))
                    {
                        // Update the time 
                        if(stimulus.type == Stimulus.StimulusTypes.AudioWeapon || stimulus.type == Stimulus.StimulusTypes.AudioMovement|| stimulus.type == Stimulus.StimulusTypes.AudioCollision)
                        {
                            brain.searchPos.transform.position = e.source.transform.position;
                            e.Update(stimulus);
                        }
                        e.Update(stimulus);
                        return;
                    }
                    else
                    {
                        // A new stimulus for an existing envelope
                        e.Add(stimulus, GetConfiguration(stimulus));
                        return;
                    }
                }
            }
            // Create a new envelope and event
            envelopes.Add(new Envelope(stimulus, GetConfiguration(stimulus)));
        }

        // Return the configuration for a stimulus
        private Configuration GetConfiguration(Stimulus stimulus)
        {
            foreach (Configuration c in perceptionManager.configs)
            {
                if (c.Type == stimulus.type)
                {
                    return c;
                }
            }
            return null;
        }

        private IEnumerator Process()
        {
            yield return new WaitForSeconds(PROCESS_TIME);
            foreach (Envelope e in envelopes)
            {
                e.Process();
            }
            envelopes.RemoveAll(e => e.markEnvelopeForDeletion == true);
            StartCoroutine(Process());
        }


        public TResponses GetResponse()
        {
            // return TResponses.DoNothing;
            float highestPerceptionValue = 0f;
            Envelope temp = null;
            if (envelopes.Count == 0)
            {
                target = null;
                return TResponses.DoNothing;
            }
            foreach (Envelope e in envelopes)
            {
               // Debug.Log(e.perceptionValue);
                if (e.perceptionValue >= highestPerceptionValue)
                {
                   // Debug.Log(e.source);
                    temp = e;
                    highestPerceptionValue = e.perceptionValue;
                }
            }
            // Applies to the ai 
            // Debug.Log(temp.source);
            //Debug.Log(temp.source);
            if (temp.source.tag == Tags.Player || temp.source.tag == "Throwable") 
            {
                target = temp.source;
                // Debug.Log(temp.source);
                // Debug.Log("TRYING TO GET RESPONSE");
                if (temp.CanSeeTarget())
                {
                    return TResponses.EngageEnemy;
                }
                else
                {
                    return TResponses.SeekCover;
                }
            }
            else return TResponses.DoNothing;
        }
    }
}
