using System;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;
public class Grapple : MonoBehaviour
{
    [Header("Grapple")]

    public float grappleDistance = 40f;
    [SerializeField] float startingGrappleSpeed = 20f;
    [SerializeField] float grappleSpeed = 10f;
    [SerializeField] float cancelDistance = 2f;
    [SerializeField] float wishDirectionClamp = .8f;
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
    private CharacterMovement playerScript;

    void Start()
    {
        playerScript = player.GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2") && grappleCoolDownTimer <= 0)
        {
            if(Physics.Raycast(grappleCast.position, grappleCast.forward, out grapplePoint, grappleMask)) initiatedGrapple = true;
            else  if(Physics.Raycast(grappleCast.position, grappleCast.forward, out grapplePoint, attackLayer)) hitEnemy = true;
            else print("Missed Grapple");
        }
        else if((isGrappling || hitEnemy) && Input.GetButtonUp("Fire2"))
            cancel();
        else if( grappleCoolDownTimer > 0)
            grappleCoolDownTimer -= Time.deltaTime;
    }
    void FixedUpdate()
    {
        if(initiatedGrapple || isGrappling) grapple();
        else if(hitEnemy) enemyHit();
    }

    private void grapple()
    {
        Vector3 grappleDir = grapplePoint.point - grappleCast.position;
        playerScript.grappleDirection = Vector3.Normalize(grappleDir);
        if (initiatedGrapple)
        {
            playerScript.grappleVelocity = grappleDir * startingGrappleSpeed * Time.fixedDeltaTime;
            initiatedGrapple = false;
            isGrappling = true;
        }
        else
            playerScript.grappleVelocity = Vector3.Normalize(grappleDir) * grappleSpeed * Time.fixedDeltaTime;
        
        playerScript.wishDirection = Vector3.ClampMagnitude(playerScript.wishDirection, wishDirectionClamp);

        // Should cancel grapple?
        float distance = Vector3.Distance(player.transform.position, grapplePoint.point);
        if(Mathf.Round(distance) <= cancelDistance) cancel();
    }

    private void enemyHit()
    {
        grapplePoint.transform.TryGetComponent<Enemy>(out Enemy T);
        T.TakeDamage(attackDamage); // Damage target enemy
        cancel();
    }

    public void cancel()
    {
        grappleCoolDownTimer = grappleCoolDown;
        isGrappling = false;
        playerScript.resetGrapple();
    }
}