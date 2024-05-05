using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Chest_DropItemToGround : Chest
{
    [Header("Drop")]
    [SerializeField] private Item_HandlePickUp _pickupItem;
    public DropTable DropTable { get; set; }

    private string _chestKey = "CommonChestKey";

    public override void Awake()
    {
        base.Awake();

        LocalizationSettings.StringDatabase.GetLocalizedStringAsync(_chestKey).Completed += handle =>
        {
            string localizedMessage = handle.Result;

            SetName(localizedMessage);
        };

        CreateLoot();
    }

    private void CreateLoot()
    {
        DropTable = new DropTable();
        DropTable.loot = new List<LootDrop>
        {
            new LootDrop("sword", 100),
        };
    }

    private void DropLoot()
    {
        Item item = DropTable.GetDrop();
        if (item != null)
        {
            Item_HandlePickUp instance = Instantiate(_pickupItem, transform.position, Quaternion.identity);
            instance.ItemDrop = item;
        }
    }

    public override void OpenChest()
    {
        base.OpenChest();
        DropLoot();
    }
}
