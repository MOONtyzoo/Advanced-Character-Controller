using UnityEngine;

namespace PlayerStates
{
    public class Slide : BaseState<Knight.StateKey, Knight>
    {
        public Slide(Knight runnerObject) : base(runnerObject) { }

        float slideDirection = 0.0f;

        public override void Enter()
        {
            slideDirection = (runnerObject.horizontalInput != 0.0f) ? Mathf.Sign(runnerObject.horizontalInput) : Mathf.Sign(runnerObject.GetVelocityX());
            runnerObject.SetVelocityX(slideDirection * runnerObject.slideSpeed);
            runnerObject.FlipSpriteToFaceInputDirection();
            runnerObject.GetAnimator().SetBool("is sliding", true);
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            runnerObject.MoveTowardsX(slideDirection * runnerObject.slideSpeed, runnerObject.runAccel);   
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (runnerObject.verticalInput >= 0.0f)
            {
                targetState = Knight.StateKey.Run;
                return true;
            }

            if (runnerObject.jumpInput.WasPressed())
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
            runnerObject.GetAnimator().SetBool("is sliding", false);
        }
    }
}