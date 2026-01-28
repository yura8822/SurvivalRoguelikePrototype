using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform playerSpawnT;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerUI playerUI;
    
    private StateMachine stateMachine;
    
    public StateMachine StateMachine => stateMachine;
    
    void Awake()
    {
        InitializeStateMachine();
    }

    private void Start()
    {
        stateMachine.StartStateMachine<LoadingState>();
    }

    private void InitializeStateMachine()
    {
        if (stateMachine == null)
        {
            GameObject sm = new GameObject("GameStateMachine");
            sm.transform.SetParent(transform);
            stateMachine = sm.AddComponent<StateMachine>();
        }
        
        stateMachine.RegisterState(new LoadingState(stateMachine));
        stateMachine.RegisterState(new GameState(stateMachine, playerSpawnT, enemySpawner));
        stateMachine.RegisterState(new LevelUpState(stateMachine));
        stateMachine.RegisterState(new ResultState(stateMachine));
    }
 
}
