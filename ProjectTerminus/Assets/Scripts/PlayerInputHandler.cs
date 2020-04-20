using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    /* Configuration */

    [Header("Mouse Settings")]
    [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
    public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

    [Header("Double Press Settings")]
    [Tooltip("Double press time window in seconds."), Range(0.001f, 1f)]
    public float doublePressDelay = 0.5f;

    /* State */

    private int forwardMoveCount;

    /* Timestamps */

    private float lastForwardMove;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        handleSprintInput();
    }

    /* Handlers */

    public void handleSprintInput()
    {
        
        if(Input.GetButtonDown(GameConstants.k_Sprint))
        {
            forwardMoveCount++;
            lastForwardMove = Time.time;
        }
        
        if(!Input.GetButton(GameConstants.k_Sprint) && Time.time - lastForwardMove > doublePressDelay)
        {
            forwardMoveCount = 0;
        }
    }

    /* Inputs */

    public Vector3 GetMoveInput()
    {
        Vector3 move = new Vector3(
            Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 
            0f, 
            Input.GetAxisRaw(GameConstants.k_AxisNameVertical));

        // Make magnitude max 1. Using this, magnitude can be smaller than 1 unlike normalize
        move = Vector3.ClampMagnitude(move, 1);

        return move;
    }

    public Vector2 GetMouseInputs()
    {
        Vector2 move = new Vector2(
            Input.GetAxis(GameConstants.k_MouseAxisNameHorizontal),
            Input.GetAxis(GameConstants.k_MouseAxisNameVertical));

        // Apply sensitivity curve
        move *= mouseSensitivityCurve.Evaluate(move.magnitude);

        // Invert y axis
        move.y = -move.y;

        return move;
    }

    public bool GetSprintInput()
    {
        bool sprint = Input.GetButton(GameConstants.k_Sprint);

        sprint &= forwardMoveCount >= 2;

        return sprint;
    }

    public bool GetJumpInput()
    {
        return Input.GetAxis(GameConstants.k_Jump) > 0;
    }

    public bool GetCrouchInput()
    {
        return Input.GetAxis(GameConstants.k_Crouch) > 0;
    }

}
