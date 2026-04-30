using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float currentHealth, maxHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
