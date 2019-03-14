using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GOAP;
using Perception;
using UnityEngine.UI;

public class StateDrivenBrain:MonoBehaviour {
    private int healthLevel;
    protected float thinkInterval = 0.25f;
    public CharacterController characterController;
    public float turnSpeed = 1;
    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent navmeshAgent;
    public enum TacticalStates { Goto, Animate, UseSmartObject };
    [HideInInspector]
    public FSM<TacticalStates> tacticalStateMachine;
    // Toggle on/off tactical state behaviour. 
    public bool tacticalStateActive = true;

    public bool displayFSMTransitions = false;
    // A list of all actions
    private List<Action> actions;
    // The current action being processed by the current state
    [HideInInspector]
    public Action currentAction;

    [HideInInspector]
    // Contains the actions required to complete the current goal
    public Stack<Action> plan;
    public Transform hand;

    [HideInInspector]
    public WorldState startWS;

    [HideInInspector]
    public Goal currentGoal;

    private List<Goal> goals;

    public GameObject target;

    public TargetTrackingManager ttm;

    public Senses senses;
    public Transform searchPos;
    public Animator animator;
    public GameObject AIGun;
    public AIGunLogic aiGunLogic;
    public Health health;

    public Transform[] patrolPoints;
    public int destinationPoint = 0;
    ImageBehaviour imageBehaviour;
    public GameObject alertImage;
    public GameObject chaseImage;

    protected void Awake() {
        // Build the Finite State Machine
        characterController = this.GetComponent<CharacterController>();
        //target = GameObject.FindGameObjectWithTag(Tags.FriendlyAI);
        ttm = this.gameObject.GetComponent<TargetTrackingManager>();
        senses = this.gameObject.GetComponent<Senses>();
        animator = this.GetComponent<Animator>();
        aiGunLogic = AIGun.GetComponent<AIGunLogic>();
        health = this.GetComponent<Health>();
        imageBehaviour = this.GetComponent<ImageBehaviour>();
        alertImage = imageBehaviour.alertImage.gameObject;
        chaseImage = imageBehaviour.chaseImage.gameObject;

        tacticalStateMachine = new FSM<TacticalStates>(displayFSMTransitions);
        tacticalStateMachine.AddState(new Goto<TacticalStates>(TacticalStates.Goto, this, 0f)); 
        tacticalStateMachine.AddState(new Animate<TacticalStates>(TacticalStates.Animate, this, 0f)); 
        tacticalStateMachine.AddState(new UseSmartObject<TacticalStates>(TacticalStates.UseSmartObject, this, 0f)); 

        tacticalStateMachine.AddTransition(TacticalStates.Goto, TacticalStates.Animate);
        tacticalStateMachine.AddTransition(TacticalStates.Goto, TacticalStates.UseSmartObject);
        tacticalStateMachine.AddTransition(TacticalStates.Goto, TacticalStates.Goto);
        tacticalStateMachine.AddTransition(TacticalStates.Animate, TacticalStates.Goto);
        tacticalStateMachine.AddTransition(TacticalStates.Animate, TacticalStates.UseSmartObject);
        tacticalStateMachine.AddTransition(TacticalStates.Animate, TacticalStates.Animate);
        tacticalStateMachine.AddTransition(TacticalStates.UseSmartObject, TacticalStates.Goto);
        tacticalStateMachine.AddTransition(TacticalStates.UseSmartObject, TacticalStates.Animate);
        tacticalStateMachine.AddTransition(TacticalStates.UseSmartObject, TacticalStates.UseSmartObject);

        actions = new List<Action>();


        Action idle = new Idle("idle", 1, this, TacticalStates.Goto);
        idle.SetEffect(Atom.OnGuard, true);
        idle.destination = patrolPoints[destinationPoint];
        actions.Add(idle);
    
        Action GetInRangeOfPlayer = new getInRangeOfPlayer("Get in range of the player", 1, this, TacticalStates.Goto);
        GetInRangeOfPlayer.SetPreCondition(Atom.KnowledgeOfPlayer, true);
        GetInRangeOfPlayer.SetEffect(Atom.CanSeePlayer, true);
        GetInRangeOfPlayer.SetEffect(Atom.InRange, true);
        GetInRangeOfPlayer.destination = GameObject.FindGameObjectWithTag(Tags.Player).transform;
        actions.Add(GetInRangeOfPlayer);

        Action attackWithGun = new AttackWithGun("Attack with gun", 1, this, TacticalStates.Goto);
        attackWithGun.SetPreCondition(Atom.InRange, true);
        attackWithGun.SetPreCondition(Atom.CanSeePlayer, true);
        attackWithGun.SetEffect(Atom.PlayerDead, true);
        attackWithGun.destination = searchPos.transform;
        actions.Add(attackWithGun);
        

        // set the current world state
        startWS = new WorldState();
        // what the ai knows about the enviornment
        startWS.SetValue(Atom.KnowledgeOfPlayer, false);
        startWS.SetValue(Atom.CanSeePlayer, false);


        goals = new List<Goal>();
        Goal idleGoal = new Goal(1);
        idleGoal.condition.SetValue(Atom.OnGuard, true);
        goals.Add(idleGoal);

        Goal combatGoal = new Goal(10);
        //combatGoal.condition.SetValue(Atom.HaveGun, true);
        //combatGoal.condition.SetValue(Atom.HaveAmmo, true);
        combatGoal.condition.SetValue(Atom.PlayerDead, true);
        goals.Add(combatGoal);

     //   Goal getInRangeGoal = new Goal(20);
       // getInRangeGoal.condition.SetValue(Atom.InRange, true);
       // goals.Add(getInRangeGoal);

        /*Goal findTargetGoal = new Goal(5);
        //combatGoal.condition.SetValue(Atom.HaveGun, true);
        //combatGoal.condition.SetValue(Atom.HaveAmmo, true);
        findTargetGoal.condition.SetValue(Atom.CanSeePlayer, true);
        goals.Add(findTargetGoal);*/
    }
    private Goal GetGoal()
    {
        Goal temp = null;
        foreach(Goal g in goals)
        {
            if(temp == null)
            {
                temp = g;
            }
            else
            {
                if (temp.priority < g.priority)
                {
                    temp = g;
                }
            }
        }
        return temp;
    }
    public void GenerateAStarPlan()
    {
        if (health.isDead != true)
        {
            Debug.Log("Generating plan");
            AStar aStar = new AStar(actions);
            // Generate the plan that meets the highest priority goal
            currentGoal = GetGoal();
            plan = aStar.GetPlan(startWS, currentGoal);
            // goal can not be achieved
            if (plan == null)
            {
                Debug.Log("Assigning default goal");
                // the default goal being the first in the list
                // rather crude solution
                currentGoal = goals[0];
                plan = aStar.GetPlan(startWS, currentGoal);
                // display the plan
                foreach (Action a in plan)
                {
                    Debug.Log("Action : " + a.Name);
                }
                //no need to pop the action off the plan as this will be performed by the current states onleave call back
                //return;
            }
            // display the plan
            foreach (Action a in plan)
            {
                Debug.Log("Action : " + a.Name);
            }
            // get the first action
            currentAction = plan.Pop();
            // manually set the state for the first action
            tacticalStateMachine.SetInitialState(currentAction.MoveToState);
        }
    }

    private void CheckPlan()
    {
        // dont generate a plan until the state has finished
        if (tacticalStateMachine.CurrentState.StateFinished)
        {
            // the plan has completed
            if (plan.Count == 0)
            {
                //tacticalStateMachine.Check();
                // set it to low priority to ensure higher priority goals are meet first
                //this is an aread that requires additional work
                currentGoal.priority = 0;
                // generate a new plan
                GenerateAStarPlan();
            }
            // the last action failed
            if (tacticalStateMachine.CurrentState.ActionStatus == ActionStates.Failed)
            {
                GenerateAStarPlan();
            }
        }
    }

    public void Start() {
        navmeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // generate the plan
        GenerateAStarPlan();
    }

    public void Update() {
        if (tacticalStateActive & plan != null && tacticalStateMachine.CurrentState != null) {
            tacticalStateMachine.CurrentState.Act();
            CheckPlan();
            tacticalStateMachine.Check();
        }        
        if(ttm.GetResponse() == TargetTrackingManager.TResponses.SeekCover)
        {
           // Debug.Log("SHOULD SET KNOWLEDGE TO TRUE");     
            startWS.SetValue(Atom.KnowledgeOfPlayer,true );
            if (currentGoal == goals[0])
            {
                GenerateAStarPlan();
            }
        }
        //Debug.Log(Atom.KnowledgeOfPlayer = true);
        if (ttm.GetResponse() == TargetTrackingManager.TResponses.EngageEnemy)
        {
            startWS.SetValue(Atom.CanSeePlayer, true);
            startWS.SetValue(Atom.KnowledgeOfPlayer, true);
            if (currentGoal == goals[0]) GenerateAStarPlan();
        }

        if (ttm.GetResponse() == TargetTrackingManager.TResponses.DoNothing)
        {
            if (ttm.target == null)
            {
                startWS.SetValue(Atom.InRange, false);
                startWS.SetValue(Atom.CanSeePlayer, false);
                startWS.SetValue(Atom.KnowledgeOfPlayer, false);
                startWS.SetValue(Atom.PlayerDead, false);
                currentAction = actions[0];
                if (currentGoal != goals[0]) GenerateAStarPlan();
            }
        }


        senses.target = ttm.target;
        target = ttm.target;
    }




    // Transition methods for FSM. Note it is possible to transist to the same state.
    // The States themselves determine when they should transist based on their interactions with the coresponding Action.
    // The State to transist to is determined by the State of the next Action in the Stack.
    public bool GuardGotoToAnimate(State<TacticalStates> currentState) {
        return (currentState.StateFinished && plan.Peek().MoveToState == TacticalStates.Animate);
    }
    public bool GuardGotoToUseSmartObject(State<TacticalStates> currentState) {
        return (currentState.StateFinished && plan.Peek().MoveToState == TacticalStates.UseSmartObject);
    }
    public bool GuardGotoToGoto(State<TacticalStates> currentState) {
        return (currentState.StateFinished && plan.Peek().MoveToState == TacticalStates.Goto);
    }
    public bool GuardAnimateToGoto(State<TacticalStates> currentState) {
        return (currentState.StateFinished && plan.Peek().MoveToState == TacticalStates.Goto);
    }
    public bool GuardAnimateToUseSmartObject(State<TacticalStates> currentState) {
        return (currentState.StateFinished && plan.Peek().MoveToState == TacticalStates.UseSmartObject);
    }
    public bool GuardAnimateToAnimate(State<TacticalStates> currentState) {
        return (currentState.StateFinished && plan.Peek().MoveToState == TacticalStates.Animate);
    }
    public bool GuardUseSmartObjectToGoto(State<TacticalStates> currentState) {
        return (currentState.StateFinished && plan.Peek().MoveToState == TacticalStates.Goto);
    }
    public bool GuardUseSmartObjectToAnimate(State<TacticalStates> currentState) {
        return (currentState.StateFinished && plan.Peek().MoveToState == TacticalStates.Animate);
    }
    public bool GuardUseSmartObjectToUseSmartObject(State<TacticalStates> currentState) {
        return (currentState.StateFinished && plan.Peek().MoveToState == TacticalStates.UseSmartObject);
    }


    // Ensure current tactical state is notified when a trigger is entered
    protected virtual void OnTriggerEnter(Collider collider) {
       // tacticalStateMachine.CurrentState.OnStateTriggerEnter(collider);

       /* if(collider.gameObject.tag == "Gun")
        {
            collider.gameObject.transform.SetParent(hand);
            collider.gameObject.transform.position = hand.transform.position;
        }*/
    }

    // Invoked by animation
    public void PickupKnife() {
        Vector3 positionOffset = Vector3.zero;
        Vector3 rotationOffset;
        Vector3 offset = new Vector3(-0.19f, 0.054f, 0.048f);
        rotationOffset = new Vector3(342f, 212f,101f);
        currentAction.interactObject.transform.parent = hand;
        currentAction.interactObject.transform.localPosition = offset;
        currentAction.interactObject.transform.localEulerAngles = rotationOffset;
    }

    // Invoked by animation
   /* public void DiveBreakWindow() {
        GameObject window = GameObject.Find("BreakableWindow1");
        BreakableObject breakable = window.GetComponent<BreakableObject>();
        if (breakable != null) {
            breakable.triggerBreak();
        }
    }*/

   
}
