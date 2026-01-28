using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private IState currentState;
    private Dictionary<Type, IState> states = new Dictionary<Type, IState>();

    public event System.Action<IState, IState> OnStateChanged;

    void Update()
    {
        currentState?.OnUpdate();
    }

    void FixedUpdate()
    {
        currentState?.OnFixedUpdate();
    }

    /// <summary>
    /// Registers the new state of the machine in the machine
    /// </summary>
    public void RegisterState<T>(T state) where T : IState
    {
        Type stateType = typeof(T);

        if (!states.ContainsKey(stateType))
        {
            states.Add(stateType, state);
        }
        else
        {
            Debug.LogWarning($"State {stateType.Name} already registered!");
        }
    }

    /// <summary>
    /// Changes the state to the specified type
    /// </summary>
    public void ChangeState<T>() where T : IState
    {
        Type stateType = typeof(T);

        if (states.ContainsKey(stateType))
        {
            IState newState = states[stateType];

            if (currentState == newState)
            {
                return;
            }

            OnStateChanged?.Invoke(currentState, newState);
            currentState?.OnExit();
            currentState = newState;
            currentState.OnEnter();
        }
        else
        {
            Debug.LogError($"State {stateType.Name} not registered!");
        }
    }

    /// <summary>
    /// Changes the state to the specified type with optional initialization callback
    /// </summary>
    /// <typeparam name="T">The state type to change to</typeparam>
    /// <param name="onStateInit">Optional callback to initialize state before OnEnter is called</param>
    public void ChangeState<T>(Action<T> onStateInit) where T : IState
    {
        Type stateType = typeof(T);

        if (states.ContainsKey(stateType))
        {
            IState newState = states[stateType];

            if (currentState == newState)
            {
                return;
            }

            OnStateChanged?.Invoke(currentState, newState);
            
            currentState?.OnExit();
            currentState = newState;
            onStateInit?.Invoke((T)currentState);
            currentState.OnEnter();
        }
        else
        {
            Debug.LogError($"State {stateType.Name} not registered!");
        }
    }


    /// <summary>
    /// Gets the current state
    /// </summary>
    public T GetCurrentState<T>() where T : class, IState
    {
        return currentState as T;
    }

    /// <summary>
    /// Checks if the machine is in the specified state
    /// </summary>
    public bool IsInState<T>() where T : IState
    {
        return currentState is T;
    }

    /// <summary>
    /// Gets all registered states
    /// </summary>
    public IEnumerable<Type> GetRegisteredStates()
    {
        return states.Keys;
    }

    /// <summary>
    /// Starts the state machine with the initial stateм
    /// </summary>
    public void StartStateMachine<T>() where T : IState
    {
        if (states.Count == 0)
        {
            Debug.LogError("No states registered! Register states before starting.");
            return;
        }

        ChangeState<T>();
    }
}