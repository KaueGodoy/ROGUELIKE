using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerHealthBar _healthBar;

    private AudioManager _audioManager;

    [Header("Health")]
    [SerializeField] private float _currentHealth = 0;
    [SerializeField] private float _maxHealth = 3;

    public static bool IsAlive { get; set; }
    public float MaxHealth { get { return _maxHealth; } }
    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }

    [Header("Hit")]
    public float HitCooldown = 0.3f;
    public float HitTimer = 0.0f;
    public static bool IsHit = false;
    private readonly float deathAnimationTime = 0.8f;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        IsAlive = true;
    }

    public void UpdatePlayerHealthBar()
    {
        _healthBar.UpdateHealthBar(MaxHealth, CurrentHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        Debug.Log($"Player takes: {damageAmount} damage");

        _audioManager.PlaySound("Hit");
        CurrentHealth -= Mathf.FloorToInt(damageAmount);
        IsHit = true;

        //UIEventHandler.HealthChanged(this.currentHealth, this.maxHealth);

        if (CurrentHealth <= 0)
        {
            UpdatePlayerHealthBar();
            Die();
            //UIEventHandler.HealthChanged(0, this.maxHealth);
            Invoke("RestartLevel", deathAnimationTime);
        }
    }

    public void Heal(float healAmount)
    {
        _audioManager.PlaySound("Hit");
        CurrentHealth += Mathf.FloorToInt(healAmount);

        if (CurrentHealth >= MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }

    public void UpdateHitTimer()
    {
        if (IsHit)
            HitTimer += Time.deltaTime;

        if (HitTimer > HitCooldown)
        {
            IsHit = false;
            HitTimer = 0f;
        }
    }

    #region Level

    private void RestartLevel()
    {
        IsAlive = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Die()
    {
        _audioManager.PlaySound("GameOver");
        IsAlive = false;
        //_rb.bodyType = RigidbodyType2D.Static;
    }
    #endregion 

}
