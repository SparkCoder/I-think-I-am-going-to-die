using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerMovement_old : MonoBehaviour
{
    [Header("Environment Check Parameters")]
    public float FootHeight = 1.0f;
    public float GroundCheckDistance = 0.2f;
    public LayerMask StandableLayer;

    [Header("Movement Parameters")]
    public float Speed = 10f;
    public float turnSpeed = 10;

    [Header("Jump Properties")]
    public float jumpForce = 6.3f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float crouchJumpBoost = 2.5f;    //Jump boost when crouching
    public float hangingJumpForce = 15f;    //Force of wall hanging jump

    [Header("Status Parameters")]
    public bool grounded = false;

    private PlayerInput input;
    private Rigidbody rb;

    void Start()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    void EnvironmentCheck()
    {
        // Check if Grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, FootHeight + GroundCheckDistance, StandableLayer);

        // Debug Rays
        Debug.DrawRay(transform.position, Vector3.down * (FootHeight + GroundCheckDistance), (grounded) ? Color.green : Color.red);
    }

    void GroundMovement()
    {
        // Move Player
        rb.AddForce(Speed * input.vertical * transform.forward);
        rb.AddForce(Speed * input.horizontal * transform.right);
    }

    void PlayerJump()
    {
        // Jump if on the ground
        if (grounded && input.jumpPressed)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Make jump responsive
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !input.jumpHeld)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void FixedUpdate()
    {
        EnvironmentCheck();

        GroundMovement();
        PlayerJump();
    }
}
