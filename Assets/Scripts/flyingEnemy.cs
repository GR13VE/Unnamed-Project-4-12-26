using UnityEngine;

public class flyingEnemy : MonoBehaviour
{
    [Header("Enemy")]
    public float speed = 10f;
    public float maxAxx = 12f;
    public float turnRadius = 4f;
    public int damage = 1;

    [Header("Setup")]
    public Transform enemy;
    public CharacterController controller;
    Vector3 wishDirection;

    GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.player = GameObject.FindWithTag("Player");
        wishDirection = new Vector3();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        this.player = GameObject.FindWithTag("Player"); // Current player position

        wishDirection = player.transform.position - enemy.position;

        controller.Move(move(Vector3.Normalize(wishDirection), controller.velocity, speed, maxAxx, turnRadius) * Time.fixedDeltaTime);
        enemy.rotation = Quaternion.LookRotation(controller.velocity);

    }

        private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVel, float accel, float maxVel)
    {
        float speed = prevVel.magnitude;
        float drop = speed * turnRadius * Time.fixedDeltaTime;
        float projVel = Vector3.Dot(prevVel * (speed -drop / speed), accelDir);
        float accVel = accel * Time.fixedDeltaTime;

        if (projVel + accVel > maxVel)
            accVel = maxVel - projVel;

        return prevVel + accelDir * accVel;
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
}
