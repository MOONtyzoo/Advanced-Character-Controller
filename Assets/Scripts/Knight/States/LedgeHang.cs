public partial class Knight
{
    private class StateLedgeHang : BaseState<StateKey, Knight>
    {
        public StateLedgeHang(Knight runnerObject) : base(runnerObject) { }

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

        public override bool TryGetTransitions(out StateKey targetState)
        {
            if (runnerObject.jumpInput.WasPressed())
            {
                targetState = StateKey.Idle;
                return true;
            }

            targetState = StateKey.Idle;
            return false;
        }

        public override void Exit()
        {
            
        }
    }
}