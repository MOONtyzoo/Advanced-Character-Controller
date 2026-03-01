using System;
using UnityEngine;

// Please read the documentation (in the readme) if this is confusing!

// You can remove this namespace after duplicating the file!
namespace StateMachine
{
    public partial class TemplateRunner
    {
        public class TemplateState : BaseState<StateKey, TemplateRunner>
        {
            public TemplateState(TemplateRunner runnerObject) : base(runnerObject) { }
            
            public override void Enter()
            {

            }

            public override void Update()
            {
                
            }

            public override void FixedUpdate()
            {
                
            }
 
            public override bool TryGetTransitions(out StateKey targetState)
            {
                // Default if no transition is triggered
                targetState = StateKey.Idle;
                return false;
            }
            
            public override void Exit()
            {

            }
        }
    }
}
