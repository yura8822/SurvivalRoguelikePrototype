using UnityEngine;

public class ResultState : BaseState
{
    public ResultState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
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
        
        // if (Services.PlayerService.HasPlayer)
        // {
        //     Object.Destroy(Services.PlayerService.Player.gameObject);
        // }
    }
}
