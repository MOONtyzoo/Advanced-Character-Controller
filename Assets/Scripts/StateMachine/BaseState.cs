using System;
using UnityEngine;

public abstract class BaseState<StateKey, RunnerObject>
    where StateKey : Enum
    where RunnerObject : MonoBehaviour
{
    private RunnerObject runnerObject;
    public StateKey stateKey;

    public BaseState(StateKey stateKey, RunnerObject runnerObject)
    {
        this.stateKey = stateKey;
        this.runnerObject = runnerObject;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract bool TryGetTransitions(out StateKey targetState);
    public abstract void Exit();
}