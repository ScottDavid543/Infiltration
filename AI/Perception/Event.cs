using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Perception
{
    public class Event : Stimulus
    {
        public float eventPerceptionValue;
        public Configuration config;
        public float life;
        public float birth;
        public bool markEventForDeletion;

        public Event(Stimulus s, Configuration config)
        {
            this.type = s.type;
            this.source = s.source;
            this.location = s.location;
            this.direction = s.direction;
            this.radius = s.radius;
            this.config = config;
            this.birth = Time.time;
            this.life = 0f;
            this.markEventForDeletion = false;
        }
        
        public void Process()
        {
           // Debug.Log(eventPerceptionValue);
            life = Time.time - birth;
            eventPerceptionValue = config.GetPerception(life);
            if (eventPerceptionValue < 0f)
            {
                markEventForDeletion = true;
            }
        }

        // Stimulation has been resent so update the event life
        public void Update()
        {
            // Don't change life if in attack or decay phase
            if (life > config.Decay)
            {
                // Reset the life to the start of the sustain period.
                birth = Time.time - config.Decay;
            }
        }

    }
}
