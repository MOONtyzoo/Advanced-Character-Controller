using UnityEngine;

namespace PlayerStates
{
    public class Roll : BaseState<Knight.StateKey, Knight>
    {
        public Roll(Knight runnerObject) : base(runnerObject) { }

        float rollDirection = 0.0f;
        float rollTimer = 0.0f;

        public override void Enter()
        {
            if (runnerObject.horizontalInput != 0.0f)
            {
                rollDirection = Mathf.Sign(runnerObject.horizontalInput);
            }
            else if (runnerObject.GetVelocityX() != 0.0f)
            {
                rollDirection = Mathf.Sign(runnerObject.GetVelocityX());
            }
            else
            {
                rollDirection = runnerObject.GetFacingDirection();
            }
            rollTimer = 0.0f;
            runnerObject.SetVelocityX(rollDirection * GetRollSpeed());
            runnerObject.TurnToFaceInputDirection();
            runnerObject.GetAnimator().SetBool("is rolling", true);
            runnerObject.GetAnimator().SetFloat("roll speed", 1.0f / runnerObject.rollDuration);
        }

        public override void Update()
        {
            runnerObject.SetVelocityX(rollDirection * GetRollSpeed());
            rollTimer += Time.deltaTime;
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (rollTimer >= runnerObject.rollDuration)
            {
                targetState = Knight.StateKey.Run;
                return true;
            }

            if (!runnerObject.IsGrounded())
            {
                targetState = Knight.StateKey.Aerial;
                return true;
            }

            targetState = Knight.StateKey.Idle;
            return false;
        }

        public override void Exit()
        {
            runnerObject.GetAnimator().SetBool("is rolling", false);
        }

        public float GetRollSpeed()
        {
            float t = runnerObject.rollSpeedCurve.Evaluate(rollTimer / runnerObject.rollDuration);
            return Mathf.Lerp(runnerObject.rollEndSpeed, runnerObject.rollStartSpeed, t);
        }
    }
}