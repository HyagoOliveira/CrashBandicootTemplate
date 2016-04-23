using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actor State Controller.
/// This script allows a control actors' states like Running, Jumping Standing etc.
/// Put this script inside an Actor.
/// </summary>
public class ActorStateController : MonoBehaviour
{
    public Dictionary<ActorState, ActorStateMachine> States { get; private set; }
    public ActorState CurrentActorState { get; private set; }
    public ActorState LastActorState { get; private set; }

    public ActorStateMachine CurrentStateMachine { get { return States[CurrentActorState]; } }
    public string CurrentAnimationName { get { return States[CurrentActorState].AnimationName; } }

    private ActorStateMachine newState;

    private void Start()
    {
        Clear();
    }


    private void Update()
    {
        if (States.Count > 0)
            States[CurrentActorState].UpdateState();
    }


    public void ChangeState(ActorState state)
    {
        newState = GetState(state);
    }

    public void ApplyChange()
    {
        if (CurrentActorState == newState.ActorState)
            return;

        States[CurrentActorState].OnExitState();
        newState.OnEnterState();

        LastActorState = CurrentActorState;
        CurrentActorState = newState.ActorState;
    }


    public ActorStateMachine AddState(ActorState state, bool current = false)
    {
        States.Add(state, new ActorStateMachine(state));

        if (current || States.Count == 1)
        {
            CurrentActorState = state;
            //ChangeState(state);
        }

        return States[state];
    }

    public void RemoveState(ActorState state)
    {
        States.Remove(state);
    }

    public ActorStateMachine GetState(ActorState state)
    {
        if (!States.ContainsKey(state))
            throw new UnityException("State " + state + " was not registered.");

        return States[state];
    }

    public string GetAnimationName(ActorState state)
    {
        return GetState(state).AnimationName;
    }

    public void Clear()
    {
        LastActorState = null;
        CurrentActorState = null;
        if (States != null) States.Clear();
        States = new Dictionary<ActorState, ActorStateMachine>();
    }

    public override string ToString()
    {
        return string.Format("Last State: {0}, Current State: {1}", 
            LastActorState == null ? "SN" : LastActorState.ToString(),
            CurrentStateMachine.ToString());
    }
}
