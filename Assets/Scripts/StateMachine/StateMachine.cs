using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<StateKey, RunnerObject>
    where StateKey : Enum
    where RunnerObject : MonoBehaviour
{
    private Dictionary<StateKey, BaseState<StateKey, RunnerObject>> stateDictionary = new Dictionary<StateKey, BaseState<StateKey, RunnerObject>>();
    private BaseState<StateKey, RunnerObject> currentState;
    private bool debugMode = false;

    public StateMachine(bool debugMode = false) {
        this.debugMode = debugMode;
    }

    public void Begin(StateKey enterState)
    {
        TransitionToState(enterState);
    }

    public void Update()
    {
        currentState.Update();

        if (currentState.TryGetTransitions(out StateKey targetState))
        {
            TransitionToState(targetState);
        }
    }

    private void TransitionToState(StateKey targetStateKey)
    {
        if (stateDictionary.TryGetValue(targetStateKey, out BaseState<StateKey, RunnerObject> targetState))
        {
            bool isTargetStateValid = targetState != null && targetState != currentState;
            if (isTargetStateValid)
            {
                currentState?.Exit();
                currentState = targetState;
                currentState.Enter();
                if (debugMode) Debug.Log(currentState.stateKey.ToString());
            }
        }
    }

    public void AddState(StateKey stateKey, BaseState<StateKey, RunnerObject> newState)
    {
        if (stateDictionary.ContainsKey(stateKey)) return;
        stateDictionary.Add(stateKey, newState);
        newState.stateKey = stateKey;
    }

    public string GetCurrentStateString() => currentState.stateKey.ToString();
}
