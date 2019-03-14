using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP {
    public class Goal
    {
        public WorldState condition;
        public int priority;
        public Goal(int priority)
        {
            this.priority = priority;
            condition = new WorldState();
        }
    }
}
