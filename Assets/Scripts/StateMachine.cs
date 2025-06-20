using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<StateKey, RunnerObject>
    where StateKey : Enum
    where RunnerObject : MonoBehaviour
{
    private Dictionary<StateKey, BaseState<StateKey, RunnerObject>> stateDictionary = new Dictionary<StateKey, BaseState<StateKey, RunnerObject>>();
    private BaseState<StateKey, RunnerObject> currentState;

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
                currentState.Exit();
                currentState = targetState;
                targetState.Enter();
            }
        }
    }

    public void AddState(StateKey StateKey, BaseState<StateKey, RunnerObject> newState)
    {
        if (stateDictionary.ContainsKey(StateKey)) return;
        stateDictionary.Add(StateKey, newState);
    }
}
