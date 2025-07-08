namespace PlayerStates
{
    public class LedgeHang : BaseState<Knight.StateKey, Knight>
    {
        public LedgeHang(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            runnerObject.SetVelocityX(0.0f);
            runnerObject.SetVelocityY(0.0f);
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (runnerObject.jumpInput.WasPressed())
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