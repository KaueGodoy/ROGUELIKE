using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerCooldowns : MonoBehaviour
{
    private PlayerAttack _playerAttack;
    private float fillAmountFull = 1f;
    private PlayerControls playerControls;

    Color ultCooldownColor = new Color32(164, 164, 164, 255);
    //Color ultDefaultColor = new Color32(255, 255, 255, 168);
    Color ultDefaultColor = new Color32(255, 150, 150, 168);

    private void Awake()
    {
        playerControls = new PlayerControls();
        _playerAttack = GetComponent<PlayerAttack>();
        offCooldown = true;

    }

    private void Start()
    {
        ResetCooldown();
        UltResetCooldown();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {

        if (playerControls.Player.Skill.triggered)
        {
            triggerCooldown = true;
        }


        if (playerControls.Player.Ult.triggered)
        {
            ultTriggerCooldown = true;
        }

    }

    private void FixedUpdate()
    {
        TriggerCooldown();
        UltTriggerCooldown();
    }

    #region Skill

    [Header("Skill")]
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Image foregroundImage;
    [SerializeField] private Image iconImage;

    public float cooldownTimer;
    public float cooldownValue = 5.00f;
    public bool triggerCooldown;
    public bool offCooldown = true;

    public float target;
    public float reduceSpeed = 2f;

    Color cooldownColor = new Color32(164, 164, 164, 255);
    Color defaultColor = new Color32(255, 255, 255, 168);

    public void TriggerCooldown()
    {

        if (triggerCooldown)
        {
            offCooldown = false;

            cooldownTimer -= Time.deltaTime;

            int numberOfDecimals = 1;

            float convertedCooldownTimer = Mathf.Round(cooldownTimer * Mathf.Pow(10, numberOfDecimals)) / Mathf.Pow(10, numberOfDecimals);

            iconImage.color = cooldownColor;

            //iconImage.gameObject.SetActive(false);
            //foregroundImage.gameObject.SetActive(false);
            foregroundImage.fillAmount = cooldownTimer / cooldownValue;

            cooldownText.gameObject.SetActive(true);
            cooldownText.text = convertedCooldownTimer + "";

            _playerAttack.attackRequest = false;

            if (cooldownTimer <= 0)
            {
                offCooldown = true;
                ResetCooldown();
            }

        }

    }

    public void ResetCooldown()
    {
        iconImage.color = defaultColor;
        iconImage.gameObject.SetActive(true);
        triggerCooldown = false;
        cooldownTimer = cooldownValue;
        target = cooldownTimer / cooldownValue;
        cooldownText.gameObject.SetActive(false);
        foregroundImage.fillAmount = fillAmountFull;
        foregroundImage.gameObject.SetActive(true);
    }

    #endregion

    #region Ult

    [Header("Ult")]
    [SerializeField] private TextMeshProUGUI ultCooldownText;
    [SerializeField] private Image ultForegroundImage;
    [SerializeField] private Image ultIconImage;

    public float ultCooldownTimer;
    public float ultCooldownValue = 10.0f;
    public bool ultTriggerCooldown;
    public bool ultOffCooldown = true;

    public float ultTarget;
    public float ultReduceSpeed = 2f;

    public void UltTriggerCooldown()
    {
        if (ultTriggerCooldown)
        {
            ultOffCooldown = false;

            ultCooldownTimer -= Time.deltaTime;

            int numberOfDecimals = 1;

            float convertedCooldownTimer = Mathf.Round(ultCooldownTimer * Mathf.Pow(10, numberOfDecimals)) / Mathf.Pow(10, numberOfDecimals);

            ultIconImage.color = ultCooldownColor;

            //iconImage.gameObject.SetActive(false);
            //foregroundImage.gameObject.SetActive(false);
            ultForegroundImage.fillAmount = ultCooldownTimer / ultCooldownValue;

            ultCooldownText.gameObject.SetActive(true);
            ultCooldownText.text = convertedCooldownTimer + "";

            if (ultCooldownTimer <= 0)
            {
                ultOffCooldown = true;
                UltResetCooldown();
            }
        }
    }

    public void UltResetCooldown()
    {
        ultIconImage.color = ultDefaultColor;
        ultIconImage.gameObject.SetActive(true);
        ultTriggerCooldown = false;
        ultCooldownTimer = ultCooldownValue;
        ultTarget = ultCooldownTimer / ultCooldownValue;
        ultCooldownText.gameObject.SetActive(false);
        ultForegroundImage.fillAmount = fillAmountFull;
        ultForegroundImage.gameObject.SetActive(true);
    }

    #endregion
}