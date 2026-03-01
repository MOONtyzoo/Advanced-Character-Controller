using StateMachine;
using UnityEngine;

public partial class Knight
{
    public class Aerial : BaseState<StateKey, Knight>
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

        public override bool TryGetTransitions(out StateKey targetState)
        {
            if (runnerObject.IsGrounded())
            {
                targetState = StateKey.Idle;
                return true;
            }

            if (runnerObject.dodgeInput.WasPressed())
            {
                targetState = StateKey.AirDive;
                return true;
            }

            if (runnerObject.IsInputtingTowardsWall() && runnerObject.timeSinceLastJump >= 0.2f)
            {
                targetState = StateKey.WallSlide;
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