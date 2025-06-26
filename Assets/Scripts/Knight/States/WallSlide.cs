using UnityEngine;

namespace PlayerStates
{
    public class WallSlide : BaseState<Knight.StateKey, Knight>
    {
        private bool isWallToTheRight;

        public WallSlide(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            isWallToTheRight = runnerObject.IsWallToRight();
            runnerObject.FlipSprite(isWallToTheRight ? true : false);
            runnerObject.GetAnimator().SetTrigger("wall slide enter");
            runnerObject.GetAnimator().SetBool("is wall sliding", true);

            runnerObject.SetGravityScale(0.0f);

            float clampedVelocityY = Mathf.Clamp(runnerObject.GetVelocityY(), -runnerObject.wallSlideSpeed, 2.0f*runnerObject.wallSlideSpeed);
            runnerObject.SetVelocityY(clampedVelocityY);
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            runnerObject.MoveTowardsY(-1.0f * runnerObject.wallSlideSpeed, runnerObject.wallSlideAccel);
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (runnerObject.IsGrounded())
            {
                targetState = Knight.StateKey.Idle;
                return true;
            }

            if (!runnerObject.IsInputtingTowardsWall())
            {
                targetState = Knight.StateKey.Aerial;
                return true;
            }

            targetState = Knight.StateKey.Idle;
            return false;
        }

        public override void Exit()
        {
            runnerObject.FlipSpriteToFaceInputDirection();
            runnerObject.GetAnimator().SetBool("is wall sliding", false);

            runnerObject.SetGravityScale(runnerObject.baseGravityScale);
        }
    }
}