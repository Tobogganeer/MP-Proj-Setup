using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    private void Awake()
    {
        instance = this;
    }

    public PlayerPawn player;

    /*
    public CharacterController Controller => instance.controller;


    [Header("Change")]
    public MovementProfile movementProfile;
    //[Min(0f)] public float walkingSpeed = 4;
    //[Min(0f)] public float runningSpeed = 6;
    [Min(0f)] public float speedChangeSmoothing = 5;
    private float currentSpeed;


    [Header("Don't change")]
    public float groundedRaycastSize = 0.5f;
    public float groundedRaycastLength = 0.6f;
    [Min(10f)] public float maxFallSpeed = 35;

    public float downforce = -2;

    public LayerMask groundLayermask;


    public bool grounded { get; private set; }
    public bool wasGrounded { get; private set; }

    private Vector3 currentVelocity;
    private Vector3 desiredVelocity;
    private Vector3 transformedDesiredVelocity;
    private Vector3 actualVelocity;

    private Vector3 lastPos;

    /// <summary>
    /// The the velocity that the controller will move. Includes downforce and gravity
    /// </summary>
    public Vector3 CurrentVelocity => currentVelocity;
    /// <summary>
    /// The desired velocity in world space
    /// </summary>
    public Vector3 DesiredVelocity => desiredVelocity;
    /// <summary>
    /// The desired velocity in local space, with speed applied
    /// </summary>
    public Vector3 TransformedDesiredVelocity => transformedDesiredVelocity;
    /// <summary>
    /// The difference between the controllers last and current position
    /// </summary>
    public Vector3 ActualVelocity => actualVelocity;
    /// <summary>
    /// The actual velocity of the character, in local space
    /// </summary>
    public Vector3 LocalActualVelocity { get; private set; }

    public bool Moving => actualVelocity.Flattened().sqrMagnitude > 0.05f && desiredVelocity.sqrMagnitude > 0.05f;
    public bool Sprinting => Input.GetKey(Inputs.Sprint);

    /// <summary>
    /// A value between 0-1 depending on if the player is walking or running
    /// </summary>
    public float NormalizedSpeed => Mathf.InverseLerp(GetWalkingSpeed(), GetRunningSpeed(), currentSpeed);

    /// <summary>
    /// A value between 0-1, where 0 is stationary and 1 is moving current max speed (walk speed or sprint speed, dependantly).
    /// </summary>
    public float FromStillToMaxSpeed01 => Mathf.InverseLerp(-currentSpeed, currentSpeed, currentVelocity.Flattened().magnitude) * 2 - 1;

    public static event Action<float> OnLand;
    private float airtime;

    public bool crouching;
    public bool running;

    private PlayerStamina stamina;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        actualVelocity = Vector3.zero;
        lastPos = transform.position;
        stamina = new PlayerStamina(player);
    }

    private void Update()
    {
        if (controller == null) controller = GetComponent<CharacterController>();

        Inputs.Update();
        crouching = Input.GetKey(Inputs.Crouch) || Input.GetKey(KeyCode.LeftControl);
        running = Sprinting && stamina.currentStamina > 0f && Moving && DesiredVelocity.z > 0f && Mathf.Abs(DesiredVelocity.x) < 0.5f;

        UpdateGrounded();

        UpdateSpeed();

        Move();

        UpdateFOV();

        UpdateStamina();

        wasGrounded = grounded;
        actualVelocity = (transform.position - lastPos) / Time.deltaTime;
        LocalActualVelocity = transform.InverseTransformDirection(actualVelocity);
        lastPos = transform.position;
    }

    private void Move()
    {
        //desiredVelocity.x = Inputs.Horizontal;
        //desiredVelocity.z = Inputs.Vertical;

        desiredVelocity.x = Inputs.HorizontalNoSmooth;
        desiredVelocity.z = Inputs.VerticalNoSmooth;
        // Sets the desired velocity

        transformedDesiredVelocity = transform.right * desiredVelocity.x + transform.forward * desiredVelocity.z;
        transformedDesiredVelocity = Vector3.ClampMagnitude(transformedDesiredVelocity, 1);
        transformedDesiredVelocity *= currentSpeed;

        float accel = grounded ? movementProfile.groundAcceleration : movementProfile.airAcceleration;

        float y = currentVelocity.y;

        //Vector3 flatVel = currentVelocity.Flattened(); 

        Vector3 flatVel = actualVelocity.Flattened(); // Possible breaking change!

        currentVelocity = Vector3.Lerp(flatVel, transformedDesiredVelocity, Time.deltaTime * accel).WithY(y);

        if (grounded)
        {
            if (!wasGrounded)
            {
                OnLand?.Invoke(airtime);
                airtime = 0;
            }

            currentVelocity.y = -downforce;
        }
        else
        {
            airtime += Time.deltaTime;
        }

        currentVelocity.y = Mathf.Clamp(currentVelocity.y - movementProfile.gravity * Time.deltaTime, -maxFallSpeed, 5000);
        // Adds gravity and also clamps the max vertical speed

        controller.Move(currentVelocity * Time.deltaTime);

        //Debug.Log("Current: " + currentVelocity + " - Actual: " + actualVelocity);
    }

    private void UpdateFOV()
    {
        //float value = Moving && Sprinting ? NormalizedSpeed : 0f;
        float value = running ? NormalizedSpeed : 0f;
        float multiplier = 1f + value * 0.3f; // Will go between 1 and 1.3

        CameraFOV.Set(multiplier);
    }

    private void UpdateGrounded()
    {
        grounded = Physics.CheckSphere(transform.position + Vector3.down * (groundedRaycastLength + controller.skinWidth), groundedRaycastSize, groundLayermask);

        //grounded = Physics.SphereCast(new Ray(transform.position, Vector3.down), groundedRaycastSize, groundedRaycastLength + controller.skinWidth, groundLayermask);
        //applyDownforce = Physics.SphereCast(new Ray(transform.position, Vector3.down), downforceCheckSize, downforceCheckLength + controller.skinWidth, groundLayermask);
    }

    private void UpdateSpeed()
    {
        //bool canRun = LocalActualVelocity.z > 0f && Mathf.Abs(LocalActualVelocity.x) < 0.5f;
        bool canRun = stamina.currentStamina > 0f && DesiredVelocity.z > 0f && Mathf.Abs(DesiredVelocity.x) < 0.5f;
        float desiredSpeed = Sprinting && Moving && canRun ? GetRunningSpeed() : GetWalkingSpeed();

        if (grounded)
            currentSpeed = Mathf.Lerp(currentSpeed, desiredSpeed, Time.deltaTime * speedChangeSmoothing);
        else
            currentSpeed = Mathf.Max(currentSpeed, Mathf.Lerp(currentSpeed, desiredSpeed, Time.deltaTime * speedChangeSmoothing));
    }

    private void UpdateStamina()
    {
        stamina.UpdateStamina(running);
    }

    private void OnDrawGizmosSelected()
    {
        if (controller == null) controller = GetComponent<CharacterController>();

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.down * (groundedRaycastLength + controller.skinWidth), groundedRaycastSize);
        //Gizmos.color = Color.black;
        //Gizmos.DrawSphere(transform.position, crouchRaycastSize / 2f);

        // Just draws the ground check rays so you can make sure they intersect the ground
    }

    //private void OnGUI()
    //{
    //    GUILayout.BeginVertical();
    //    GUILayout.Box("Local Velocity: " + LocalActualVelocity);
    //    //GUILayout.Box("Velocity: " + actualVelocity);
    //    //
    //    //GUILayout.Box("Desired Velocity: " + transformedDesiredVelocity);
    //}


    public float GetWalkingSpeed()
    {
        if (player.IsScourge)
        {
            return GetSpeed(PlayerSpeeds.ScourgeWalking);
        }
        else
        {
            return GetSpeed(PlayerSpeeds.AstronautWalking);
        }
    }

    public float GetRunningSpeed()
    {
        if (player.IsScourge)
        {
            return GetSpeed(PlayerSpeeds.ScourgeRunning);
        }
        else
        {
            return GetSpeed(PlayerSpeeds.AstronautRunning);
        }
    }

    private float GetSpeed(float baseSpeed)
    {
        if (player.IsScourge)
        {
            if (player.HasOxygen) baseSpeed *= PlayerSpeeds.ScourgeOxMul;
            else baseSpeed *= PlayerSpeeds.ScourgeNoOxMul;

            if (crouching) baseSpeed *= PlayerSpeeds.ScourgeCrouchingMul;

            return baseSpeed;
        }
        else
        {
            if (player.HasOxygen)
            {
                if (player.suited) baseSpeed *= PlayerSpeeds.AstronautSuitedOxMul;
                else baseSpeed *= PlayerSpeeds.AstronautUnsuitedOxMul;
            }
            else
            {
                if (player.suited) baseSpeed *= PlayerSpeeds.AstronautSuitedNoOxMul;
                else baseSpeed *= PlayerSpeeds.AstronautUnsuitedNoOxMul;
            }

            if (crouching) baseSpeed *= PlayerSpeeds.AstronautCrouchingMul;

            return baseSpeed;
        }
    }
    */
}
