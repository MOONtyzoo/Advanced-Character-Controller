using StateMachine;

public partial class Knight
{
    public class AirDive : BaseState<StateKey, Knight>
    {
        float diveDirection = 0.0f;

        public AirDive(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            runnerObject.FlipSpriteToFaceInputDirection();
            diveDirection = runnerObject.GetFacingDirection();
            runnerObject.rbody.linearVelocityX = diveDirection * runnerObject.diveSpeed;
            runnerObject.rbody.linearVelocityY = runnerObject.diveUpVelocity;
    
            runnerObject.animator.SetBool("is diving", true);
            runnerObject.animator.SetTrigger("dive enter");
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
            runnerObject.rbody.linearVelocityX = diveDirection * runnerObject.diveSpeed;
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
            runnerObject.animator.SetBool("is diving", false);
        }
    }
}