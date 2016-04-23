using UnityEngine;

public class ActorStateMachine
{ 
    public ActorState ActorState { get; private set; }
    public string AnimationName { get; private set; }
    public int Index { get; protected set; }

    public float CurrentTime { get; private set; }
    public float LastTime { get; private set; }

    private static int index = -1;

    public delegate void StateActionEvent();
    private event StateActionEvent enterStateAction;
    private event StateActionEvent onStateAction;
    private event StateActionEvent exitStateAction;

    public ActorStateMachine(ActorState state, string animationName = "")
    {
        this.ActorState = state;
        AnimationName = animationName;
        Index = ++index;

        CurrentTime = 0f;
        LastTime = CurrentTime;
    }

    public void UpdateState()
    {
        if (onStateAction != null)
        {
            onStateAction();
            CurrentTime += Time.deltaTime;
        }
    }

    public void OnEnterState()
    {
        if (enterStateAction != null)
        {
            enterStateAction();
        }
        CurrentTime = 0f;
    }

    public void OnExitState()
    {
        if (exitStateAction != null)
        {
            exitStateAction();
        }

        LastTime = CurrentTime;
        CurrentTime = 0f;
    }

    public ActorStateMachine RegisterOnEnterState(StateActionEvent action)
    {
        enterStateAction += action;
        return this;
    }

    public ActorStateMachine RegisterOnState(StateActionEvent action)
    {
        onStateAction += action;
        return this;
    }

    public ActorStateMachine RegisterOnExitState(StateActionEvent action)
    {
        exitStateAction += action;
        return this;
    }

    public override string ToString()
    {
        return string.Format("{0}, Index: {1}", this.ActorState, Index);
    }
}
