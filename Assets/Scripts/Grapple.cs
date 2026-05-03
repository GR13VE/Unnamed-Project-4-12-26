using UnityEngine;

using Vector3 = UnityEngine.Vector3;
public class Grapple : MonoBehaviour
{
    [Header("Grapple")]

    public float grappleDistance = 40f;
    public float initialGrappleSpeed = 40f;
    public float continuosGrappleSpeed = 10f;
    public int attackDamage = 2;
    public float grappleCoolDown = 2f;
    private float grappleCoolDownTimer = 0f;

    [Header("Setup")]
    public Transform grappleCast;
    public CharacterController player;
    public LayerMask grappleMask;
    public LayerMask attackLayer;

    RaycastHit grapplePoint;
    Vector3 grappleDir;
    bool isGrappling = false;
    bool initiatedGrapple = false;
    bool hitEnemy = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2") && grappleCoolDownTimer <= 0)
        {
            grappleCoolDownTimer = grappleCoolDown;
            if(Physics.Raycast(grappleCast.position, grappleCast.forward, out grapplePoint, grappleDistance, attackLayer))
            {
                grappleDir = grapplePoint.point - grappleCast.position;
                print("Hit Enemy: " + grapplePoint.point + "____ Direction: " + grappleDir );
                isGrappling = true;
                initiatedGrapple = true;
                hitEnemy = true;
            }
            else if(Physics.Raycast(grappleCast.position, grappleCast.forward, out grapplePoint, grappleDistance, grappleMask))
            {
                grappleDir = grapplePoint.point - grappleCast.position;
                print("Hit: " + grapplePoint.point + "____ Direction: " + grappleDir );
                initiatedGrapple = true;
                isGrappling = true;
            }
        }
        else if(isGrappling && Input.GetButtonUp("Fire2"))
        {
            grappleCoolDownTimer = grappleCoolDown;
            isGrappling = false;
        }
        else if( grappleCoolDownTimer > 0)
            grappleCoolDownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (initiatedGrapple)
        {
            player.Move(Vector3.Normalize(grappleDir) * initialGrappleSpeed * Time.fixedDeltaTime);
            float distance = Vector3.Distance(grappleCast.position, grapplePoint.transform.position);
            print("Initiated Grapple: " + distance);
            initiatedGrapple = false; // Allows for different start up speed

            // Handle damage enemy
            if (hitEnemy && grapplePoint.transform.TryGetComponent<Enemy>(out Enemy T))
            {
                T.TakeDamage(attackDamage);
                print("Grappled Target");
                isGrappling = false;
                grappleCoolDownTimer = grappleCoolDown;
            }
        }
        else if (isGrappling)
        {
            grappleDir = grapplePoint.point - grappleCast.position;
            player.Move(Vector3.Normalize(grappleDir) * continuosGrappleSpeed * Time.fixedDeltaTime);
        }
    }
}
