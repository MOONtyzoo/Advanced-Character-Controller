using UnityEngine;

public partial class Knight
{
    private class StateWallSlide : BaseState<StateKey, Knight>
    {
        private bool isWallToTheRight;
        private float timeInputtingAwayFromWall = 0.0f;

        public StateWallSlide(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            isWallToTheRight = runnerObject.IsWallToRight();
            runnerObject.FlipSprite(isWallToTheRight ? true : false);
            runnerObject.GetAnimator().SetTrigger("wall slide enter");
            runnerObject.GetAnimator().SetBool("is wall sliding", true);

            runnerObject.SetGravityScale(0.0f);
            runnerObject.SnapToWall();

            float clampedVelocityY = Mathf.Clamp(runnerObject.GetVelocityY(), -runnerObject.wallSlideSpeed, 2.0f * runnerObject.wallSlideSpeed);
            runnerObject.SetVelocityY(clampedVelocityY);
            runnerObject.SetVelocityX(0.0f);
        }

        public override void Update()
        {
            if (!runnerObject.IsInputtingTowardsWall())
            {
                timeInputtingAwayFromWall += Time.deltaTime;
            }
            else
            {
                timeInputtingAwayFromWall = 0.0f;
            }
        }

        public override void FixedUpdate()
        {
            runnerObject.MoveTowardsY(-1.0f * runnerObject.wallSlideSpeed, runnerObject.wallSlideAccel);
        }

        public override bool TryGetTransitions(out StateKey targetState)
        {
            if (runnerObject.IsGrounded())
            {
                targetState = StateKey.Idle;
                return true;
            }

            if (timeInputtingAwayFromWall >= 0.05f)
            {
                targetState = StateKey.Aerial;
                return true;
            }

            if (runnerObject.jumpInput.WasPressed())
            {
                runnerObject.WallJump();
                targetState = StateKey.Aerial;
                return true;
            }

            targetState = StateKey.Idle;
            return false;
        }

        public override void Exit()
        {
            runnerObject.FlipSprite(runnerObject.IsWallToRight());
            runnerObject.GetAnimator().SetBool("is wall sliding", false);

            runnerObject.SetGravityScale(runnerObject.baseGravityScale);
        }
    }
}