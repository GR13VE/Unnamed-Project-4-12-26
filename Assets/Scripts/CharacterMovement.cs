using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CharacterMovement : MonoBehaviour
{
    [Header("Ground Movement")]
    public float groundAcc = 38f;
    public float maxGroundVel = 16f; // Mostly limits diagonal player speed added each fixed update
    [SerializeField] float minGroundVel = 2f; // Minimum acceleration that an input can result in
    public float friction = 4f;
    public float jumpForce = 20f;

    [Header("Arial Movement")]
    public float gravity = 32f;
    public float standingGravity = 2f;
    public float airAcc = 16f;
    public float maxAirVel = 12f; // Limits arial speed added every fixed update
    [SerializeField] float minAirVel = 1f;
    public float airResistance = 1.2f;


    [SerializeField] float maxGrappleVel = 24f;
    [SerializeField] float maxGrappleWishDir = .8f; // Clamps the magnitude of the Wish Direction

    [Header("Buffers")]
    [SerializeField] float jumpBuffer = .1f; // Counts early jump presses
    private float jumpBufferTime = 0f;

    [Header("Setup")]
    [SerializeField] float rotateSpeed = 4f; // Handles rotation for gravity changes
    [SerializeField] float gravityDist = 22f; // How far below a new gravity change needs to be to take affect
    [SerializeField] CharacterController controller;
    [SerializeField] Transform groundCheck;

    [SerializeField] LayerMask groundMask;
    [SerializeField] string gravityChange = "GravityShifter";

    private float groundDist = .2f;
    public bool isGrounded;
    public Vector3 gravityNormal = new Vector3(0, -1, 0);
    public Vector3 grappleVelocity;
    public Vector3 grappleDirection;

    // Input Handling
    private Vector3 wishDirection; // Desired direction for the player
    private bool isJumping = false;

    // Update is called once per frame
    void Update()
    {
        // Get Wish Direction
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        wishDirection = transform.right * x + transform.forward * z;
        wishDirection = Vector3.ClampMagnitude(wishDirection, 1f);

        // Handle Jump
        if (jumpBufferTime > 0 && isGrounded)
        {
            isJumping = true;
            jumpBufferTime = 0;
        }
        else if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
                isJumping = true;
            else
                jumpBufferTime = jumpBuffer;
        }
       
        // Decrease Timers
        if (jumpBufferTime > 0)
            jumpBufferTime -= Time.deltaTime;
    }
    
    void FixedUpdate()
    {
        Vector3 velocity = controller.velocity;
        bool isGrappling = grappleDirection != Vector3.zero;
        if (isGrappling)
            wishDirection = Vector3.ClampMagnitude(wishDirection, maxGrappleWishDir);
    
        // Handel movement
        if (!isGrounded)
            velocity = move(wishDirection, velocity, airAcc, isGrappling? maxGrappleVel : maxAirVel,airResistance, minAirVel); // Quake instead returns Accelerate directly. This approach allows us to easily limit bunny hopping speed with Max Air Velocity and add air resistance.
        else
            velocity = move(wishDirection, velocity, groundAcc, isGrappling? maxGrappleVel : maxGroundVel, friction, minGroundVel);
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask); // Makes a sphere at groundCheck to detect ground collisions
        // Check for alt gravity ground via Gravity Shifter tag
        meshGravity();

        // Handle Jump
        if (isJumping)
        {
            velocity -=  gravityNormal.normalized *jumpForce;
            isGrounded = false;
            isJumping = false;
        }
        
        // Handle Grapple and Gravity
        if(isGrappling)
            velocity += grappleVelocity;
        else if(!isGrounded) // Add gravity
            velocity +=  gravityNormal.normalized *  gravity * Time.fixedDeltaTime;
        else // Helps stick the player to the ground
            velocity +=  gravityNormal.normalized * standingGravity * Time.fixedDeltaTime;
        
        applyRotation(); // Applies any gravity shifting rotations

        controller.Move(velocity * Time.fixedDeltaTime);
    }

    private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVel, float accel, float maxVel, float minAcc)
    {
        float projVel = Vector3.Dot(prevVel, accelDir);
        float accVel = accel * Time.fixedDeltaTime;

        if (projVel + accVel > maxVel)
            accVel = maxVel - projVel;
        else if(projVel + accVel< minAcc && accelDir.magnitude != 0)
            return prevVel + accelDir * minAcc;

        return prevVel + accelDir * accVel;
    }
    private Vector3 move(Vector3 accelDir, Vector3 prevVel, float acc, float maxAcc, float resistance, float minAcc)
    {
        float speed = prevVel.magnitude;
        // Calculate how much friction to be applied this frame
        float drop = speed * resistance * Time.fixedDeltaTime; // If we are not giving an input apply stop force
        
        if (speed != 0) // Avoid divide by zero
            prevVel *= Mathf.Max(speed - drop, 0) / speed;

        return Accelerate(accelDir, prevVel, acc, maxAcc, minAcc);
    }

    private void meshGravity()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, -transform.up, gravityDist);
        if(hits.Length != 0)
        {
            RaycastHit[] tagged = hits.Where(i => i.transform.tag == gravityChange).ToArray();
            foreach(var hit in tagged) // Should only have 1 but just in case we wanna change things
            {
               gravityNormal = -hit.normal.normalized;
               return;
            }
        }
        else // Set Gravity to default
            gravityNormal = new Vector3(0, -1, 0);
    }

    public void shiftGravity(Vector3 newGravity){
        print("Gravity SHift: " + newGravity + " magnitude: " + newGravity.magnitude);
        gravityNormal = newGravity.normalized;
    }

    private void applyRotation(){
        Quaternion targetRotate = Quaternion.FromToRotation(controller.transform.up, -gravityNormal) * controller.transform.rotation;
        controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, targetRotate, rotateSpeed * Time.fixedDeltaTime);
    }

    public void resetGrapple()
    {
        grappleDirection = Vector3.zero;
        grappleVelocity = Vector3.zero;
    }
}