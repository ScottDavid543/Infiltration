using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perception
{
    public class Stimulus
    {
        public enum StimulusTypes { AudioMovement, AudioCollision, AudioWeapon, AudioExplosion, VisualCanSee }
        public StimulusTypes type;
        public GameObject source;
        public Vector3 location;
        public Vector3 direction;
        public float radius;
        public GameObject secondarySource;


        public Stimulus(StimulusTypes type, GameObject source, Vector3 location, Vector3 direction, float radius, GameObject secondarySource)
        {
            this.type = type;
            this.source = source;
            this.location = location;
            this.direction = direction;
            this.radius = radius;
            this.secondarySource = secondarySource;
        }
        public Stimulus() { }
    }
}