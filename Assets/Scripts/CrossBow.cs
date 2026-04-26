using UnityEngine;

public class CrossBow : MonoBehaviour
{
    public float damage = 4f;
    public float pushForce = 6f;
    public float areaOfEffect = 4f;
    public float coolDown = 1f;
    public Transform crossBow;
    public LayerMask groundMask;
    public LayerMask playerMask;

    RaycastHit hit;
    RaycastHit sphereHit;
    float coolDownTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f") && coolDownTimer <= 0)
        {
            coolDownTimer = coolDown;
            if(Physics.Raycast(crossBow.position, crossBow.forward, out hit, 80f, groundMask))
            {
                Collider[] hitColliders = Physics.OverlapSphere(hit.point, areaOfEffect, playerMask);
                foreach(var currentHit in hitColliders)
                {
                    print("Sphere Hit: " + currentHit);
                    if(currentHit.transform.TryGetComponent<CharacterController>(out CharacterController controller))
                    {
                        Vector3 pushDir = Vector3.Normalize(currentHit.transform.position - hit.point);
                        print("Push Player: " + pushDir * pushForce);
                        controller.Move(pushDir * pushForce * Time.deltaTime);
                    }
                }
            }
            
        }
        else if(coolDownTimer > 0)
            coolDownTimer -= Time.deltaTime;

    }

}
