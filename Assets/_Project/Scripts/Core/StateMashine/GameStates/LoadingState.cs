using UnityEngine;

public class LoadingState : BaseState
{
    public LoadingState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnEnter()
    {
        Services.PoolService?.InitializePools();
        Services.ConfigService?.LoadOrCreateConfig();
        Services.SpatialGridService?.Initialization();
        ChangeState<GameState>();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}