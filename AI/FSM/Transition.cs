using UnityEngine;
using System.Collections;
using System.Reflection;
using System;


// Records a transition from one state to another
public class Transition<T> {
    private State<T> state;
	public T toState,fromState;
	public MethodBase guard = null;
	MonoBehaviour guardScript;
    object[] stateParameter;
	public Transition(T fromState,T toState, MonoBehaviour guardScript, State<T> state){
        this.state = state;
		this.toState = toState;
		this.fromState = fromState;
		this.guardScript = guardScript;
		Assembly assembly = Assembly.GetExecutingAssembly();
		Type guardScriptType = assembly.GetType(guardScript.GetType().ToString());
		guard = guardScriptType.GetMethod("Guard" + fromState.ToString() + "To" + toState.ToString());
        stateParameter = new object[] { state };
	}
	
	public bool InvokeGuard(){
		if (guard == null) {
			Debug.LogError("Guard missing : Guard" + fromState.ToString()+"To"+toState.ToString());
			return false;
		}
		else return (bool)guard.Invoke(guardScript,stateParameter);
	}
}
