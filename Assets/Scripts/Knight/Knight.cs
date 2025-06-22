using System.Collections;
using UnityEngine;

public class Knight : MonoBehaviour
{
    [SerializeField] public float runSpeed;
    [SerializeField] public float runAccel;
    [SerializeField] private float jumpVelocity;
    [SerializeField] public float crouchSpeed;
    [SerializeField] public float crouchAccel;
    [SerializeField] public float slideSpeed;
    [SerializeField] public float rollSpeed;
    [SerializeField] public float rollDuration;
    [SerializeField] private LayerMask terrainLayerMask;

    private Rigidbody2D rbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private StateMachine<StateKey, Knight> stateMachine;
    public enum StateKey
    {
        Idle, Run, Crouch, Roll, Slide,

        Aerial, AirDive,
        WallSlide, LedgeHang, LedgeVault,

        AttackCombo, AttackCrouch,

        Hurt, Die
    }

    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public InputButton jumpInput = new InputButton("Jump", 0.17f);
    [HideInInspector] public InputButton rollInput = new InputButton("Roll", 0.2f);

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
        BaseState<StateKey, Knight> runState = new PlayerStates.Run(this);
        BaseState<StateKey, Knight> crouchState = new PlayerStates.Crouch(this);
        BaseState<StateKey, Knight> aerialState = new PlayerStates.Aerial(this);
        BaseState<StateKey, Knight> slideState = new PlayerStates.Slide(this);
        BaseState<StateKey, Knight> rollState = new PlayerStates.Roll(this);

        stateMachine.AddState(StateKey.Idle, idleState);
        stateMachine.AddState(StateKey.Run, runState);
        stateMachine.AddState(StateKey.Crouch, crouchState);
        stateMachine.AddState(StateKey.Aerial, aerialState);
        stateMachine.AddState(StateKey.Slide, slideState);
        stateMachine.AddState(StateKey.Roll, rollState);

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
        rollInput.Update();
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

    public bool IsGrounded() => isGrounded;
    public bool IsFalling() => isFalling;

    public string GetCurrentStateString() => stateMachine.GetCurrentStateString();
}
