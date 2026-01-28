using System;

public abstract class BaseState : IState
{
    protected StateMachine stateMachine;
    
    public BaseState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    
    public virtual void OnEnter()
    {
    }
    
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    
    public virtual void OnExit()
    {
    }

    protected void ChangeState<T>() where T : IState
    {
        stateMachine.ChangeState<T>();
    }

    protected void ChangeState<T>(Action<T> onStateInit) where T : IState
    {
        stateMachine.ChangeState<T>(onStateInit);
    }
}

