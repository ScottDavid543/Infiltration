using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GOAP
{
    public enum Atom { PlayerDead,CanSeePlayer,InRange,KnowledgeOfPlayer,OnGuard}

    public class WSType
    {
        public bool value;
        public bool enabled;
        public WSType()
        {
            value = false;
            enabled = false;
        }
    }
    public class WorldState 
    {
        private Dictionary<Atom, WSType> state;

        public WorldState()
        {
            state = new Dictionary<Atom, WSType>();
            foreach(Atom a in Enum.GetValues(typeof(Atom)))
            {
                state.Add(a, new WSType());
            }
        }
        public bool GetValue(Atom a)
        {
            return state[a].value;
        }

        public void SetValue(Atom a, bool value)
        {
            state[a].value = value;
            state[a].enabled = true;
        }

        public bool AreEqual(WorldState ws)
        {
            foreach(Atom a in Enum.GetValues(typeof(Atom)))
            {
                if(state[a].value != ws.state[a].value)
                {
                    return false;
                }
            }
            return true;
        }

        private bool AreEnabledEqual(WorldState ws)
        {
            foreach(Atom a in Enum.GetValues(typeof(Atom))){
                if (ws.state[a].enabled)
                {
                    if(state[a].value != ws.state[a].value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool GoalAchieved(WorldState goal)
        {
            return AreEnabledEqual(goal);
        }
        public bool PreConditionsMeet(WorldState preconditions)
        {
            return AreEnabledEqual(preconditions);
        }

        public WorldState GetWSWithEffectsApplied(WorldState effects)
        {
            WorldState ws = new WorldState();
            // Create a copy
            foreach(Atom a in Enum.GetValues(typeof(Atom)))
            {
                ws.state[a].value = state[a].value;
                ws.state[a].enabled = state[a].enabled;
            }
            // Apply effects to copy
            foreach(Atom a in Enum.GetValues(typeof(Atom)))
            {
                if (effects.state[a].value)
                {
                    ws.state[a].value = true;
                    ws.state[a].enabled = true;
                }
            }
            return ws;
        }

        public int GetNumberMismatchingAtoms(WorldState to)
        {
            int mismatch = 0;
            foreach(Atom a in Enum.GetValues(typeof(Atom)))
            {
                if (to.state[a].enabled)
                {
                    if (state[a].value != to.state[a].value) mismatch++;
                }
            }
            return mismatch;
        }

        public void ApplyEffects(WorldState effects)
        {
            foreach(Atom a in Enum.GetValues(typeof(Atom)))
            {
                if (effects.state[a].value)
                {
                    state[a].value = true;
                }
            }
        }

        public void Display(string msg)
        {
            Console.WriteLine(msg);
            foreach (KeyValuePair<Atom, WSType> a in state)
            {
                Debug.Log(" Key " + a.Key.ToString() + ": " + a.Value.value);
                Console.WriteLine("Key :{0,15} ", a.Key.ToString());
                Console.WriteLine(" :{0}  ", a.Value.value);
            }
        }
    }
}
