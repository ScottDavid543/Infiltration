using UnityEngine;
using System.Collections;

namespace GOAP {
    public enum ActionStates { Success, Failed, Running }

    public abstract class GOAPAction {
        public WorldState preConditions;
        public WorldState effects;
        protected string name;
        public string Name { get { return name; } }
        protected int cost;
        public int Cost { get { return cost; } }
        protected StateDrivenBrain.TacticalStates moveToState;
        public StateDrivenBrain.TacticalStates MoveToState { get { return moveToState; } }
        public GOAPAction(string name, int cost, StateDrivenBrain.TacticalStates moveToState) {
            this.name = name;
            this.moveToState = moveToState;
            this.cost = cost;
            preConditions = new WorldState();
            effects = new WorldState();
        }
        public void SetPreCondition(Atom a, bool value) {
            preConditions.SetValue(a, value);
        }
        public void SetEffect(Atom a, bool value) {
            effects.SetValue(a, value);
        }
        public abstract ActionStates Initialise();
        public virtual ActionStates Update() { return ActionStates.Running; }
        public abstract void CleanUp();

    }
}
