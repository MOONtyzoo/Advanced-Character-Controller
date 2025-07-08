public partial class Knight
{
    private class StateDive : BaseState<StateKey, Knight>
    {
        float diveDirection = 0.0f;

        public StateDive(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            runnerObject.FlipSpriteToFaceInputDirection();
            diveDirection = runnerObject.GetFacingDirection();
            runnerObject.SetVelocityX(diveDirection * runnerObject.diveSpeed);
            runnerObject.SetVelocityY(runnerObject.diveUpVelocity);
            runnerObject.GetAnimator().SetBool("is diving", true);
            runnerObject.GetAnimator().SetTrigger("dive enter");
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
            runnerObject.SetVelocityX(diveDirection * runnerObject.diveSpeed);   
        }

        public override bool TryGetTransitions(out StateKey targetState)
        {
            if (runnerObject.IsGrounded())
            {
                targetState = StateKey.Roll;
                return true;
            }

            if (runnerObject.IsInputtingTowardsWall())
            {
                targetState = StateKey.WallSlide;
                return true;
            }

            targetState = StateKey.Idle;
            return false;
        }

        public override void Exit()
        {
            runnerObject.GetAnimator().SetBool("is diving", false);
        }
    }
}