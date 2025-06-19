using System.Collections;
using UnityEngine;

public class Knight : MonoBehaviour
{
    [SerializeField] private float runSpeed;
    [SerializeField] private float runAccel;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchAccel;
    [SerializeField] private LayerMask terrainLayerMask;

    private Rigidbody2D rbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float horizontalInput;
    private float verticalInput;
    private float jumpInput;

    private bool isGrounded = false;
    private bool isFalling = false;
    private bool isTurning = false;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        GatherInput();
        MovementLogic();
        UpdateAnimator();
    }

    private void GatherInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        jumpInput = Input.GetAxis("Jump");
    }

    private void MovementLogic()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, terrainLayerMask);
        isFalling = !isGrounded && rbody.linearVelocityY < 0.0f;
        TurningLogic();
       
        // Faster acceleration if trying to turn in opposite direction or stopping
        float accelFactor = Mathf.Sign(horizontalInput) == Mathf.Sign(rbody.linearVelocityX) ? 1.0f : 2.0f;
        rbody.linearVelocityX = Mathf.MoveTowards(rbody.linearVelocityX, horizontalInput * runSpeed, accelFactor * runAccel * Time.deltaTime);

        if (isGrounded && jumpInput > 0.0f)
        {
            rbody.linearVelocityY = jumpVelocity;
            animator.SetTrigger("jump");
        }
    }

    private void TurningLogic()
    {
        bool facingLeft = spriteRenderer.flipX;
        bool wantsToTurn = (horizontalInput < 0.0f && !facingLeft)
            || (horizontalInput > 0.0f && facingLeft);
        bool canTurn = isGrounded;

        if (wantsToTurn && canTurn)
        {
            // Determine what type of turn
            if (Mathf.Abs(rbody.linearVelocityX) > 2.0f)
            {
                // Triggers a turn animation in animator (flips sprite on completion)
                isTurning = true;
            }
            else
            {
                // Performs an instant, in-place turn
                if (!isTurning) spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }
        
        if (!wantsToTurn) { isTurning = false; }
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("run speed", Mathf.Abs(rbody.linearVelocityX));
        animator.SetBool("is falling", isFalling);
        animator.SetBool("is grounded", isGrounded);
        animator.SetBool("is turning", isTurning);
    }
}
