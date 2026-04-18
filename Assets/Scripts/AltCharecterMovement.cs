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
    public float maxFallVel = -28f;

    [Header("Buffers")]
    public float jumpBuffer = .1f; // Counts early jump presses
    private float jumpBufferTime = 0f;

    [Header("Setup")]
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;

    private float groundDist = .2f;
    private bool isGrounded;

    private Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 currentDir = transform.right * x + transform.forward * z;

        Vector3 wishDir = Vector3.Normalize(currentDir);

        if(isGrounded && velocity.y < 0)
            velocity.y = 0;
        else
        {
            velocity.y += gravity * Time.fixedDeltaTime ;
            Debug.Log(velocity.y);
            if(velocity.y < maxFallVel)
                velocity.y = maxFallVel;
        }
        // Handel movement
        if (!isGrounded)
            velocity = Accelerate(wishDir, velocity, airAcc, maxAirVel);
        else
            velocity = move(wishDir, velocity, groundAcc, maxGroundVel, friction);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask); // Makes a sphere at groundCheck to detect ground collisions

        // Handle Jump
        if (jumpBufferTime > 0 && isGrounded)
        {
            velocity.y = jumpForce;
            jumpBufferTime = 0;
        }
        else if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
                velocity.y = jumpForce;
            else
                jumpBufferTime = jumpBuffer;
        }

        controller.Move(velocity * Time.fixedDeltaTime);
        
        // Decrease timers
        if (jumpBufferTime > 0)
            jumpBufferTime -= Time.deltaTime;
    }

    private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVel, float accel, float maxVel)
    {
        float projVel = Vector3.Dot(prevVel, accelDir);
        float accVel = accel * Time.fixedDeltaTime;

        if (projVel + accVel > maxVel)
            accVel = maxVel - projVel;

        return prevVel + accelDir * accVel;
    }
    private Vector3 move(Vector3 accelDir, Vector3 prevVel, float acc, float maxAcc, float resistance)
    {
        float speed = prevVel.magnitude;
        speed = Mathf.Round(speed * 10) / 10; //  Prevents sliding and jittering when stopping or changing direction
        float drop = speed * resistance * Time.fixedDeltaTime;
        if (speed != 0)
        {
            prevVel *= Mathf.Max(speed - drop, 0) / speed;
        }
        return Accelerate(accelDir, prevVel, acc, maxAcc);
    }
}
