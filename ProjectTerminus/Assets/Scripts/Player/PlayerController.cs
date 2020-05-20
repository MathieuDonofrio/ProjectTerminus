using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Amount of movement per second")]
    public Vector3 velocity = Vector3.zero;

    [Tooltip("Current head rotation")]
    public HeadRotation headRotation = new HeadRotation();

    [Header("Movement Settings")]
    [Tooltip("Base movement speed in meters/units per second.")]
    public float movementSpeed = 6f;

    [Tooltip("Side movement is sometimes multiplied by this value")]
    public float sideSpeedModifier = 0.85f;

    [Tooltip("Backwards movement is multiplied by this value")]
    public float backwardsSpeedModifier = 0.8f;

    [Header("Air Movement Settings")]
    [Tooltip("Amount of movement allowed per second when in air")]
    public float inAirControl = 0.08f;

    [Tooltip("Movement is multiplied by this value if in air")]
    public float inAirSpeedModifier = 0.5f;

    [Tooltip("Vertical movement is multiplied by this value if falling")]
    public float fallSpeedModifier = 1.5f;

    [Header("Sliding Settings")]
    [Tooltip("Slope sliding constant speed")]
    public float slopeSlidingSpeed = 4.0f;

    [Tooltip("Percentage amount of controlable movement when slope sliding")]
    public float slopeSlidingControl = 0.8f;

    [Header("Jump Settings")]
    [Tooltip("Amount of meters high a jump will vertically move the player")]
    public float jumpHeight = 1.15f;

    [Tooltip("Minimum amount of time delay between jumps in seconds")]
    public float minJumpDelay = 0.5f;

    [Header("Sprint Settings")]
    [Tooltip("Forward movement is multiplied by this value if sprinting")]
    public float sprintSpeedModifier = 1.75f;

    [Header("Crouch Settings")]
    [Tooltip("Movement is multiplied by this value if crouching")]
    public float crouchSpeedModifier = 0.5f;

    [Header("Interpolation Settings")]
    [Tooltip("Time it takes to interpolate head rotation 99% of the way to the target."), Range(0.001f, 1f)]
    public float rotationLerpTime = 0.1f;

    [Tooltip("Time it takes to interpolate position 99% of the way to the target."), Range(0.001f, 1f)]
    public float positionLerpTime = 0.2f;

    [Header("Ground Settings")]
    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;

    [Header("Regeneration Settings")]
    [Tooltip("Amount of time before regenerating after having taken damage")]
    public float regenDelay = 6;

    [Tooltip("Amount of health per second regained when regenerating")]
    public float regenAmount = 2;

    [Header("Audio Settings")]
    [Tooltip("Amount of footstep sounds played when moving one meter")]
    public float footstepFrequency = 0.5f;

    [Tooltip("Volume to play footstep sounds as")]
    public float footstepSFXVolume = 0.5f;

    [Tooltip("Threashold to start hearing heatbeat sounds")]
    public float heartbeatSFXHealthTH = 0.8f;

    [Tooltip("Heart rate at rest in beats per minute. Minimum heart rate")]
    public float restedHeartRate = 40;

    [Tooltip("Volume to play heartbeat sounds as")]
    public float heartbeatSFXVolume = 1f;

    [Header("Body parts")]
    [Tooltip("Player head")]
    public GameObject playerHead;

    [Header("Audio Clips")]
    [Tooltip("Sound played for left footsteps")]
    public AudioClip leftFootstepSFX;

    [Tooltip("Sound played for right footsteps")]
    public AudioClip rightFootstepSFX;

    [Tooltip("Sound played when jumping")]
    public AudioClip jumpSFX;

    [Tooltip("Sound played when landing")]
    public AudioClip landSFX;

    [Tooltip("Sound played when heart is beating")]
    public AudioClip heartbeatSFX;

    [Header("Gun")]
    [Tooltip("The gun holder")]
    public GunHolder gunHolder;

    [Header("HUD")]
    public HUDController hudController;

    [Header("Lose Menu")]
    public LoseMenu loseMenu;

    [Header("Wave Manager")]
    public WaveManager waveManager;

    /* Required Components */

    private PlayerInputHandler inputHandler;

    private Entity entity;

    private CharacterController controller;

    private AudioSource audioSouce;

    /* States */

    public bool IsGrounded { get; private set; }

    public bool IsSprinting { get; private set; }

    public bool IsCrouching { get; private set; }

    /* Timestamps */

    private float lastJumpTime;

    private float lastLandTime;

    private float lastSlopeSlideTime;

    private float lastHeartBeat;

    private float lastDamage;

    /* Counters */

    private float footstepDistanceCounter;

    private float fallDistanceCounter;

    /* Other */

    private HeadRotation targetHeadRotation;

    private Vector3 groundNormal;

    private bool colliding;

    private bool footstepSwitch;

    private void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        entity = GetComponent<Entity>();
        controller = GetComponent<CharacterController>();
        audioSouce = GetComponent<AudioSource>();

        entity.onDamaged += OnDamage;

        entity.onDeath += OnDeath;
    }

    private void Update()
    {

#if UNITY_EDITOR
        Profiler.BeginSample("PlayerControllerUpdate");
#endif

        // State
        IsSprinting = inputHandler.GetSprintInput();
        IsCrouching = inputHandler.GetCrouchInput();

        // Ground Detection
        GroundCheck();

        // Movement
        HandleRotationMovement();
        HandlePositionMovement();

        // Post-Movement
        PostMovement();

        // Regeneration
        HandleRegeneration();

        // SFX Specific
        FootstepSFX();
        HeartbeatSFX();

#if UNITY_EDITOR
        Profiler.EndSample();
#endif
    }

    private void LateUpdate()
    {
        // HUD
        UpdateHUD();
    }

    /* Handlers */

    private void HandlePositionMovement()
    {
        // Get direction movement
        Vector3 movement = inputHandler.GetMoveInput();

        // Apply crouching modifier
        if (IsCrouching) movement *= crouchSpeedModifier;

        // Apply Sprinting modifier
        if (IsSprinting && movement.z > 0) movement *= sprintSpeedModifier;

        // Side speed modifier
        if (IsSprinting) movement.x *= sideSpeedModifier;

        // Backwards speed modifier
        if (movement.z < 0) movement.z *= backwardsSpeedModifier;

        GunController gun = gunHolder.CurrentHeldGun();

        if(gun != null)
        {
            movement *= gun.movementMultiplier;

            if (gunHolder.aiming) movement *= gun.movementAimingModifier;
        }

        // Transform movement for player rotation
        movement = playerHead.transform.TransformDirection(movement);

        // Apply speed
        movement *= movementSpeed;

        // Framerate-independent interpolation
        float t = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / positionLerpTime * Time.deltaTime);

        // In air movement control
        if (!IsGrounded) t *= inAirControl;

        // Update velocity using controller velocity for collision movement
        velocity.x = Mathf.Lerp(controller.velocity.x, movement.x, t);
        velocity.z = Mathf.Lerp(controller.velocity.z, movement.z, t);

        // Apply gravity
        velocity.y += Physics.gravity.y * fallSpeedModifier * Time.deltaTime;

        // Slope sliding
        if (CanSlopeSlide())
        {
            // Find slide direction
            Vector3 slideDirection = Vector3.Cross(Vector3.Cross(Vector3.up, groundNormal), groundNormal);

            // Apply slode sliding control
            velocity *= slopeSlidingControl;

            // Add slide velocity
            velocity += slideDirection * slopeSlidingSpeed;

            // Record last slope slide
            lastSlopeSlideTime = Time.time;
        }

        // Jumping
        if (CanJump() && inputHandler.GetJumpInput())
        {
            // Apply in air speed modifier
            velocity *= inAirSpeedModifier;

            // Add nessesary force to jump to reach jump height
            velocity.y = Mathf.Sqrt(-2.0f * jumpHeight * fallSpeedModifier * Physics.gravity.y);

            // Jump sound
            audioSouce.PlayOneShot(jumpSFX);

            // Record last jump
            lastJumpTime = Time.time;
        }

        // Update transform
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleRotationMovement()
    {
        // Get mouse movement
        Vector2 movement = inputHandler.GetMouseInputs();

        // Add movement
        targetHeadRotation.yaw += movement.x;
        targetHeadRotation.pitch += movement.y;

        // Lock head pitch
        targetHeadRotation.pitch = Mathf.Clamp(targetHeadRotation.pitch, -89, 89);

        // Framerate-independent interpolation
        float t = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / rotationLerpTime * Time.deltaTime);

        // Interpolate current heat rotation
        headRotation.yaw = Mathf.Lerp(headRotation.yaw, targetHeadRotation.yaw, t);
        headRotation.pitch = Mathf.Lerp(headRotation.pitch, targetHeadRotation.pitch, t);

        // Update transform
        transform.localEulerAngles = new Vector3(0.0f, headRotation.yaw, 0.0f);
        playerHead.transform.localEulerAngles = new Vector3(headRotation.pitch, 0.0f, 0.0f);
    }

    private void GroundCheck()
    {
        bool grounded = false;

        colliding = false;

        if (Time.time >= lastJumpTime + 0.1f && Physics.SphereCast(
            GetCapsuleBottomHemisphere(),
            controller.radius, Vector3.down,
            out RaycastHit hit,
            IsGrounded ? (controller.skinWidth + 0.05f) : 0.07f,
            groundCheckLayers, QueryTriggerInteraction.Ignore))
        {
            // Record ground hit normal
            groundNormal = hit.normal.normalized;

            // Only count as grounded if the ground slope is under limit
            grounded = Vector3.Dot(groundNormal, transform.up) > 0f && IsNormalUnderSlopeLimit(groundNormal);

            // Record if was colliding
            colliding = true;
        }

        // Handle landing
        if (grounded && !IsGrounded)
        {
            // Land sound
            if(fallDistanceCounter >= 0.2f) audioSouce.PlayOneShot(landSFX);

            // Reset fall distance
            fallDistanceCounter = 0.0f;
        }

        IsGrounded = grounded;
    }

    private void PostMovement()
    {
        // Jump stop
        if (controller.collisionFlags.HasFlag(CollisionFlags.CollidedAbove) && velocity.y > 0) velocity.y = 0.0f;

        // Fall stop
        if (controller.collisionFlags.HasFlag(CollisionFlags.CollidedBelow) && velocity.y < 0) velocity.y = -1.0f;

        // Make head rotation on 360 degrees
        if (Mathf.Abs(headRotation.yaw) > 360 && Mathf.Abs(targetHeadRotation.yaw) >= 360)
        {
            headRotation.yaw %= 360;
            targetHeadRotation.yaw %= 360;
        }

        // Update counters
        if (IsGrounded)
        {
            // Calculate distance using x and z axis
            float distance = Mathf.Sqrt(controller.velocity.x * controller.velocity.x
                + controller.velocity.z * controller.velocity.z);

            // Increase footstep distance
            footstepDistanceCounter += distance * Time.deltaTime;
        }
        else
        {
            // Calculate distance using y axis
            float distance = -Mathf.Min(controller.velocity.y, 0);

            // Increase fall distance
            fallDistanceCounter += distance * Time.deltaTime;
        }
    }

    private void UpdateHUD()
    {
        // Check for HUD controller
        if (hudController == null)
            return;

        // Update health
        hudController.UpdateHealth(entity.HealthRatio());
    }

    private void HandleRegeneration()
    {
        if(Time.time - lastDamage > regenDelay && !entity.FullHealth())
        {
            entity.Heal(regenAmount * Time.deltaTime, HealType.REGEN);
        }
    }

    private void FootstepSFX()
    {
        // Check for footstep
        if (footstepDistanceCounter < 1.0f / footstepFrequency)
            return;

        // Footstep sound
        audioSouce.PlayOneShot(footstepSwitch ? leftFootstepSFX : rightFootstepSFX, footstepSFXVolume);

        // Reset counter
        footstepDistanceCounter = 0.0f;

        // Change foot
        footstepSwitch = !footstepSwitch;
    }

    private void HeartbeatSFX()
    { 
        // Check if heart beat can be heard
        if (entity.HealthRatio() > heartbeatSFXHealthTH)
            return;

        // Remap health ratio
        float ratio = 1 - (1.0f / heartbeatSFXHealthTH * entity.HealthRatio());

        // Calculate heart rate in beats per minute (BPM)
        float heartRate = restedHeartRate + 40 * ratio;

        // Calculate delta
        float delta = 60.0f / heartRate;
        
        // Check for heart beat
        if (Time.time - lastHeartBeat < delta)
            return;
        
        // Heartbeat sound
        audioSouce.PlayOneShot(heartbeatSFX, ratio * heartbeatSFXVolume);

        // Record last heart beat
        lastHeartBeat = Time.time;
    }

    private void OnDamage(float damage, GameObject damager, DamageType damageType) 
    {
        lastDamage = Time.time;
    }

    private void OnDeath()
    {
        loseMenu.LostGame(waveManager.GetCurrentWave());
    }

    /* Services */

    public bool GroundedOnSlope()
    {
        return IsGrounded && Vector3.Angle(Vector3.up, groundNormal) > 10 && Time.time >= lastJumpTime + 0.1f;
    }

    public bool CanSlopeSlide()
    {
        return colliding && !IsGrounded && velocity.y <= 0 && !IsNormalUnderSlopeLimit(groundNormal);
    }

    public bool CanJump()
    {
        return IsGrounded && Time.time >= lastJumpTime + minJumpDelay;
    }

    public bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(Vector3.up, normal) <= controller.slopeLimit;
    }

    public Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (Vector3.up * controller.radius);
    }

    public Vector3 GetCapsuleTopHemisphere(float height)
    {
        return transform.position + (Vector3.up * (height - controller.radius));
    }

    public Vector3 GetMovement()
    {
        return controller.velocity;
    }
}

/// <summary>
/// Head Rotation struct that stores yaw and pitch
/// </summary>
[Serializable]
public struct HeadRotation
{
    public float yaw;
    public float pitch;
}


#if UNITY_EDITOR

namespace UnityEditor
{
    /// <summary>
    /// Custom property drawer to be able to see yaw and pitch inline in inspector (like Vector3)
    /// </summary>
    [CustomPropertyDrawer(typeof(HeadRotation))]
    public class HeadRotationDrawer : PropertyDrawer
    {

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float yawLabelSize = 30;
            float pitchLabelSize = 35;

            float spacingSize = 5;
            float remainingSize = position.width - yawLabelSize - pitchLabelSize - spacingSize;
            float boxSize = remainingSize * 0.498f;

            float offset = 0;

            var yawLabelRect = new Rect(position.x + offset, position.y, yawLabelSize, position.height);
            offset += yawLabelSize;
            var yawRect = new Rect(position.x + offset, position.y, boxSize, position.height);
            offset += boxSize + spacingSize;
            var pitchLabelRect = new Rect(position.x + offset, position.y, pitchLabelSize, position.height);
            offset += pitchLabelSize;
            var pitchRect = new Rect(position.x + offset, position.y, boxSize, position.height);

            EditorGUI.LabelField(yawLabelRect, "Yaw");
            EditorGUI.PropertyField(yawRect, property.FindPropertyRelative("yaw"), GUIContent.none);
            EditorGUI.LabelField(pitchLabelRect, "Pitch");
            EditorGUI.PropertyField(pitchRect, property.FindPropertyRelative("pitch"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

    }
}

#endif