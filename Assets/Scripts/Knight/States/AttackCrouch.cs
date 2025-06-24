using UnityEngine;

namespace PlayerStates
{
    public class AttackCrouch : BaseState<Knight.StateKey, Knight>
    {
        public AttackCrouch(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            runnerObject.GetAnimator().SetTrigger("attack enter");
            runnerObject.GetAnimator().SetTrigger("next attack");
            runnerObject.GetAnimator().SetFloat("attack speed", 0.333f / runnerObject.attackCrouchDuration);
            runnerObject.GetAnimator().SetBool("is attacking", true);
            runnerObject.GetAnimator().SetBool("is crouching", true);
            runnerObject.TurnToFaceInputDirection();
        }

        public override void Update()
        {
            runnerObject.MoveTowards(0.0f, runnerObject.runAccel);
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (stateTimer >= runnerObject.attackCrouchDuration)
            {
                targetState = Knight.StateKey.Crouch;
                return true;
            }

            targetState = Knight.StateKey.Idle;
            return false;
        }

        public override void Exit()
        {
            runnerObject.GetAnimator().SetBool("is attacking", false);
            runnerObject.GetAnimator().SetBool("is crouching", false);
        }
    }
}