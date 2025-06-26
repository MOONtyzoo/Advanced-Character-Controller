namespace PlayerStates
{
    public class Aerial : BaseState<Knight.StateKey, Knight>
    {
        public Aerial(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {

        }

        public override void Update()
        {

        }

        public override void FixedUpdate()
        {
            runnerObject.MoveTowardsX(runnerObject.horizontalInput * runnerObject.runSpeed, runnerObject.crouchAccel);
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (runnerObject.IsGrounded())
            {
                targetState = Knight.StateKey.Idle;
                return true;
            }

            if (runnerObject.dodgeInput.WasPressed())
            {
                targetState = Knight.StateKey.AirDive;
                return true;
            }

            if (runnerObject.IsInputtingTowardsWall() && runnerObject.GetTimeSinceLastJump() >= 0.2f)
            {
                targetState = Knight.StateKey.WallSlide;
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