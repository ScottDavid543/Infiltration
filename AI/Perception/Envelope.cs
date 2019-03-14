using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Perception
{
    public class Envelope
    {
        private List<Event> events;
        public GameObject source;
        public float perceptionValue;
        public bool markEnvelopeForDeletion = false;

        public Envelope(Stimulus stimulus, Configuration config)
        {
            source = stimulus.source;
            events = new List<Event>();
            markEnvelopeForDeletion = false;
            Add(stimulus, config);
        }
        public void Add(Stimulus stimulus, Configuration config)
        {
            events.Add(new Event(stimulus, config));
           // Debug.Log("Event added");
           // Debug.Log(stimulus.type);
            Process();
        }
        public bool Exists(Stimulus stimulus)
        {
            return events.Exists(e => e.source == stimulus.source && e.type == stimulus.type);
        }
        public void Process()
        {
            perceptionValue = 0f;
            foreach (Event e in events)
            {
                // Update event's perception value
                e.Process();
                // Update evelope's perception value
                if (!e.markEventForDeletion)
                {
                    perceptionValue += e.eventPerceptionValue;
                }
            }
            // Remove all events marked for deletion
            events.RemoveAll(e => e.markEventForDeletion == true);
            // If there are no events left mark the envelop for deletion
            if (events.Count == 0)
            {
                markEnvelopeForDeletion = true;
            }
        }
        public void Update(Stimulus stimulus)
        {
            Event foundEvent = events.Find(e => e.source == stimulus.source && e.type == stimulus.type);
            if (foundEvent != null)
            {
                foundEvent.Update();
                Process();
            }
        }
        public bool CanSeeTarget()
        {
            foreach (Event e in events)
            {
               // Debug.Log(events);
                //Debug.Log(e.type);
                if (e.type == Stimulus.StimulusTypes.VisualCanSee)
                {
                    //Debug.Log("Target Spotted");
                    return true;
                }
            }
            return false;
        }

    }
}
