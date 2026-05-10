using UnityEngine;

using Vector3 = UnityEngine.Vector3;
public class Grapple : MonoBehaviour
{
    [Header("Grapple")]

    public float grappleDistance = 40f;
    [SerializeField] float initialGrappleSpeed = 40f;
    [SerializeField] float continuosGrappleSpeed = 10f;
    [SerializeField] float cancelDistance = 2f;
    public int attackDamage = 2;
    public float grappleCoolDown = 2f;
    private float grappleCoolDownTimer = 0f;

    [Header("Setup")]
    [SerializeField] Transform grappleCast;
    [SerializeField] CharacterController player;
    [SerializeField] LayerMask grappleMask;
    [SerializeField] LayerMask attackLayer;

    private RaycastHit grapplePoint;
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
                initiatedGrapple = true;
                hitEnemy = true;
            }
            else if(Physics.Raycast(grappleCast.position, grappleCast.forward, out grapplePoint, grappleDistance, grappleMask))
            {
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
        Vector3 grappleDir;
        AltCharecterMovement playerScript = player.GetComponent<AltCharecterMovement>(); 
        
        if (initiatedGrapple) // Starting speed
        {
            grappleDir = grapplePoint.point - grappleCast.position;

            // Add Grapple variables to player movement script
            playerScript.grappleVelocity = Vector3.Normalize(grappleDir) * initialGrappleSpeed * Time.fixedDeltaTime;
            playerScript.grappleDirection = Vector3.Normalize(grappleDir);
            initiatedGrapple = false; // Allows for different start up speed

            // Handle damage enemy
            if (hitEnemy && grapplePoint.transform.TryGetComponent<Enemy>(out Enemy T))
            {
                T.TakeDamage(attackDamage); // Damage target enemy
                grappleCoolDownTimer = grappleCoolDown;
            }
            else
                isGrappling = true;
        }
        else if (isGrappling)
        {
            grappleDir = grapplePoint.point - grappleCast.position;
            playerScript.grappleDirection = Vector3.Normalize(grappleDir);
            playerScript.grappleVelocity = Vector3.Normalize(grappleDir) * continuosGrappleSpeed * Time.fixedDeltaTime;

            // Should cancel grapple?
            float distance = Vector3.Distance(player.transform.position, grapplePoint.point);
            if(Mathf.Round(distance) <= cancelDistance)
                isGrappling = false;
        }
        else
            playerScript.resetGrapple();
    }
}
