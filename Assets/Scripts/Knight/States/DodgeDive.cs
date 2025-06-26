using UnityEngine;

namespace PlayerStates
{
    public class Dodge : BaseState<Knight.StateKey, Knight>
    {
        float diveDirection = 0.0f;

        public Dodge(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            if (runnerObject.horizontalInput != 0.0f)
            {
                diveDirection = Mathf.Sign(runnerObject.horizontalInput);
            }
            else if (runnerObject.GetVelocityX() != 0.0f)
            {
                diveDirection = Mathf.Sign(runnerObject.GetVelocityX());
            }
            else
            {
                diveDirection = runnerObject.GetFacingDirection();
            }
            runnerObject.SetVelocityX(diveDirection * GetRollSpeed());
            runnerObject.FlipSpriteToFaceInputDirection();
            runnerObject.GetAnimator().SetBool("is dodging", true);
            runnerObject.GetAnimator().SetTrigger("dodge enter");
            runnerObject.GetAnimator().SetFloat("roll speed", 1.0f / runnerObject.dodgeDuration);
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            runnerObject.SetVelocityX(diveDirection * GetRollSpeed());   
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (stateTimer >= runnerObject.dodgeDuration)
            {
                targetState = Knight.StateKey.Roll;
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
            runnerObject.GetAnimator().SetBool("is dodging", false);
        }

        public float GetRollSpeed()
        {
            float t = runnerObject.dodgeSpeedCurve.Evaluate(stateTimer / runnerObject.dodgeDuration);
            return Mathf.Lerp(runnerObject.dodgeEndSpeed, runnerObject.dodgeStartSpeed, t);
        }
    }
}