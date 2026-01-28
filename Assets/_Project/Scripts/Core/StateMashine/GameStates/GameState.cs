using UnityEngine;

public class GameState : BaseState
{
    private Transform playerSpawnT;
    private EnemySpawner enemySpawner;
    
    public GameState(StateMachine stateMachine, Transform playerSpawnT, EnemySpawner enemySpawner) : base(stateMachine)
    {
        this.enemySpawner = enemySpawner;
        this.playerSpawnT = playerSpawnT;
    }

    public override void OnEnter()
    {
        // TODO: Запуск после поражения очищает спавнеры
        
        if (!Services.PlayerService.HasPlayer) SpawnPlayer();
        enemySpawner.Initialize();
        enemySpawner.StartSpawning();
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
        enemySpawner.StopSpawning();
    }

    private void SpawnPlayer()
    {
        GameObject prefab = Services.PlayerService.PlayerPrefab;
        
        if (prefab == null)
        {
            Debug.LogError("[GameState] Player prefab not assigned!");
            return;
        }
        Object.Instantiate(prefab, playerSpawnT);
    }
}
