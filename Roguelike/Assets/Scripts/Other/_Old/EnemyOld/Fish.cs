using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour, IEnemy
{
    [Header("Health")]
    public float currentHealth;
    public float maxHealth = 200;

    [Header("HP Bar")]
    public Transform pfHealthBar;
    public Vector3 offset = new Vector3(0, 1f);

    [Header("Drop")]
    public Item_HandlePickUp pickupItem;
    public DropTable DropTable { get; set; }

    HealthSystem healthSystem;
    Transform healthBarTransform;

    public int ID { get; set; }
    public bool IsDead { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    void Start()
    {
        currentHealth = maxHealth;

        DropTable = new DropTable();
        DropTable.loot = new List<LootDrop>
        {
            new LootDrop("coin", 100),
        };
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

        //Debug.Log("Health: " + healthSystem.GetHealthPercent());
        //Debug.Log("Health: " + healthSystem.GetCurrentHealth());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void InstantiateHealthBar()
    {
        healthSystem = new HealthSystem(maxHealth);

        healthBarTransform = Instantiate(pfHealthBar, transform.position + offset, Quaternion.identity, transform);
        //healthBarTransform.gameObject.SetActive(false);

        EnemyHealthBar healthBar = healthBarTransform.GetComponent<EnemyHealthBar>();
        healthBar.Setup(healthSystem);

        Debug.Log("Health: " + healthSystem.GetHealthPercent());
        Debug.Log("Health: " + healthSystem.GetCurrentHealth());
    }

    public void Die()
    {
        DropLoot();
        Destroy(gameObject);
    }

    void DropLoot()
    {
        Item item = DropTable.GetDrop();
        if (item != null)
        {
            Item_HandlePickUp instance = Instantiate(pickupItem, transform.position, Quaternion.identity);
            instance.ItemDrop = item;
        }
    }
}