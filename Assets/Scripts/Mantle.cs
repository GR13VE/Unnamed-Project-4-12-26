using UnityEngine;

public class Mantle : MonoBehaviour
{
    [SerializeField] float mantleOverDistance = 1f; // How far on the ledge we want the player to end up on
    [SerializeField] float mantleSpeed = 4f;
    [SerializeField] float mantleCastDistance = 2f;

    [SerializeField] Transform mantleCast;
    [SerializeField] Transform cameraCast;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;

    private CharacterController player;
    private RaycastHit mantleHit; // The point we want the player to be after mantle
    private bool isMantling = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerGravity = player.GetComponent<AltCharecterMovement>().gravityNormal;
        RaycastHit hit;
        bool isCameraCast = Physics.Raycast(cameraCast.position, mantleCast.forward, mantleCastDistance + mantleOverDistance, groundMask);
        bool isMantleCast = Physics.Raycast(mantleCast.position, mantleCast.forward, out hit, mantleCastDistance, groundMask);

        if(isMantleCast && !isCameraCast && Input.GetAxisRaw("Vertical") == 1f && !isMantling)
        {            
            Vector3 mantleHitTrans = cameraCast.position + (player.transform.forward * (hit.distance + mantleOverDistance));
            bool isMalleableCast = Physics.Raycast(mantleHitTrans, -cameraCast.up, out mantleHit, Vector3.Distance(cameraCast.position, mantleCast.position), groundMask);
            if(isMalleableCast && playerGravity == -mantleHit.normal.normalized)
            {
                print("Mantle: " + mantleHit.point + " : " + mantleHit.normal + " : " + player.transform.up);
                isMantling = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isMantling)
        {
            Vector3 mantleDir = mantleHit.point - groundCheck.position;
            player.Move(Vector3.Normalize(mantleDir) * mantleSpeed * Time.fixedDeltaTime);
            float distance = Vector3.Distance(mantleHit.point, groundCheck.position);
            if(Mathf.Round(distance) == 0f)
            {
                print("Mantled");
                isMantling = false;
            }
        }
    }
}
