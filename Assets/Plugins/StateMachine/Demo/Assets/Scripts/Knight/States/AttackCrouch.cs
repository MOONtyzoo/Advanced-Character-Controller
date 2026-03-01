using StateMachine;

public partial class Knight
{
    public class AttackCrouch : BaseState<StateKey, Knight>
    {
        public AttackCrouch(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            runnerObject.animator.SetTrigger("attack enter");
            runnerObject.animator.SetTrigger("next attack");
            runnerObject.animator.SetFloat("attack speed", 0.333f / runnerObject.attackCrouchDuration);
            runnerObject.animator.SetBool("is attacking", true);
            runnerObject.animator.SetBool("is crouching", true);
            runnerObject.FlipSpriteToFaceInputDirection();
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {   
            runnerObject.MoveTowardsX(0.0f, runnerObject.runAccel);
        }

        public override bool TryGetTransitions(out StateKey targetState)
        {
            if (stateTimer >= runnerObject.attackCrouchDuration)
            {
                targetState = StateKey.Crouch;
                return true;
            }

            targetState = StateKey.Idle;
            return false;
        }

        public override void Exit()
        {
            runnerObject.animator.SetBool("is attacking", false);
            runnerObject.animator.SetBool("is crouching", false);
        }
    }
}