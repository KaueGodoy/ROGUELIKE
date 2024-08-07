using UnityEngine;

public class EnemyAttackOnAnimationEvent : MonoBehaviour
{
    private NewPlayerController _player;
    private Transform _target;

    [Header("Attack")]

    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private float _damage = 20f;
    //[SerializeField] private float attackDuration = 0.9f;
    //[SerializeField] private float attackRate = 0.9f;

    public float AttackRange { get { return _attackRange; } set { _attackRange = value; } }
    public float Damage { get { return _damage; } set { _damage = value; } }

    private void Start()
    {
        _player = NewPlayerController.Instance;
        _target = _player.transform;
    }

    private void Update()
    {
        float distance = Vector2.Distance(transform.position, _target.position);

        if (distance < AttackRange)
        {
            TriggerAttackAnimation();
        }
    }

    public virtual void TriggerAttackAnimation()
    {

    }

    public void AttackPlayer()
    {
        NewPlayerController.Instance.TakeDamage(Damage);
    }
}
