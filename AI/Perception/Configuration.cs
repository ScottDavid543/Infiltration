using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Perception
{
    public class Configuration
    {
        public Stimulus.StimulusTypes type;
        public float peak;
        public float attack;
        public float decay;
        public float sustainPercentage;
        public float sustain;
        public float release;
        public float modifier;
        public float aggressive;
        public float threatening;
        public float interesting;
        public Stimulus.StimulusTypes Type { get { return type; } }
        public float Decay { get { return decay; } }

        public Configuration(Stimulus.StimulusTypes type, float peak, float attack, float decay, float sustainPercentage, float sustain, float release, float modifier, float aggressive, float threatening, float interesting)
        {
            this.type = type;
            this.peak = peak;
            this.attack = attack;
            this.decay = decay;
            this.sustainPercentage = sustainPercentage;
            this.sustain = sustain;
            this.release = release;
            this.modifier = modifier;
            this.aggressive = aggressive;
            this.threatening = threatening;
            this.interesting = interesting;
        }
        public float GetPerception(float life)
        {
            if (life < attack)
            {
                return GetGraphY(life, 0f, attack, 0f, peak);
            }
            else if (life < decay)
            {
                return GetGraphY(life, attack, decay, peak, (peak * sustainPercentage));
            }
            else if (life < sustain)
            {
                return (peak * sustainPercentage);
            }
            else if (life < release)
            {
                return GetGraphY(life, sustain, release, (peak * sustainPercentage), 0f);
            }
            else
            {
                return -1f;
            }
        }
        private float GetGraphY(float x, float x1, float x2, float y1, float y2)
        {
            float m = (y2 - y1) / (x2 - x1);
            return m * (x - x1) + y1;
        }
        public static List<Configuration> GetConfigurations()
        {
            List<Configuration> configs = new List<Configuration>();
            configs.Add(new Configuration(Stimulus.StimulusTypes.VisualCanSee, 30f, 1.5f, 2.5f, 0.9f, 7f, 10f, 1f, 1f, 0.625f, 0.25f));
            configs.Add(new Configuration(Stimulus.StimulusTypes.AudioWeapon, 10f, 1.5f, 2.5f, 0.9f, 7f, 10f, 1f, 1f, 0.625f, 0.25f));   
            configs.Add(new Configuration(Stimulus.StimulusTypes.AudioMovement, 5f, 1.5f, 2.5f, 0.9f, 7f, 10f, 1f, 1f, 0.625f, 0.25f));
            configs.Add(new Configuration(Stimulus.StimulusTypes.AudioCollision, 5f, 1.5f, 2.5f, 0.9f, 7f, 10f, 1f, 1f, 0.625f, 0.25f));
            return configs;
        }


    }
}
