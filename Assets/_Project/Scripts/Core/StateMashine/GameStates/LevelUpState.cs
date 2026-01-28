using UnityEngine;

public class LevelUpState : BaseState
{
    public LevelUpState(StateMachine stateMachine) : base(stateMachine)
    {
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        
        // // Останавливаем движение
        // if (Services.PlayerService.HasPlayer)
        // {
        //     Services.PlayerService.Player.GetMovement()?.SetEnabled(false);
        // }

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
        
        // // Возобновляем движение
        // if (Services.PlayerService.HasPlayer)
        // {
        //     Services.PlayerService.Player.GetMovement()?.SetEnabled(true);
        // }
    }
}
