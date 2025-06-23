using UnityEngine;

namespace PlayerStates
{
    public class AttackCrouch : BaseState<Knight.StateKey, Knight>
    {
        private float attackTimer = 0.0f;

        public AttackCrouch(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            runnerObject.GetAnimator().SetTrigger("attack enter");
            runnerObject.GetAnimator().SetBool("is attacking", true);
            runnerObject.GetAnimator().SetBool("is crouching", true);
            runnerObject.TurnToFaceInputDirection();
            attackTimer = 0.0f;
        }

        public override void Update()
        {
            attackTimer += Time.deltaTime;
            runnerObject.MoveTowards(0.0f, runnerObject.runAccel);
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (attackTimer >= runnerObject.attackCrouchDuration)
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