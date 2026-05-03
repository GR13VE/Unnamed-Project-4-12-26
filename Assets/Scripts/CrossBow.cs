using UnityEngine;

public class CrossBow : MonoBehaviour
{
    public int damage = 4;
    public float pushForce = 6f;
    public float minPushForce = 2f;
    public float areaOfEffect = 4f;
    public float coolDown = 1f;
    public Transform crossBow;
    public LayerMask groundMask;
    public LayerMask playerMask;
    public LayerMask enemyMask;

    RaycastHit hit;
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
                    if(currentHit.transform.TryGetComponent<CharacterController>(out CharacterController controller))
                    {
                        Vector3 pushDir = Vector3.Normalize(currentHit.transform.position - hit.point);
                        float distance = hit.distance;
                        print("Push Player: " + pushDir + "____" + pushDir * Mathf.Max(pushForce* areaOfEffect/distance  * .1f, minPushForce) + "__ Distance: " + distance);
                        controller.Move(pushDir * pushForce * (Mathf.Max(areaOfEffect/distance, minPushForce) * .1f) * Time.deltaTime);
                    }
                }
                Collider[] enemyHitColliders = Physics.OverlapSphere(hit.point, areaOfEffect, enemyMask);
                foreach(var currentHit in enemyHitColliders)
                {
                    if(currentHit.transform.TryGetComponent<Enemy>(out Enemy target))
                    {
                        target.TakeDamage(damage);
                        print("Hit Enemy");
                    }
                }
            }
            
        }
        else if(coolDownTimer > 0)
            coolDownTimer -= Time.deltaTime;

    }

}
