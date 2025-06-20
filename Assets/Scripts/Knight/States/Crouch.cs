namespace PlayerStates
{
    public class Crouch : BaseState<Knight.StateKey, Knight>
    {
        public Crouch(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            runnerObject.GetAnimator().SetBool("is crouching", true);
        }

        public override void Update()
        {
            runnerObject.MoveTowards(runnerObject.horizontalInput * runnerObject.crouchSpeed, runnerObject.crouchAccel);

            if (runnerObject.WantsToTurn())
            {
                runnerObject.FlipSprite();
            }
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (runnerObject.verticalInput >= 0.0f)
            {
                targetState = Knight.StateKey.Idle;
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
            runnerObject.GetAnimator().SetBool("is crouching", false);
        }
    }
}