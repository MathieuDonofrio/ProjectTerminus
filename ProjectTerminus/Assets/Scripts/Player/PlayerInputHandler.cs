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

    private float lastFire;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleSprintInput();
        HandleFireInput();
    }

    /* Handlers */

    private void HandleSprintInput()
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

    private void HandleFireInput()
    {
        if (Input.GetButtonDown(GameConstants.k_Fire))
        {
            lastFire = Time.time;
        }
    }

    /* Services */

    /// <summary>
    /// Returns the move inputs in a vector3 where
    /// x represents the horizontal movement and z represents the vertical movement.
    /// Values are clamped between -1 and 1, 0 represents no movement.
    /// </summary>
    /// <returns>horizontal and vertical move inputs</returns>
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

    /// <summary>
    /// Returns the moue inputs in a vector2 where
    /// x represents the x axis and y represents the y axis.
    /// Values are clamped between -1 and 1, 0 represents no movement.
    /// Y axis is inverted
    /// </summary>
    /// <returns>x and y axis mouse movement</returns>
    public Vector2 GetMouseInputs()
    {
        Vector2 move = new Vector2(
            Input.GetAxisRaw(GameConstants.k_MouseAxisNameHorizontal),
            Input.GetAxisRaw(GameConstants.k_MouseAxisNameVertical));

        // Apply sensitivity curve
        move *= mouseSensitivityCurve.Evaluate(move.magnitude);

        // Invert y axis
        move.y = -move.y;

        return move;
    }

    /// <summary>
    /// Returns whether or not the the player is sprinting. 
    /// Checks to see if the player double clicked sprint input.
    /// </summary>
    /// <returns>true if the sprint input is enabled, false otherwise</returns>
    public bool GetSprintInput()
    {
        bool sprint = Input.GetButton(GameConstants.k_Sprint);

        sprint &= forwardMoveCount >= 2;

        return sprint;
    }

    /// <summary>
    /// Returns whether or not the fire input has been pressed within a timeframe.
    /// </summary>
    /// <param name="releaseDelay">Amount of time after pressing trigger the fire input is still valid</param>
    /// <returns>true if the fire down input was held down within release delay, false otherwise</returns>
    public bool GetFireDownInput(float releaseDelay)
    {
        bool fire = Input.GetButtonDown(GameConstants.k_Fire);

        fire |= Time.time - lastFire <= releaseDelay;

        return fire;
    }

    /// <summary>
    /// Returns whether or not the fire input is being held down.
    /// </summary>
    /// <returns>true if the fire input is being held down, false otherwise</returns>
    public bool GetFireInput()
    {
        return Input.GetButton(GameConstants.k_Fire);
    }

    /// <summary>
    /// Returns whether or not the aim input is being held down.
    /// </summary>
    /// <returns>true if the aim input is being held down, false otherwise</returns>
    public bool GetAimInput()
    {
        return Input.GetButton(GameConstants.k_Aim);
    }

    /// <summary>
    /// Returns whether or not the jump input is being held down.
    /// </summary>
    /// <returns>true if the jump input is being held down, false otherwise</returns>
    public bool GetJumpInput()
    {
        return Input.GetButton(GameConstants.k_Jump);
    }

    /// <summary>
    /// Returns whether or not the crouch input is being held down.
    /// </summary>
    /// <returns>true if the crouch input is being held down, false otherwise</returns>
    public bool GetCrouchInput()
    {
        return Input.GetButton(GameConstants.k_Crouch);
    }

    /// <summary>
    /// Returns whethe or not the reload input is being held down.
    /// </summary>
    /// <returns>reload input</returns>
    public bool GetReloadInput()
    {
        return Input.GetButton(GameConstants.k_Reload);
    }

    /// <summary>
    /// Returns the current mouse scroll wheel
    /// </summary>
    /// <returns>mouse scroll where</returns>
    public float GetMouseScrollWheel()
    {
        return Input.GetAxis(GameConstants.k_MouseScrollWheel);
    }
}
