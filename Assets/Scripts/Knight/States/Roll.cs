using UnityEngine;

namespace PlayerStates
{
    public class Roll : BaseState<Knight.StateKey, Knight>
    {
        float rollDirection = 0.0f;

        public Roll(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            rollDirection = runnerObject.GetFacingDirection();
            runnerObject.SetVelocityX(rollDirection * runnerObject.rollSpeed);
            runnerObject.GetAnimator().SetTrigger("roll enter");
            runnerObject.GetAnimator().SetBool("is rolling", true);
            runnerObject.GetAnimator().SetFloat("roll speed", 0.5f / runnerObject.rollDuration);
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            runnerObject.SetVelocityX(rollDirection * runnerObject.rollSpeed);
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (stateTimer >= runnerObject.rollDuration * 0.66f && runnerObject.verticalInput < 0.0f)
            {
                targetState = Knight.StateKey.Slide;
                return true;
            }

            if (stateTimer >= runnerObject.rollDuration)
            {
                targetState = Knight.StateKey.Run;
                return true;
            }

            if (!runnerObject.IsGrounded())
            {
                targetState = Knight.StateKey.Aerial;
                return true;
            }

            targetState = Knight.StateKey.Idle;
            return false;
        }

        public override void Exit()
        {
            runnerObject.GetAnimator().SetBool("is rolling", false);
        }
    }
}