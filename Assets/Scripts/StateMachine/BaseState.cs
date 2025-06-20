using System;
using UnityEngine;

public abstract class BaseState<StateKey, RunnerObject>
    where StateKey : Enum
    where RunnerObject : MonoBehaviour
{
    protected RunnerObject runnerObject;
    public StateKey stateKey;

    public BaseState(RunnerObject runnerObject)
    {
        this.runnerObject = runnerObject;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract bool TryGetTransitions(out StateKey targetState);
    public abstract void Exit();
}