using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP {
    public class Node
    {
        public WorldState ws;
        public int g;
        public int h;
        public int f;
        public Action action;
        public WorldState parentWS;
    }	
}
