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

    private RaycastHit grappleHit;
    private RaycastHit enemyHit;
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
            if(Physics.Raycast(grappleCast.position, grappleCast.forward, out grappleHit, grappleDistance, grappleMask)) 
                initiatedGrapple = true;
            if(Physics.Raycast(grappleCast.position, grappleCast.forward, out enemyHit, grappleDistance, attackLayer))
                hitEnemy = true;
            if(initiatedGrapple && hitEnemy) // Priorities grapple
            {
                print(grappleHit.distance + " : " + enemyHit.distance);
                initiatedGrapple = grappleHit.distance <= enemyHit.distance;
                hitEnemy = !initiatedGrapple;
            }
            
        }
        else if( grappleCoolDownTimer > 0)
            grappleCoolDownTimer -= Time.deltaTime;
    }
    void FixedUpdate()
    {
        if(initiatedGrapple || isGrappling) grapple();
        else if(hitEnemy) grappleEnemy();
    }

    private void grapple()
    {
        Vector3 grappleDir = grappleHit.point - grappleCast.position;
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
        float distance = Vector3.Distance(player.transform.position, grappleHit.point);
        if(Mathf.Round(distance) <= cancelDistance) cancel();
    }

    private void grappleEnemy()
    {
        enemyHit.transform.TryGetComponent<Enemy>(out Enemy T);
        print("Hit: " + T.name);
        T.TakeDamage(attackDamage); // Damage target enemy
        cancel();
    }

    public void cancel()
    {
        grappleCoolDownTimer = grappleCoolDown;
        isGrappling = false;
        hitEnemy = false;
        playerScript.resetGrapple();
    }
}