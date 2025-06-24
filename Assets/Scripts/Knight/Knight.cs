using System.Collections;
using UnityEngine;

public class Knight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask terrainLayerMask;

    [Header("Run")]
    [SerializeField] public float runSpeed;
    [SerializeField] public float runAccel;

    [Header("Jump")]
    [SerializeField] private float jumpVelocity;

    [Header("Crouch")]
    [SerializeField] public float crouchSpeed;
    [SerializeField] public float crouchAccel;

    [Header("Slide")]
    [SerializeField] public float slideSpeed;

    [Header("Dodge")]
    [SerializeField] public AnimationCurve dodgeSpeedCurve;
    [SerializeField] public float dodgeStartSpeed;
    [SerializeField] public float dodgeEndSpeed;
    [SerializeField] public float dodgeDuration;


    [Header("Dive")]
    [SerializeField] public float diveSpeed;
    [SerializeField] public float diveUpVelocity;

    [Header("Roll")]
    [SerializeField] public float rollSpeed;
    [SerializeField] public float rollDuration;

    [Header("Attacks")]
    [SerializeField] public float attackCombo1Duration;
    [SerializeField] public float attackCombo1EndLag;
    [SerializeField] public float attackCombo2Duration;
    [SerializeField] public float attackCombo2EndLag;
    [SerializeField] public float attackCrouchDuration;

    private Rigidbody2D rbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private StateMachine<StateKey, Knight> stateMachine;
    public enum StateKey
    {
        Idle, Run, Crouch, Slide, Dodge, Roll,

        Aerial, AirDive,
        WallSlide, LedgeHang, LedgeVault,

        AttackCombo, AttackCrouch,

        Hurt, Die
    }

    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public InputButton jumpInput = new InputButton("Jump", 0.2f);
    [HideInInspector] public InputButton dodgeInput = new InputButton("Roll", 0.2f);
    [HideInInspector] public InputButton attackInput = new InputButton("Attack", 0.2f);

    private bool isGrounded = false;
    private bool isFalling = false;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        SetUpStateMachine();
    }

    private void SetUpStateMachine()
    {
        stateMachine = new StateMachine<StateKey, Knight>();


        BaseState<StateKey, Knight> idleState = new PlayerStates.Idle(this);
        stateMachine.AddState(StateKey.Idle, idleState);

        BaseState<StateKey, Knight> runState = new PlayerStates.Run(this);
        stateMachine.AddState(StateKey.Run, runState);

        BaseState<StateKey, Knight> crouchState = new PlayerStates.Crouch(this);
        stateMachine.AddState(StateKey.Crouch, crouchState);

        BaseState<StateKey, Knight> slideState = new PlayerStates.Slide(this);
        stateMachine.AddState(StateKey.Slide, slideState);

        BaseState<StateKey, Knight> dodgeState = new PlayerStates.Dodge(this);
        stateMachine.AddState(StateKey.Dodge, dodgeState);

        BaseState<StateKey, Knight> rollState = new PlayerStates.Roll(this);
        stateMachine.AddState(StateKey.Roll, rollState);


        BaseState<StateKey, Knight> aerialState = new PlayerStates.Aerial(this);
        stateMachine.AddState(StateKey.Aerial, aerialState);

        BaseState<StateKey, Knight> airDiveState = new PlayerStates.AirDive(this);
        stateMachine.AddState(StateKey.AirDive, airDiveState);


        BaseState<StateKey, Knight> attackComboState = new PlayerStates.AttackCombo(this);
        stateMachine.AddState(StateKey.AttackCombo, attackComboState);

        BaseState<StateKey, Knight> attackCrouchState = new PlayerStates.AttackCrouch(this);
        stateMachine.AddState(StateKey.AttackCrouch, attackCrouchState);


        stateMachine.Begin(StateKey.Idle);
    }

    private void Update()
    {
        GatherInput();

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, terrainLayerMask) && rbody.linearVelocityY <= 0.5f;
        isFalling = !isGrounded && rbody.linearVelocityY < 0.0f;

        stateMachine.Update();

        UpdateAnimator();
    }

    private void GatherInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        jumpInput.Update();
        dodgeInput.Update();
        attackInput.Update();
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("speed", Mathf.Abs(rbody.linearVelocityX));
        animator.SetBool("is falling", isFalling);
        animator.SetBool("is grounded", isGrounded);
    }

    public void MoveTowards(float targetVelocityX, float acceleration)
    {
        // Faster acceleration if trying to turn in opposite direction or stopping
        float accelFactor = Mathf.Sign(horizontalInput) == Mathf.Sign(rbody.linearVelocityX) ? 1.0f : 2.0f;
        rbody.linearVelocityX = Mathf.MoveTowards(rbody.linearVelocityX, targetVelocityX, accelFactor * acceleration * Time.deltaTime);
    }

    public void Jump()
    {
        rbody.linearVelocityY = jumpVelocity;
        animator.SetTrigger("jump");
    }

    public Animator GetAnimator() => animator;

    public bool IsFacingLeft() => spriteRenderer.flipX;
    public bool IsFacingRight() => !spriteRenderer.flipX;
    public float GetFacingDirection() => spriteRenderer.flipX ? -1.0f : 1.0f;
    public bool WantsToTurn() => (horizontalInput < 0.0f && IsFacingRight())
                || (horizontalInput > 0.0f && IsFacingLeft());
    public void TurnToFaceInputDirection()
    {
        if (horizontalInput == 0.0f) return;
        spriteRenderer.flipX = horizontalInput < 0.0f;
    }

    public Vector2 GetVelocity() => rbody.linearVelocity;
    public float GetVelocityX() => rbody.linearVelocityX;
    public void SetVelocityX(float newVelocityX) => rbody.linearVelocityX = newVelocityX;
    public float GetVelocityY() => rbody.linearVelocityY;
    public void SetVelocityY(float newVelocityY) => rbody.linearVelocityY = newVelocityY;

    public bool IsGrounded() => isGrounded;
    public bool IsFalling() => isFalling;

    public string GetCurrentStateString() => stateMachine.GetCurrentStateString();
}
