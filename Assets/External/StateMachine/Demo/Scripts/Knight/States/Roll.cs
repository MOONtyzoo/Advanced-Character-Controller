using StateMachine;

public partial class Knight
{
    public class Roll : BaseState<StateKey, Knight>
    {
        float rollDirection = 0.0f;

        public Roll(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            rollDirection = runnerObject.GetFacingDirection();
            runnerObject.rbody.linearVelocityX = rollDirection * runnerObject.rollSpeed;
            runnerObject.animator.SetTrigger("roll enter");
            runnerObject.animator.SetBool("is rolling", true);
            runnerObject.animator.SetFloat("roll speed", 0.5f / runnerObject.rollDuration);
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            runnerObject.rbody.linearVelocityX = rollDirection * runnerObject.rollSpeed;
        }

        public override bool TryGetTransitions(out StateKey targetState)
        {
            if (stateTimer >= runnerObject.rollDuration * 0.66f && runnerObject.verticalInput < 0.0f)
            {
                targetState = StateKey.Slide;
                return true;
            }

            if (stateTimer >= runnerObject.rollDuration)
            {
                targetState = StateKey.Run;
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
            runnerObject.animator.SetBool("is rolling", false);
        }
    }
}