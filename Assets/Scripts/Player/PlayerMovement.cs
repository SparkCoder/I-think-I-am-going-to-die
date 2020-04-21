using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float speed = 20f;
    public float gravity = 10;
    public float jumpForce = 3.5f;

    [Header("Sensors")]
    public float height = 1.8f;
    public float heightPadding = 0.1f;
    public LayerMask groundMask;
    public bool DebugChecks;

    [HideInInspector] public CharacterController controller;

    private Vector3 velocity;
    private PlayerInput input;
    private bool isGrounded;
    private bool crouched;
    private float t;

    void Awake()
    {
        isGrounded = false;
        crouched = false;
        t = 0;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
    }


    void FixedUpdate()
    {
        if (!GameManager.Instance.dead)
        {
            // Ground Check
            isGrounded = Physics.Raycast(transform.position, -transform.up, height + heightPadding, groundMask);

            // Stop when grounded
            if (isGrounded && velocity.y < 0)
                velocity.y = -2.0f;

            // Lateral movement
            Vector3 move = transform.right * input.horizontal + transform.forward * t * GameManager.Instance.StaminaBar.value;
            controller.Move(move * speed * Time.fixedDeltaTime);

            // Jump action
            if (input.jumpPressed && isGrounded)
                velocity.y = Mathf.Sqrt(jumpForce * -2.0f * gravity * Physics.gravity.y);

            // Gravity
            velocity.y += gravity * Physics.gravity.y * Time.fixedDeltaTime;

            // Apply vertical movement
            controller.Move(velocity * Time.fixedDeltaTime);

            // Crouching
            if (input.crouchHeld && isGrounded)
            {
                crouched = true;
                controller.height = 1f;
            }
            else if (crouched)
            {
                controller.height = 3.8f;
                controller.Move(Vector3.up * 2.8f);
                crouched = false;
            }

            t += Time.fixedDeltaTime;
            if (t > 1.0f)
                t = 1.0f;

            // Debugging
            if (DebugChecks)
                RunDebug();
        }
    }

    void RunDebug()
    {
        Debug.DrawRay(transform.position, -transform.up * (height + heightPadding), Color.green);
        Debug.Log(input.crouchHeld);
    }
}
