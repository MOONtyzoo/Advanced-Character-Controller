using System;
using UnityEngine;

public abstract class BaseState<StateKey, RunnerObject>
    where StateKey : Enum
    where RunnerObject : MonoBehaviour
{
    protected RunnerObject runnerObject;
    public StateKey stateKey;
    protected float stateTimer;

    public BaseState(RunnerObject runnerObject)
    {
        this.runnerObject = runnerObject;
    }

    public void BaseEnter()
    {
        stateTimer = 0.0f;
        Enter();
    }
    public abstract void Enter();

    public void BaseUpdate()
    {
        stateTimer += Time.deltaTime;
        Update();
    }
    public abstract void Update();

    public abstract bool TryGetTransitions(out StateKey targetState);
    public abstract void Exit();
}