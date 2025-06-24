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
            PerformAttackNum(0);
        }

        public override void Update()
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= GetCurrentAttackDuration() && runnerObject.attackInput.WasPressed())
            {
                PerformAttackNum((comboNum + 1) % 2);
            }
        }

        public override bool TryGetTransitions(out Knight.StateKey targetState)
        {
            if (attackTimer >= GetCurrentAttackDuration() + GetCurrentAttackEndLag())
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

        private void PerformAttackNum(int newComboNum)
        {
            comboNum = newComboNum;
            attackTimer = 0.0f;
            runnerObject.GetAnimator().SetTrigger("next attack");
            SetAnimatorAttackSpeed();
            runnerObject.TurnToFaceInputDirection();
        }

        private float GetCurrentAttackDuration()
        {
            if (comboNum == 0)
            {
                return runnerObject.attackCombo1Duration;
            }
            else
            {
                return runnerObject.attackCombo2Duration;
            }
        }

        private float GetCurrentAttackEndLag()
        {
            if (comboNum == 0)
            {
                return runnerObject.attackCombo1EndLag;
            }
            else
            {
                return runnerObject.attackCombo2EndLag;
            }
        }

        private void SetAnimatorAttackSpeed()
        {
            if (comboNum == 0)
            {
                runnerObject.GetAnimator().SetFloat("attack speed", 0.6667f / runnerObject.attackCombo1Duration);
            }
            else
            {
                runnerObject.GetAnimator().SetFloat("attack speed", 0.3333f / runnerObject.attackCombo2Duration);
            }
        }
    }
}