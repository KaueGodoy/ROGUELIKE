using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    private Animator animator;
    public List<BaseStat> Stats { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PerformAttack()
    {
        animator.SetTrigger("Base_Attack");
        Debug.Log(this.name + " basic attack!");
    }

    public void PerformSkillAttack()
    {
        animator.SetTrigger("Skill_Attack");
        Debug.Log(this.name + " skill attack!");
    }

    public void PerformUltAttack()
    {
        animator.SetTrigger("Ult_Attack");
        Debug.Log(this.name + " ult attack!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<IEnemy>().TakeDamage(Stats[0].GetCalculatedStatValue());
            Debug.Log("Hit: " + collision.name);
        }

    }
}
