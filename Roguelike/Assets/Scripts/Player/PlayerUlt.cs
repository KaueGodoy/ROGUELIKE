using UnityEngine;

public class PlayerUlt : MonoBehaviour
{
    public static PlayerUlt Instance;

    private PlayerCooldowns _playerCooldowns;

    [Header("Ult")]
    [SerializeField] private Transform UltSpawnPoint;
    [SerializeField] private GameObject Projectile;
    [SerializeField] private ParticleSystem _ultParticle;

    [SerializeField] private float _ultAttackDelay = 0.8f;
    [SerializeField] private float _ultAttackTimer = 0.0f;

    private bool _isUltPerformed = false;

    public bool UltAttackRequest { get; set; }
    public static bool UltAttackAnimation { get; set; }

    [SerializeField] private float _ultBaseDamage = 1000f;
    [SerializeField] private float _ultMultiplier = 1f;
    public float UltBaseDamage { get { return _ultBaseDamage; } private set { } }
    public float UltMultiplier { get { return _ultMultiplier; } set { _ultMultiplier = value; } }
    public float FinalDamage { get { return UltBaseDamage * UltMultiplier; } private set { } }

    private void Awake()
    {
        Instance = this;

        _playerCooldowns = GetComponent<PlayerCooldowns>();
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerUlt += GameInput_OnPlayerUlt;
        UltAttackRequest = false;
    }

    private void GameInput_OnPlayerUlt(object sender, System.EventArgs e)
    {
        UltAttackRequest = true;
        PerformUlt();
    }

    private void Update()
    {
        UpdateUltTimer();
    }

    public void PerformUlt()
    {
        if (_playerCooldowns.ultOffCooldown)
        {
            if (UltAttackRequest)
            {
                UltAttackRequest = false;
                UltAttackAnimation = true;
                NewPlayerMovement.Instance.IsControlLocked = true;

                if (!_isUltPerformed)
                {
                    _isUltPerformed = true;

                    ParticleManager.Instance.InstantiateParticle(_ultParticle, transform.position);
                    Invoke("InstantiateUlt", _ultAttackDelay / 2);
                    Invoke("UltComplete", _ultAttackDelay);
                }
            }
        }
        else
        {
            UltAttackRequest = false;
        }
    }

    private void InstantiateUlt()
    {
        Instantiate(Projectile, UltSpawnPoint.position, UltSpawnPoint.rotation);
        AudioManager.Instance.PlaySound("Attack");
    }

    private void UltComplete()
    {
        _isUltPerformed = false;
        NewPlayerMovement.Instance.IsControlLocked = false;
    }

    public void UpdateUltTimer()
    {
        if (UltAttackAnimation)
            _ultAttackTimer += Time.deltaTime;

        if (_ultAttackTimer > _ultAttackDelay)
        {
            UltAttackAnimation = false;
            _ultAttackTimer = 0f;
        }
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerUlt -= GameInput_OnPlayerUlt;
    }
}
