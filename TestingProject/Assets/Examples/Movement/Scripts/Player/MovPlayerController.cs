using UnityEngine;
using Rewired;

public class MovPlayerController : MonoBehaviour
{
    Player input;
    Rigidbody rb;
    Vector3 gravitationalForce;
    Vector3 rayDir = Vector3.down;
    Vector3 prevVelocity = Vector3.zero;

    [Header("Other:")]
    [SerializeField] bool adjustInputsToCameraAngle = false;
    [SerializeField] bool usingLayerMaskForGrounded = true;
    [SerializeField] LayerMask GroundLayer;
    CameraFovController _cameraFovController;
    bool shouldMaintainHeight = true;
    bool _prevGrounded = false;
    bool activeCoyote;
    bool activeBuffer;
    bool isGrounded;
    RaycastHit _rayHit;

    [Header("Height Spring:")]
    [SerializeField] float rideHeight = 1.75f;
    [SerializeField] float rayToGroundLength = 3f;
    [SerializeField] float rideSpringStrength = 50f;
    [SerializeField] float rideSpringDamper = 5f;

    enum lookDirectionOptions { velocity, acceleration, moveInput };
    Quaternion uprightTargetRot = Quaternion.identity;
    Quaternion lastTargetRot;
    Vector3 _platformInitRot;
    bool didLastRayHit;

    [Header("Upright Spring:")]
    [SerializeField] lookDirectionOptions characterLookDirection = lookDirectionOptions.velocity;
    [SerializeField] float uprightSpringStrength = 40f;
    [SerializeField] float uprightSpringDamper = 5f;

    Vector3 moveInput;
    float speedFactor = 1f;
    float maxAccelForceFactor = 1f;
    Vector3 goalVel = Vector3.zero;

    [Header("Movement:")]
    [SerializeField] float maxNormalSpeed = 8f;
    [SerializeField] float maxRunningSpeed = 11f;
    [SerializeField] float acceleration = 200f;
    [SerializeField] float maxAccelForce = 150f;
    [SerializeField] float leanFactor = 0.25f;
    [SerializeField] AnimationCurve accelerationFactorFromDot;
    [SerializeField] AnimationCurve maxAccelerationForceFactorFromDot;
    [SerializeField] Vector3 moveForceScale = new Vector3(1f, 0f, 1f);
    float maxSpeed = 8f;

    Vector3 jumpInput;
    float timeSinceJumpPressed = 0f;
    float timeSinceUngrounded = 0f;
    float timeSinceJump = 0f;
    bool jumpReady = true;
    bool jumped = false;

    [Header("Jump:")]
    [SerializeField] float jumpForceFactor = 10f;
    [SerializeField] float riseGravityFactor = 5f;
    [SerializeField] float fallGravityFactor = 10f;
    [SerializeField] float lowJumpFactor = 2.5f;
    [SerializeField] float jumpBuffer = 0.15f;
    [SerializeField] float coyoteTime = 0.25f;

    #region UnityMethods

    private void Awake()
    {
        input = ReInput.players.GetPlayer(0);
        rb = GetComponent<Rigidbody>();
        gravitationalForce = Physics.gravity * rb.mass;
        _cameraFovController = FindObjectOfType<CameraFovController>();
    }

    private void Update()
    {
        moveInput = new Vector3(input.GetAxisRaw("Horizontal"), 0, input.GetAxisRaw("Vertical"));

        if (input.GetButtonDown("Jump"))
        {
            timeSinceJumpPressed = 0f;

            JumpLogic(_rayHit);
        }

        if (moveInput != Vector3.zero)
        {
            if (input.GetButton("Run")) maxSpeed = maxRunningSpeed;
            else maxSpeed = maxNormalSpeed;
        }
        else maxSpeed = maxNormalSpeed;

        _cameraFovController.CameraFOV(maxSpeed == maxRunningSpeed);
    }

    private void FixedUpdate()
    {
        if (adjustInputsToCameraAngle)
        {
            moveInput = AdjustInputToFaceCamera(moveInput);
        }

        (bool rayHitGround, RaycastHit rayHit) = RaycastToGround();
        SetPlatform(rayHit);

        isGrounded = CheckIfGrounded(rayHitGround, rayHit);
        bool grounded = isGrounded;
        if (grounded == true)
        {
            timeSinceUngrounded = 0f;

            if (timeSinceJump > 0.2f)
            {
                jumped = false;
            }
        }
        else
        {
            timeSinceUngrounded += Time.fixedDeltaTime;
        }

        if (moveInput.sqrMagnitude > 0.1f) CharacterMove(moveInput, rayHit);
        
        CharacterJump(jumpInput, grounded, rayHit);

        if (rayHitGround && shouldMaintainHeight)
        {
            MaintainHeight(rayHit);
        }

        Vector3 lookDirection = GetLookDirection(characterLookDirection);
        MaintainUpright(lookDirection, rayHit);

        _prevGrounded = grounded;
    }

    #endregion

    #region renameLater

    private void MaintainHeight(RaycastHit rayHit)
    {
        Vector3 vel = rb.velocity;
        Vector3 otherVel = Vector3.zero;
        Rigidbody hitBody = rayHit.rigidbody;
        if (hitBody != null)
        {
            otherVel = hitBody.velocity;
        }
        float rayDirVel = Vector3.Dot(rayDir, vel);
        float otherDirVel = Vector3.Dot(rayDir, otherVel);

        float relVel = rayDirVel - otherDirVel;
        float currHeight = rayHit.distance - rideHeight;
        float springForce = (currHeight * rideSpringStrength) - (relVel * rideSpringDamper);
        Vector3 maintainHeightForce = -gravitationalForce + springForce * Vector3.down;
        //Vector3 oscillationForce = springForce * Vector3.down;
        rb.AddForce(maintainHeightForce);
        //_squashAndStretchOcillator.ApplyForce(oscillationForce);
        //Debug.DrawLine(transform.position, transform.position + (_rayDir * springForce), Color.yellow);

        // Apply force to objects beneath
        if (hitBody != null)
        {
            hitBody.AddForceAtPosition(-maintainHeightForce, rayHit.point);
        }
    }

    private void MaintainUpright(Vector3 yLookAt, RaycastHit rayHit = new RaycastHit())
    {
        CalculateTargetRotation(yLookAt, rayHit);

        Quaternion currentRot = transform.rotation;
        Quaternion toGoal = MathsUtils.ShortestRotation(uprightTargetRot, currentRot);

        Vector3 rotAxis;
        float rotDegrees;

        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;

        rb.AddTorque((rotAxis * (rotRadians * uprightSpringStrength)) - (rb.angularVelocity * uprightSpringDamper));
    }

    private void CharacterMove(Vector3 moveInput, RaycastHit rayHit)
    {
        Vector3 m_UnitGoal = moveInput;
        Vector3 unitVel = this.goalVel.normalized;
        float velDot = Vector3.Dot(m_UnitGoal, unitVel);
        float accel = acceleration * accelerationFactorFromDot.Evaluate(velDot);
        Vector3 goalVel = m_UnitGoal * maxSpeed * speedFactor;
        Vector3 otherVel = Vector3.zero;
        Rigidbody hitBody = rayHit.rigidbody;
        this.goalVel = Vector3.MoveTowards(this.goalVel, goalVel, accel * Time.fixedDeltaTime);
        Vector3 neededAccel = (this.goalVel - rb.velocity) / Time.fixedDeltaTime;
        float maxAccel = maxAccelForce * maxAccelerationForceFactorFromDot.Evaluate(velDot) * maxAccelForceFactor;
        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
        rb.AddForceAtPosition(Vector3.Scale(neededAccel * rb.mass, moveForceScale), transform.position + new Vector3(0f, transform.localScale.y * leanFactor, 0f)); // Using AddForceAtPosition in order to both move the player and cause the play to lean in the direction of input.
    }

    private void CharacterJump(Vector3 jumpInput, bool grounded, RaycastHit rayHit)
    {
        timeSinceJumpPressed += Time.fixedDeltaTime;
        timeSinceJump += Time.fixedDeltaTime;
        if (rb.velocity.y < 0)
        {
            shouldMaintainHeight = true;
            jumpReady = true;
            if (!grounded)
            {
                // Increase downforce for a sudden plummet.
                rb.AddForce(gravitationalForce * (fallGravityFactor - 1f)); // Hmm... this feels a bit weird. I want a reactive jump, but I don't want it to dive all the time...
            }
        }
        else if (rb.velocity.y > 0)
        {
            if (!grounded)
            {
                if (jumped)
                {
                    rb.AddForce(gravitationalForce * (riseGravityFactor - 1f));
                }
                if (jumpInput == Vector3.zero)
                {
                    // Impede the jump height to achieve a low jump.
                    rb.AddForce(gravitationalForce * (lowJumpFactor - 1f));
                }
            }
        }

        if (!grounded && !jumped)
        {
            activeCoyote = timeSinceUngrounded < coyoteTime;
        }

        if (!grounded  && jumped)
        {
            activeBuffer = timeSinceJumpPressed < jumpBuffer;
        }

        if (!activeCoyote && !activeBuffer && !grounded) return;

        _rayHit = rayHit;
    }

    private void JumpLogic(RaycastHit rayHit)
    {
        if (!activeCoyote && !activeBuffer && !isGrounded || jumped && !activeBuffer) return;

        if (jumpReady)
        {
            jumpReady = false;
            shouldMaintainHeight = false;
            jumped = true;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Cheat fix... (see comment below when adding force to rigidbody).
            if (rayHit.distance != 0) // i.e. if the ray has hit
            {
                rb.position = new Vector3(rb.position.x, rb.position.y - (rayHit.distance - rideHeight), rb.position.z);
            }
            rb.AddForce(Vector3.up * jumpForceFactor, ForceMode.Impulse); // This does not work very consistently... Jump height is affected by initial y velocity and y position relative to RideHeight... Want to adopt a fancier approach (more like PlayerMovement). A cheat fix to ensure consistency has been issued above...
            timeSinceJumpPressed = jumpBuffer; // So as to not activate further jumps, in the case that the player lands before the jump timer surpasses the buffer.
            timeSinceJump = 0f;

            //FindObjectOfType<AudioManager>().Play("Jump");
        }
    }

    #endregion

    #region Utility

    private Vector3 GetLookDirection(lookDirectionOptions lookDirectionOption)
    {
        Vector3 lookDirection = Vector3.zero;

        if (lookDirectionOption == lookDirectionOptions.velocity || lookDirectionOption == lookDirectionOptions.acceleration)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = 0f;

            if (lookDirectionOption == lookDirectionOptions.velocity)
            {
                lookDirection = velocity;
            }
            else if (lookDirectionOption == lookDirectionOptions.acceleration)
            {
                Vector3 deltaVelocity = velocity - prevVelocity;
                prevVelocity = velocity;
                Vector3 acceleration = deltaVelocity / Time.fixedDeltaTime;
                lookDirection = acceleration;
            }
        }
        else if (lookDirectionOption == lookDirectionOptions.moveInput)
        {
            lookDirection = moveInput;
        }

        return lookDirection;
    }

    private void CalculateTargetRotation(Vector3 yLookAt, RaycastHit rayHit = new RaycastHit())
    {
        if (didLastRayHit)
        {
            lastTargetRot = uprightTargetRot;
            try
            {
                _platformInitRot = transform.parent.rotation.eulerAngles;
            }
            catch
            {
                _platformInitRot = Vector3.zero;
            }
        }
        if (rayHit.rigidbody == null)
        {
            didLastRayHit = true;
        }
        else
        {
            didLastRayHit = false;
        }

        if (yLookAt != Vector3.zero)
        {
            uprightTargetRot = Quaternion.LookRotation(yLookAt, Vector3.up);
            lastTargetRot = uprightTargetRot;
            try
            {
                _platformInitRot = transform.parent.rotation.eulerAngles;
            }
            catch
            {
                _platformInitRot = Vector3.zero;
            }
        }
        else
        {
            try
            {
                Vector3 platformRot = transform.parent.rotation.eulerAngles;
                Vector3 deltaPlatformRot = platformRot - _platformInitRot;
                float yAngle = lastTargetRot.eulerAngles.y + deltaPlatformRot.y;
                uprightTargetRot = Quaternion.Euler(new Vector3(0f, yAngle, 0f));
            }
            catch { }
        }
    }

    private Vector3 AdjustInputToFaceCamera(Vector3 moveInput)
    {
        float facing = Camera.main.transform.eulerAngles.y;
        return (Quaternion.Euler(0, facing, 0) * moveInput);
    }

    private void SetPlatform(RaycastHit rayHit)
    {
        try
        {
            RigidPlatform rigidPlatform = rayHit.transform.GetComponent<RigidPlatform>();
            RigidParent rigidParent = rigidPlatform.rigidParent;
            transform.SetParent(rigidParent.transform);
        }
        catch
        {
            transform.SetParent(null);
        }
    }

    private (bool, RaycastHit) RaycastToGround()
    {
        RaycastHit rayHit;
        Ray rayToGround = new Ray(transform.position, rayDir);

        bool rayHitGround;

        if (usingLayerMaskForGrounded) rayHitGround = Physics.Raycast(rayToGround, out rayHit, rayToGroundLength, GroundLayer.value);
        else rayHitGround = Physics.Raycast(rayToGround, out rayHit, rayToGroundLength);

        //Debug.DrawRay(transform.position, rayDir * rayToGroundLength, Color.blue);

        return (rayHitGround, rayHit);
    }

    private bool CheckIfGrounded(bool rayHitGround, RaycastHit rayHit)
    {
        bool grounded;

        if (rayHitGround == true) grounded = rayHit.distance <= rideHeight * 1.3f;
        else grounded = false;

        return grounded;
    }

    #endregion
}
