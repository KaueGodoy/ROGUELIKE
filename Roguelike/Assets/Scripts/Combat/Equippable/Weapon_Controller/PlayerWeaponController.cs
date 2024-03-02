using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public GameObject playerHand;
    public GameObject EquippedWeapon { get; set; }

    public CharacterPanel characterPanel;

    Transform spawnProjectile;
    CharacterStats characterStats;
    Item currentlyEquippedItem;
    IWeapon weaponEquipped;

    private void Start()
    {
        spawnProjectile = transform.Find("ProjectileSpawn");
        characterStats = GetComponent<PlayerController>().characterStats;

        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
    }

    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e)
    {
        PerformWeaponAttack();
    }

    public void EquipWeapon(Item itemToEquip)
    {
        if (EquippedWeapon != null)
            UnequipWeapon();

        EquippedWeapon = Instantiate(Resources.Load<GameObject>("Weapons/" + itemToEquip.ObjectSlug),
       playerHand.transform.position, playerHand.transform.rotation);

        weaponEquipped = EquippedWeapon.GetComponent<IWeapon>();

        if (EquippedWeapon.GetComponent<IProjectileWeapon>() != null)
        {
            EquippedWeapon.GetComponent<IProjectileWeapon>().ProjectileSpawn = spawnProjectile;
        }

        EquippedWeapon.transform.SetParent(playerHand.transform);

        weaponEquipped.Stats = itemToEquip.Stats;
        currentlyEquippedItem = itemToEquip;

        characterStats.AddStatBonus(itemToEquip.Stats);

        UIEventHandler.ItemEquipped(itemToEquip);
        UIEventHandler.StatsChanged();

        Debug.Log(weaponEquipped.Stats[0].GetCalculatedStatValue());

    }

    public void UnequipWeapon()
    {
        if (EquippedWeapon != null)
        {
            InventoryController.Instance.GiveItem(currentlyEquippedItem.ObjectSlug);
            characterStats.RemoveStatBonus(weaponEquipped.Stats);
            characterPanel.UnequipWeapon();
            UIEventHandler.StatsChanged();
            Destroy(EquippedWeapon.transform.gameObject);
        }
    }

    public void PerformWeaponAttack()
    {
        if (EquippedWeapon != null)
        {
            weaponEquipped.PerformAttack(CalculateDamage());
        }
    }

    private float CalculateDamage()
    {
        float baseDamage = (characterStats.GetStat(BaseStat.BaseStatType.Attack).GetCalculatedStatValue())
                             * (1 + (characterStats.GetStat(BaseStat.BaseStatType.AttackBonus).GetCalculatedStatValue() / 100))
                             + (characterStats.GetStat(BaseStat.BaseStatType.FlatAttack).GetCalculatedStatValue());

        float damageToDeal = baseDamage * (1 + characterStats.GetStat(BaseStat.BaseStatType.DamageBonus).GetCalculatedStatValue() / 100);

        damageToDeal += CalculateCrit(damageToDeal);
        Debug.Log("Damage dealt: " + damageToDeal);
        return damageToDeal;
    }

    private float CalculateCrit(float damage)
    {
        if (Random.value <= (characterStats.GetStat(BaseStat.BaseStatType.CritRate).GetCalculatedStatValue() / 100))
        {
            float critDamage = (damage * ((characterStats.GetStat(BaseStat.BaseStatType.CritDamage).GetCalculatedStatValue()) / 100));
            return critDamage;
        }
        return 0;
    }

    public void PerformWeaponSkillAttack()
    {
        weaponEquipped.PerformSkillAttack();
    }

    public void PerformWeaponUltAttack()
    {
        weaponEquipped.PerformUltAttack();
    }

}
