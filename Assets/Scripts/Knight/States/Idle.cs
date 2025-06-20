using UnityEngine;

namespace PlayerStates
{
    public class Idle : BaseState<Knight.StateKey, Knight>
    {
        public Idle(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {

        }

        public override void Update()
        {
            runnerObject.MoveTowards(runnerObject.horizontalInput * runnerObject.runSpeed, runnerObject.runAccel);

            if (runnerObject.WantsToTurn())
            {
                runnerObject.FlipSprite();
            }
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (Mathf.Abs(runnerObject.GetVelocityX()) > 1.0f)
            {
                targetState = Knight.StateKey.Run;
                return true;
            }

            if (runnerObject.verticalInput < 0.0f)
            {
                targetState = Knight.StateKey.Crouch;
                return true;
            }

            if (runnerObject.jumpInput > 0.0f)
            {
                runnerObject.Jump();
                targetState = Knight.StateKey.Aerial;
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

        }
    }
}