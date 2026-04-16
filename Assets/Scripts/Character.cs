using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public Animator anim;
    string currentAnimState;
    public int attackDamage = 1;
    public float attackDistance = 20f, attackDelay = 0.5f, attackSpeed = 1f;
    public LayerMask attackLayer;
    int attackCount = 0;
    bool attacking = false;
    PlayerInput playerInput;
    public Camera cam;
    //public GameObject hitEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim.CrossFade("Idle", 0f, 0);
    }

    public void ChangeAnimState(string newState)
    {
        if (currentAnimState == newState)
        {
            return;
        }

        currentAnimState = newState;
        anim.CrossFadeInFixedTime(currentAnimState, 0.2f);
    }

    void SetAnim()
    {
        if (!attacking)
        {
            ChangeAnimState("Idle");
        }
    }

    public void Attack()
    {
        attacking = true;

        Invoke(nameof(ResetAttack), 0.5f);
        Invoke(nameof(AttackRaycast), attackDelay);

        if (attackCount == 0)
        {
            attackCount++;
            ChangeAnimState("Attack");

        }
        if (attackCount == 1)
        {
            attackCount = 0;
        }
    }

    void AttackRaycast()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit,
        attackDistance, attackLayer))
        {
            HitTarget(hit.point);

            if (hit.transform.TryGetComponent<Enemy>(out Enemy T))
            {
                T.TakeDamage(attackDamage);
                Console.WriteLine("Hit Target");
            }
        }
    }

    void HitTarget(Vector3 pos)
    {
        Console.WriteLine("Hit Target");
        //GameObject go = Instantiate(hitEffect, pos, Quaternion.identity);
        //Destroy(go, 20);
    }

    void ResetAttack()
    {
        attacking = false;
    }
    void AssignInputs()
    {
        //Input.
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }

        SetAnim();
    }
}
