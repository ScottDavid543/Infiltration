using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;


 public class FSM<T>{
     // The states within the state machine
	private List<State<T>> states;
     // It is possible to pause the state machine
	private bool pause = false;
     // The current state
    public State<T> currentState;
    public bool displayTransitionMessages = false;
    public State<T> CurrentState {
        get { return currentState; }
    }
    public bool Pause{
        get { return pause; }
        set { pause = value; }
    }

	public FSM(bool displayTransitionMessages = false){
        this.displayTransitionMessages = displayTransitionMessages;
        states = new List<State<T>>();
	}
	
    // Add a state to the machine
	public void AddState(State<T> state){
        state.displayCallBackMessages = displayTransitionMessages;
        int index = -1;
        // Duplicate states are not permitted
        index = states.FindIndex(delegate(State<T> s) { return s.GetName() == state.ToString(); });
        if (index == -1) states.Add(state);
        else Debug.LogError("State " + state.ToString() + " already exists within FSM");
    }

    public void SetInitialState(T initialState) {
        currentState = states.Find(delegate(State<T> s) { return s.GetName() == initialState.ToString(); });
        // Ensure the OnEnter callback is invoked for the current state when the system starts
        currentState.OnEnter();
    }
	
     // Add a transition by passing the two enum types types 
	public void AddTransition(T fromState, T toState){
        // First make sure the two states exist within the state list
        int indexFrom =  states.FindIndex(delegate(State<T> s) { return s.GetName() == fromState.ToString(); });
        int indexTo = states.FindIndex(delegate(State<T> s) { return s.GetName() == toState.ToString(); });
        if (indexFrom >= 0 && indexTo >= 0)
			states[indexFrom].AddTransition(fromState,toState);
		else Debug.LogError("One or more states do not exist within FSM " + fromState.ToString() + " to : " + toState.ToString());
	}

    public void Check(){
		if (!pause){
			bool changed;
            // Monitor should contain functionality that needs to be executed periodically on the current state
            currentState.Monitor();
            // Check the guards on the current state to determine if a transition should occur
            T toState = currentState.CheckGuards(out changed);
            // If a transition has occurred
			if (changed){
                // Invoke the callback on the old state
                currentState.OnLeave();
                // Locate the index possition of the new state within the list
                int indexTo = states.FindIndex(delegate(State<T> s) { return s.GetName() == toState.ToString(); });
                // The new state becomes the current state
                currentState = states[indexTo];   
                // Invoke its callback
                currentState.OnEnter();
			}
		}
	}
	
	public void Dump(){
		Debug.Log("**** States ****");
		foreach(State<T> s in states)
			Debug.Log("State : " + s.GetName());
	}
}
