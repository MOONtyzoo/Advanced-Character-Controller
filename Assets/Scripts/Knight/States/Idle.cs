using UnityEngine;

public partial class Knight
{
    private class StateIdle : BaseState<StateKey, Knight>
    {
        public StateIdle(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            
        }

        public override void Update()
        {
            runnerObject.FlipSpriteToFaceInputDirection();
        }

        public override void FixedUpdate()
        {
            runnerObject.MoveTowardsX(runnerObject.horizontalInput * runnerObject.runSpeed, runnerObject.runAccel);
        }

        public override bool TryGetTransitions(out StateKey targetState)
        {
            if (Mathf.Abs(runnerObject.GetVelocityX()) > 1.0f)
            {
                targetState = StateKey.Run;
                return true;
            }

            if (runnerObject.verticalInput < 0.0f)
            {
                targetState = StateKey.Crouch;
                return true;
            }

            if (runnerObject.dodgeInput.WasPressed())
            {
                targetState = StateKey.Dodge;
                return true;
            }

            if (runnerObject.attackInput.WasPressed())
            {
                targetState = StateKey.AttackCombo;
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

        }
    }
}