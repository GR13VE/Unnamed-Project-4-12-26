using System.Net.NetworkInformation;
using System.Numerics;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;
public class Grapple : MonoBehaviour
{

    public Transform grappleCast;
    public CharacterController player;
    public LayerMask grappleMask;
    public float grappleDistance = 40f;
    public float grappleSpeed = 40f;
    public float grappleCoolDown = 2f;
    private float grappleCoolDownTimer = 0f;

    RaycastHit grapplePoint;
    Vector3 grappleDir;
    bool isGrappling = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2") && grappleCoolDownTimer <= 0)
        {
            grappleCoolDownTimer = grappleCoolDown;
            if(Physics.Raycast(grappleCast.position, grappleCast.forward, out grapplePoint, grappleDistance, grappleMask))
            {
                grappleDir = grapplePoint.point - grappleCast.position;
                print("Hit: " + grapplePoint.point + "____ Direction: " + grappleDir );
                isGrappling = true;
            }
        }
        else if(grappleCoolDownTimer > 0)
            grappleCoolDownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isGrappling)
        {
            player.Move(Vector3.Normalize(grappleDir) * grappleSpeed * Time.fixedDeltaTime);
            float distance = Vector3.Distance(grappleCast.position, grapplePoint.transform.position);
            print(distance);
            isGrappling = false;
        }
    }
}
