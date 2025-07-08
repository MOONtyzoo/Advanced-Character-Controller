using UnityEngine;

public partial class Knight
{
    private class StateAttackCrouch : BaseState<StateKey, Knight>
    {
        public StateAttackCrouch(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            runnerObject.GetAnimator().SetTrigger("attack enter");
            runnerObject.GetAnimator().SetTrigger("next attack");
            runnerObject.GetAnimator().SetFloat("attack speed", 0.333f / runnerObject.attackCrouchDuration);
            runnerObject.GetAnimator().SetBool("is attacking", true);
            runnerObject.GetAnimator().SetBool("is crouching", true);
            runnerObject.FlipSpriteToFaceInputDirection();
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {   
            runnerObject.MoveTowardsX(0.0f, runnerObject.runAccel);
        }

        public override bool TryGetTransitions(out StateKey targetState)
        {
            if (stateTimer >= runnerObject.attackCrouchDuration)
            {
                targetState = StateKey.Crouch;
                return true;
            }

            targetState = StateKey.Idle;
            return false;
        }

        public override void Exit()
        {
            runnerObject.GetAnimator().SetBool("is attacking", false);
            runnerObject.GetAnimator().SetBool("is crouching", false);
        }
    }
}