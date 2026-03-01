using UnityEngine;

// Please read the documentation (in the readme) if this is confusing!

// You can remove this namespace after duplicating the file!
namespace StateMachine
{
    public partial class TemplateRunner : MonoBehaviour
    {
        [SerializeField] private bool debugMode = false;
        
        private StateMachine<StateKey, TemplateRunner> stateMachine;
        public enum StateKey
        {
            Idle,
            Run,
            Aerial
        }

        private void Awake()
        {
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            stateMachine = new StateMachine<StateKey, TemplateRunner>();
            stateMachine.SetDebugMode(debugMode);

            var idleState = new TemplateState(this);
            stateMachine.AddState(StateKey.Idle, idleState);
            
            var runState = new TemplateState(this);
            stateMachine.AddState(StateKey.Run, runState);
            
            var aerialState = new TemplateState(this);
            stateMachine.AddState(StateKey.Aerial, aerialState);
            
            stateMachine.Begin(StateKey.Idle); // DONT FORGET THIS!!!
        }

        private void Update()
        {
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }
    }
}
