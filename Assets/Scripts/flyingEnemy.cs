using UnityEngine;

public class flyingEnemy : MonoBehaviour
{
    [Header("Enemy")]
    public float speed = 10f;
    public float maxAxx = 12f;
    public float turnRadius = 26f;
    public float correctionForce = 1.2f;

    public int damage = 1;

    [Header("Setup")]
    public Transform enemy;
    public CharacterController controller;

    float angleToPlayer;
    Vector3 wishDirection;

    GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate() 
    {
        /*
            I need to change this to move toward target full speed unless target goes outside of in front of self
            if target no longer in sight then we need to slow down in order to re orient self.
        */

        this.player = GameObject.FindWithTag("Player"); // Current player position

        wishDirection = player.transform.position - enemy.position;
        angleToPlayer = Vector3.Angle(wishDirection, enemy.forward);

        controller.Move(Accelerate(Vector3.Normalize(wishDirection), controller.velocity, speed, maxAxx) * Time.fixedDeltaTime);
        enemy.rotation = Quaternion.LookRotation(controller.velocity);

    }

        private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVel, float accel, float maxVel)
    {
        if(angleToPlayer > turnRadius)
        {
            float speed = prevVel.magnitude;
            float drop = correctionForce * speed * Time.fixedDeltaTime;
            if(speed != 0)
                prevVel *= Mathf.Max(speed - drop, 0) / speed;
            accel *= correctionForce;
        }
            
        
        float projVel = Vector3.Dot(prevVel, accelDir);
        float accVel = accel * Time.fixedDeltaTime;

        if (projVel + accVel > maxVel)
            accVel = maxVel - projVel;

        return prevVel + accelDir * accVel;
    }
}
