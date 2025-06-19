using System.Collections;
using UnityEngine;

public class Knight : MonoBehaviour
{
    [SerializeField] private float runSpeed;
    [SerializeField] private float runAccel;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private LayerMask terrainLayerMask;

    private Rigidbody2D rbody;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded = false;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float jumpInput = Input.GetAxis("Jump");

        rbody.linearVelocityX = runSpeed * horizontalInput;

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, terrainLayerMask);

        if (isGrounded && jumpInput > 0)
        {
            rbody.linearVelocityY = jumpVelocity;
        }

        spriteRenderer.flipX = rbody.linearVelocityX == 0.0f ? spriteRenderer.flipX : rbody.linearVelocityX > 0.0f ? false : true;
    }
}
