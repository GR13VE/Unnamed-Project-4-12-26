using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AltCharecterMovement : MonoBehaviour
{
    [Header("Ground Movement")]
    public float groundAcc = 38f;
    public float maxGroundVel = 16f; // Mostly limits diagonal player speed added each fixed update
    [SerializeField] float minGroundVel = 2f; // Minimum acceleration that an input can result in
    public float friction = 4f;
    public float jumpForce = 20f;

    [Header("Arial Movement")]
    public float gravity = 32f;
    public float airAcc = 16f;
    public float maxAirVel = 12f; // Limits arial speed added every fixed update
    [SerializeField] float minAirVel = 1f;
    public float airResistance = 1.2f; // Adds slight resistance while in air to prevent bunny hopping maintaining all momentum

    [SerializeField] float rotateSpeed = 4f; // Handles rotation for gravity changes
    [SerializeField] float gravityDist = 22f; // How far below a new gravity change needs to be to take affect


    [Header("Buffers")]
    [SerializeField] float jumpBuffer = .1f; // Counts early jump presses
    private float jumpBufferTime = 0f;

    [Header("Setup")]
    [SerializeField] CharacterController controller;
    [SerializeField] Transform groundCheck;

    [SerializeField] LayerMask groundMask;
    [SerializeField] string gravityChange = "GravityShifter";


    private float groundDist = .08f;
    private bool isGrounded;
    private Vector3 gravityNormal = new Vector3(0, -1, 0);

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
        Vector3 velocity;
    
        // Handel movement
        if (!isGrounded)
            velocity = move(wishDirection, controller.velocity, airAcc, maxAirVel, airResistance, minAirVel); // Quake instead returns Accelerate directly. This approach allows us to easily limit bunny hopping speed with Max Air Velocity and add air resistance.
        else
            velocity = move(wishDirection, controller.velocity, groundAcc, maxGroundVel, friction, minGroundVel);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask); // Makes a sphere at groundCheck to detect ground collisions

        // Check for alt gravity ground via Gravity Shifter tag
        meshGravity();

        if (isJumping)
        {
            velocity -=  gravityNormal.normalized *jumpForce;
            isJumping = false;
        }

        // Add gravity
        if(!isGrounded)
            velocity +=  gravityNormal.normalized *  gravity * Time.fixedDeltaTime;
        else // Prevents oddities varied gravity
            velocity +=  gravityNormal.normalized *  2f * Time.fixedDeltaTime;
        
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
        {
            print("Min: " + (prevVel + accelDir * minAcc));
            return prevVel + accelDir * minAcc;
        }
        else if(isGrounded) print("Normal: " + (prevVel + accelDir * accVel));

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
                print(tagged.Length + "  Hit: " + hit.normal);
               gravityNormal = -hit.normal.normalized;
               return;
            }
        }
        else
        {
            gravityNormal = new Vector3(0, -1, 0);
        }
    }

    public void shiftGravity(Vector3 newGravity){
        print("Gravity SHift: " + newGravity + " magnitude: " + newGravity.magnitude);
        gravityNormal = newGravity.normalized;
    }

    private void applyRotation(){
        Quaternion targetRotate = Quaternion.FromToRotation(controller.transform.up, -gravityNormal) * controller.transform.rotation;
        controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, targetRotate, rotateSpeed * Time.fixedDeltaTime);
    }
}
