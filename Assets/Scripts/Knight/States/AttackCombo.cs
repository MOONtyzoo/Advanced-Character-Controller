using UnityEngine;

namespace PlayerStates
{
    public class AttackCombo : BaseState<Knight.StateKey, Knight>
    {
        private float attackTimer = 0.0f;
        private int comboNum = 0;

        public AttackCombo(Knight runnerObject) : base(runnerObject) { }

        public override void Enter()
        {
            runnerObject.GetAnimator().SetTrigger("attack enter");
            runnerObject.GetAnimator().SetBool("is attacking", true);
            runnerObject.TurnToFaceInputDirection();
            attackTimer = 0.0f;
            comboNum = 0;
        }

        public override void Update()
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= GetCurrentAttackDuration() && runnerObject.attackInput.WasPressed())
            {
                comboNum = (comboNum + 1) % 2;
                attackTimer = 0.0f;
            }
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (attackTimer >= GetCurrentAttackDuration())
            {
                targetState = Knight.StateKey.Idle;
                return true;
            }

            targetState = Knight.StateKey.Idle;
            return false;
        }

        public override void Exit()
        {
            runnerObject.GetAnimator().SetBool("is attacking", false);
        }
        
        private float GetCurrentAttackDuration() => comboNum == 0 ? runnerObject.attackCombo1Duration : runnerObject.attackCombo2Duration;
    }
}