public partial class Knight
{
    private class StateCrouch : BaseState<StateKey, Knight>
    {
        public StateCrouch(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            runnerObject.GetAnimator().SetBool("is crouching", true);
        }

        public override void Update()
        {
            runnerObject.FlipSpriteToFaceInputDirection();
        }

        public override void FixedUpdate()
        {
            runnerObject.MoveTowardsX(runnerObject.horizontalInput * runnerObject.crouchSpeed, runnerObject.crouchAccel);
        }

        public override bool TryGetTransitions(out StateKey targetState)
        {
            if (runnerObject.verticalInput >= 0.0f)
            {
                targetState = StateKey.Idle;
                return true;
            }

            if (runnerObject.attackInput.WasPressed())
            {
                targetState = StateKey.AttackCrouch;
                return true;
            }

            if (runnerObject.jumpInput.WasPressed())
            {
                runnerObject.Jump();
                targetState = StateKey.Aerial;
                return true;
            }

            if (!runnerObject.IsGrounded())
            {
                targetState = StateKey.Aerial;
                return true;
            }

            targetState = StateKey.Idle;
            return false;
        }

        public override void Exit()
        {
            runnerObject.GetAnimator().SetBool("is crouching", false);
        }
    }
}