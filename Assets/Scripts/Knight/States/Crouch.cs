using StateMachine;

public partial class Knight
{
    public class Crouch : BaseState<StateKey, Knight>
    {
        public Crouch(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            runnerObject.animator.SetBool("is crouching", true);
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
            runnerObject.animator.SetBool("is crouching", false);
        }
    }
}