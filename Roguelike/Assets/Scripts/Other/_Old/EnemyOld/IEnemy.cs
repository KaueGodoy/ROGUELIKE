public interface IEnemy
{
    int ID { get; set; }
    void Die();
    void TakeDamage(float damage);
    void PerformAttack();
    bool IsDead { get; set; }
}
