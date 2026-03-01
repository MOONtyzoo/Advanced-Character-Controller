using UnityEngine;
using StateMachine;

public partial class Knight
{
    public class Dodge : BaseState<StateKey, Knight>
    {
        float diveDirection = 0.0f;

        public Dodge(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            if (runnerObject.horizontalInput != 0.0f)
            {
                diveDirection = Mathf.Sign(runnerObject.horizontalInput);
            }
            else if (runnerObject.rbody.linearVelocityX != 0.0f)
            {
                diveDirection = Mathf.Sign(runnerObject.rbody.linearVelocityX);
            }
            else
            {
                diveDirection = runnerObject.GetFacingDirection();
            }
            runnerObject.rbody.linearVelocityX = diveDirection * GetRollSpeed();
            runnerObject.FlipSpriteToFaceInputDirection();
            runnerObject.animator.SetBool("is dodging", true);
            runnerObject.animator.SetTrigger("dodge enter");
            runnerObject.animator.SetFloat("roll speed", 1.0f / runnerObject.dodgeDuration);
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            runnerObject.rbody.linearVelocityX = diveDirection * GetRollSpeed();   
        }

        public override bool TryGetTransitions(out StateKey targetState)
        {
            if (stateTimer >= runnerObject.dodgeDuration)
            {
                targetState = StateKey.Roll;
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
            runnerObject.animator.SetBool("is dodging", false);
        }

        public float GetRollSpeed()
        {
            float t = runnerObject.dodgeSpeedCurve.Evaluate(stateTimer / runnerObject.dodgeDuration);
            return Mathf.Lerp(runnerObject.dodgeEndSpeed, runnerObject.dodgeStartSpeed, t);
        }
    }
}