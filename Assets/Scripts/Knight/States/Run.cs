using UnityEngine;

namespace PlayerStates
{
    public class Run : BaseState<Knight.StateKey, Knight>
    {
        public Run(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            runnerObject.TurnToFaceInputDirection();
        }

        public override void Update()
        {
            runnerObject.MoveTowards(runnerObject.horizontalInput * runnerObject.runSpeed, runnerObject.runAccel);

            if (runnerObject.WantsToTurn())
            {
                runnerObject.GetAnimator().SetBool("is turning", true);
            }
            else
            {
                runnerObject.GetAnimator().SetBool("is turning", false);
            }
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (Mathf.Abs(runnerObject.GetVelocityX()) < 1.0f && runnerObject.horizontalInput == 0.0f)
            {
                targetState = Knight.StateKey.Idle;
                return true;
            }

            if (runnerObject.verticalInput < 0.0f)
            {
                targetState = Knight.StateKey.Slide;
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
            runnerObject.GetAnimator().SetBool("is turning", false);
        }
    }
}