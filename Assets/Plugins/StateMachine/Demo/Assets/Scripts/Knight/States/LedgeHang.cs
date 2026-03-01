using StateMachine;

public partial class Knight
{
    public class LedgeHang : BaseState<StateKey, Knight>
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
            runnerObject.rbody.linearVelocityX = 0.0f;
            runnerObject.rbody.linearVelocityY = 0.0f;
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