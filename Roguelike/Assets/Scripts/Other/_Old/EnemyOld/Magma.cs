using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magma : MonoBehaviour, IEnemy
{

    [Header("Health")]
    public float currentHealth;
    public float maxHealth = 200;

    [Header("HP Bar")]
    public Transform pfHealthBar;
    public Vector3 offset = new Vector3(0, 1f);

    HealthSystem healthSystem;
    Transform healthBarTransform;

    public int ID { get; set; }
    public bool IsDead { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void PerformAttack()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth == maxHealth)
        {
            InstantiateHealthBar();
        }

        currentHealth -= damage;
        healthSystem.Damage(damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void InstantiateHealthBar()
    {
        healthSystem = new HealthSystem(maxHealth);

        healthBarTransform = Instantiate(pfHealthBar, transform.position + offset, Quaternion.identity, transform);

        EnemyHealthBar healthBar = healthBarTransform.GetComponent<EnemyHealthBar>();
        healthBar.Setup(healthSystem);

        Debug.Log("Health: " + healthSystem.GetHealthPercent());
        Debug.Log("Health: " + healthSystem.GetCurrentHealth());
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
