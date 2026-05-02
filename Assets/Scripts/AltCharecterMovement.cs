using System;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AltCharecterMovement : MonoBehaviour
{
    [Header("Ground Movement")]
    public float groundAcc = 8f;
    public float maxGroundVel = 38f; // Mostly limits diagonal player speed added each fixed update
    public float friction = 3.6f;
    public float jumpForce = 2.6f;

    [Header("Arial Movement")]
    public float gravity = -16f;
    public float airAcc = 14f;
    public float airResistance = 1.6f;
    public float maxAirVel = 42f; // Limits arial speed added every fixed update
    public float maxFallVel = -28f;


    [Header("Buffers")]
    public float jumpBuffer = .1f; // Counts early jump presses
    private float jumpBufferTime = 0f;

    [Header("Setup")]
    public CharacterController controller;
    public Transform groundCheck;
    public Transform grappleCast;

    public LayerMask groundMask;
    public string gravityChange = "GravityShifter";


    private float groundDist = .08f;
    private bool isGrounded;

    // Input Handling
    private Vector3 wishDirection; // Desired direction for the player
    private bool isJumping = false;
    bool isNormalGravity = true;
    private Vector3 velocity;
    private Vector3 gravityVector = new Vector3(0, -1, 0); // Start with default gravity;

    // Update is called once per frame
    void Update()
    {
        // Get Wish Direction
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        wishDirection = Vector3.Normalize(transform.right * x + transform.forward * z);

        // Handle Jump
        if (jumpBufferTime > 0 && isGrounded)
        {
            isJumping = true;
            jumpBufferTime = 0;
        }
        else if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                isJumping = true;
            }
            else
                jumpBufferTime = jumpBuffer;
        }
       
        // Decrease Timers
        if (jumpBufferTime > 0)
            jumpBufferTime -= Time.deltaTime;
    }
   // Works
    void FixedUpdate()
    {
        velocity = controller.velocity;

                // Handle Jump
        if (isJumping)
        {
            velocity.y = jumpForce;
            isJumping = false;
        }

        // Add Gravity
        if(isGrounded && velocity.y < 0)
            velocity.y = gravity;
        else
            velocity.y += gravity * Time.fixedDeltaTime;

        // Handel movement
        if (!isGrounded)
            velocity = move(wishDirection, velocity, airAcc, maxAirVel, airResistance); // Quake instead returns Accelerate directly. This approach allows us to easily limit bunny hopping speed with Max Air Velocity and add air resistance.
        else
            velocity = move(wishDirection, velocity, groundAcc, maxGroundVel, friction);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask); // Makes a sphere at groundCheck to detect ground collisions
        if(isGrounded){
            Collider[] hitColliders = Physics.OverlapSphere(groundCheck.position, groundDist, groundMask);
            foreach(var hitCollider in hitColliders)
                if(hitCollider.tag == gravityChange)
                {
                    Vector3 gravityShiftVector = hitCollider.GetComponent<gravityShifter>().gravityShiftVector;
                    print("GravityShifter: " + gravityShiftVector);
                }
        }

        controller.Move(velocity * Time.fixedDeltaTime);
    }

    private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVel, float accel, float maxVel)
    {
        float projVel = Vector3.Dot(prevVel, accelDir);
        float accVel = accel * Time.fixedDeltaTime;

        if (projVel + accVel > maxVel)
            accVel = maxVel - projVel;

        Vector3 finish = prevVel + accelDir * accVel;
        //finish.y = velocity.y; // So that vertical velocity can be handed separately.
        return finish;
    }
    private Vector3 move(Vector3 accelDir, Vector3 prevVel, float acc, float maxAcc, float resistance)
    {
        float speed = prevVel.magnitude;
        // Calculate how much friction to be applied this frame
        float drop = speed * resistance * Time.fixedDeltaTime; // If we are not giving an input apply stop force
        
        if (speed != 0) // Avoid divide by zero
            prevVel *= Mathf.Max(speed - drop, 0) / speed;

        return Accelerate(accelDir, prevVel, acc, maxAcc);
    }

    public void shiftGravity(){
        print("Flip Player");
        isNormalGravity = !isNormalGravity;
        controller.transform.Rotate(0, 0, 180);
    }
}
