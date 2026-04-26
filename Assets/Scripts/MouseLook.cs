using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float maxTilt = 4f;
    public float tiltVelocityMult = .4f;
    public float maxFovChange = 6f;
    public float fovSmoothing = 1f;
    public float maxVel = 60f;
    public Transform playerBody;
    public CharacterController player;
    public Camera cam;

    float xRotation = 0f;
    float tilt = 0f;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Handle Z tilt based on side velocity
        Vector3 local = transform.InverseTransformVector(player.velocity); // Gets local Z velocity of player
        tilt = -local.x * tiltVelocityMult;
        if(tilt < -maxTilt)
            tilt = -maxTilt;
        else if(tilt > maxTilt)
            tilt = maxTilt;
        //print(tilt);

        // Handle Fov change based on velocity
        float speed = player.velocity.magnitude;
        //print(speed + "____" + maxVel);
        cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, 60f + Mathf.Lerp(speed,maxVel, speed/maxVel), fovSmoothing);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, tilt);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}
