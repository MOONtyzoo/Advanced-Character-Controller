using StateMachine;
using UnityEngine;

public partial class Knight
{
    public class Run : BaseState<StateKey, Knight>
    {
        public Run(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            runnerObject.FlipSpriteToFaceInputDirection();
        }

        public override void Update()
        {
            if (runnerObject.WantsToTurn())
            {
                runnerObject.animator.SetBool("is turning", true);
            }
            else
            {
                runnerObject.animator.SetBool("is turning", false);
            }
        }
        
        public override void FixedUpdate()
        {
            runnerObject.MoveTowardsX(runnerObject.horizontalInput * runnerObject.runSpeed, runnerObject.runAccel);
        }

        public override bool TryGetTransitions(out StateKey targetState)
        {
            if (Mathf.Abs(runnerObject.rbody.linearVelocityX) < 1.0f && runnerObject.horizontalInput == 0.0f)
            {
                targetState = StateKey.Idle;
                return true;
            }

            if (runnerObject.verticalInput < 0.0f)
            {
                targetState = StateKey.Slide;
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
            runnerObject.animator.SetBool("is turning", false);
        }
    }
}