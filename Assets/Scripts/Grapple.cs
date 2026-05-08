using UnityEngine;

using Vector3 = UnityEngine.Vector3;
public class Grapple : MonoBehaviour
{
    [Header("Grapple")]

    public float grappleDistance = 40f;
    [SerializeField] float initialGrappleSpeed = 40f;
    [SerializeField] float continuosGrappleSpeed = 10f;
    public int attackDamage = 2;
    public float grappleCoolDown = 2f;
    private float grappleCoolDownTimer = 0f;

    [Header("Setup")]
    [SerializeField] Transform grappleCast;
    [SerializeField] CharacterController player;
    [SerializeField] LayerMask grappleMask;
    [SerializeField] LayerMask attackLayer;

    private RaycastHit grapplePoint;
    private Vector3 grappleDir;
    private bool isGrappling = false;
    private bool initiatedGrapple = false;
    private bool hitEnemy = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2") && grappleCoolDownTimer <= 0)
        {
            if(Physics.Raycast(grappleCast.position, grappleCast.forward, out grapplePoint, grappleDistance, attackLayer))
            {
                grappleDir = grapplePoint.point - grappleCast.position;
                isGrappling = true;
                initiatedGrapple = true;
                hitEnemy = true;
            }
            else if(Physics.Raycast(grappleCast.position, grappleCast.forward, out grapplePoint, grappleDistance, grappleMask))
            {
                grappleDir = grapplePoint.point - grappleCast.position;
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
        if (initiatedGrapple) // Starting speed
        {
            player.Move(Vector3.Normalize(grappleDir) * initialGrappleSpeed * Time.fixedDeltaTime);
            initiatedGrapple = false; // Allows for different start up speed

            // Handle damage enemy
            if (hitEnemy && grapplePoint.transform.TryGetComponent<Enemy>(out Enemy T))
            {
                T.TakeDamage(attackDamage); // Damage target enemy
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
