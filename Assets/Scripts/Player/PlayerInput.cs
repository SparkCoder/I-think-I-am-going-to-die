using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{
    public bool EnableInput = true;

    [HideInInspector] public float vertical;
    [HideInInspector] public float horizontal;
    [HideInInspector] public bool jumpPressed;
    [HideInInspector] public bool jumpHeld;
    [HideInInspector] public bool crouchPressed;
    [HideInInspector] public bool crouchHeld;

    private bool ready2Clear;

    void Update()
    {
        InputClear();

        if (!EnableInput)
            return;

        EvaluateInputs();
        horizontal = Mathf.Clamp(horizontal, -1f, 1f);
        vertical = Mathf.Clamp(vertical, -1f, 1f);
    }

    void FixedUpdate()
    {
        ready2Clear = true;
    }

    void InputClear()
    {
        if (!ready2Clear)
            return;

        horizontal = 0f;
        vertical = 0f;
        jumpPressed = false;
        jumpHeld = false;
        crouchPressed = false;
        crouchHeld = false;

        ready2Clear = false;
    }

    void EvaluateInputs()
    {
        horizontal += Input.GetAxis("Horizontal");
        vertical += Input.GetAxis("Vertical");

        jumpPressed = jumpPressed || Input.GetButtonDown("Jump");
        jumpHeld = jumpHeld || Input.GetButton("Jump");
        crouchPressed = crouchPressed || Input.GetButtonDown("Crouch");
        crouchHeld = crouchHeld || Input.GetButton("Crouch");
    }
}
