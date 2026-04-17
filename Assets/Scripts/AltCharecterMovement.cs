using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AltCharecterMovement : MonoBehaviour
{
    [Header("Ground Movement")]
    public float groundAcc = 8f;
    public float maxGroundVel = 38f;
    public float friction = 3.6f;
    public float jumpForce = 2.6f;

    [Header("Arial Movement")]
    public float gravity = -16f;
    public float airAcc = 14f;
    public float maxAirVel = 42f;
    public float airResistance = 2f;
    public float maxFallVel = -28f;

    [Header("Buffers")]
    public float jumpBuffer = .066f; // Counts early jump presses
    private float jumpBufferTime = 0f;

    [Header("Setup")]
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;

    private float groundDist = .2f;
    private bool isGrounded;
    private Vector3 velocity;
    private Vector3 lastDir;

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 currentDir = transform.right * x + transform.forward * z;

        // Handle Jump
        if(Input.GetButtonDown("Jump") || jumpBufferTime > 0)
        {
            if (Physics.CheckSphere(groundCheck.position, groundDist, groundMask)) // We don't update isGrounded until after so bunny hopping is possible
            {
                lastDir = currentDir;
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
            else if(jumpBufferTime <= 0)
                jumpBufferTime = jumpBuffer;
        }

        Vector3 wishDir = Vector3.Normalize(currentDir);

        // Handel movement
        if (isGrounded)
            velocity = move(wishDir, velocity, groundAcc, maxGroundVel, friction);
        else
            velocity = move(wishDir, velocity, airAcc, maxAirVel, airResistance);
        
        // Add gravity
        velocity.y += gravity * Time.deltaTime;
        if(velocity.y < maxFallVel) // Set player terminal velocity
            velocity.y = maxFallVel;

        controller.Move(velocity * Time.deltaTime);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask); // Makes a sphere at groundCheck to detect ground collisions


        // Decrease timers
        if(jumpBufferTime >= 0)
            jumpBufferTime -= Time.deltaTime;
    }

    private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVel, float accel, float maxVel)
    {
        float projVel = Vector3.Dot(prevVel, accelDir);
        float accVel = accel * Time.deltaTime;

        if(projVel + accVel > maxVel)
            accVel = maxVel - projVel;

        return prevVel + accelDir * accVel;
    }
    private Vector3 move(Vector3 accelDir, Vector3 prevVel, float acc, float maxAcc, float resistance)
    {
        float speed = prevVel.magnitude;
        speed = Mathf.Round(speed * 1)/1; //  Prevents sliding and jittering when stopping or changing direction
        if(speed != 0)
        {
            float drop = speed * resistance * Time.deltaTime;
            prevVel *= Mathf.Max(speed - drop, 0) / speed;
        }
        return Accelerate(accelDir, prevVel, acc, maxAcc);
    }
}
