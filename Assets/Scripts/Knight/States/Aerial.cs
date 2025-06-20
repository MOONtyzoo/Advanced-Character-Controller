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
            runnerObject.MoveTowards(runnerObject.horizontalInput * runnerObject.runSpeed, runnerObject.crouchAccel);
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (runnerObject.IsGrounded())
            {
                targetState = Knight.StateKey.Idle;
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