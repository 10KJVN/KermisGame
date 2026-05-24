using UnityEngine;

/// <summary>
/// Physics‑based character controller for third‑person (or top‑down) movement.
/// Features:
/// - Smooth ground and air movement with separate acceleration values
/// - Jumping with configurable height and extra air jumps
/// - Slope handling: only surfaces within maxGroundAngle are considered "ground"
/// - Contact normal averaging for stable movement on uneven surfaces
/// - Input relative to an optional transform (e.g., camera or character orientation)
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // Ground Contact Count's public getter.
    public int Gcc { get; private set; }
    
    [Header("Movement")]
    [SerializeField] private Transform playerInputSpace;
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 10f;
    [SerializeField, Range(0f, 100f)] private float maxAirAcceleration = 1f;
    
    [Header("Jumping")]
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 2f;
    [SerializeField, Range(0f, 5f)] private int maxAirJumps = 0;
    
    [Header("Slope")]
    [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 25f;
    
    private Vector3 velocity;
    private Vector3 desiredVelocity;
    private Vector3 contactNormal;
    private Rigidbody body; // Interpolate it and freeze rotations
    //private Animator animator;
    
    [Header("Debug")]
    [SerializeField] private int jumpPhase;
    [SerializeField] private bool desiredJump;
    [SerializeField] private int groundContactCount;
    
    private float minGroundDotProduct;
    private bool OnGround => groundContactCount > 0;

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }
    
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        OnValidate();

        //animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        if (playerInputSpace)
        {
            Vector3 forward = playerInputSpace.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = playerInputSpace.right;
            right.y = 0f;
            right.Normalize();

            desiredVelocity = (forward * playerInput.y + right * playerInput.x) * maxSpeed;
        }
        else
        {   
            desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        }
        
        desiredJump |= Input.GetButtonDown("Jump");
    }

    private void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();
        
        if (desiredJump)
        {
            desiredJump = false;
            Jump();

            //animator?.SetTrigger("desiredJump");
        }
        
        body.linearVelocity = velocity;
        ClearState();
    }

    private void UpdateState()
    {
        velocity = body.linearVelocity;

        if (OnGround)
        {
            jumpPhase = 0;
            
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
                Gcc = groundContactCount;
            }
        }
        
        else
        {
            contactNormal = Vector3.up;
        }
    }
    
    private Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    private void AdjustVelocity()
    {
        var xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        var zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
        
        var currentX = Vector3.Dot(velocity, xAxis);
        var currentZ = Vector3.Dot(velocity, zAxis);

        var acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        var maxSpeedChange = acceleration * Time.deltaTime;
        
        var newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        var newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);
        
        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }
    
    private void Jump()
    {
        if (OnGround || jumpPhase < maxAirJumps)
        {
            jumpPhase += 1;
            
            var jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            var alignedSpeed = Vector3.Dot(velocity, contactNormal);
            
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed = alignedSpeed, 0f);
            }
            
            velocity += contactNormal * jumpSpeed;
        }
    }
    
    private void ClearState()
    {
        // OnGround = false;
        groundContactCount = 0;
        Gcc = groundContactCount;
        contactNormal = Vector3.zero;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            
            if (normal.y >= minGroundDotProduct)
            {
                // OnGround = true;
                groundContactCount += 1;
                contactNormal += normal;
                Gcc = groundContactCount;
            }
        }
    }
}