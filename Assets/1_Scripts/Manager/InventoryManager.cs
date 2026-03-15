using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public ItemDatabaseData database;
    public List<string> ownedItemIDs = new List<string>(); // 소지 중인 아이템 ID
    public string equippedItemID = ""; // 현재 착용 중인 아이템 ID

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        LoadInventory();
    }

    // 장비 획득 시 호출
    public bool AddItem(EquipmentData item)
    {
        if (ownedItemIDs.Contains(item.id)) return false; // 이미 있다면 추가 안 함

        ownedItemIDs.Add(item.id);
        SaveInventory();
        return true;
    }

    // 장착 시 호출
    public void EquipItem(string id)
    {
        equippedItemID = id;
        SaveInventory();
    }

    // 착용 중인 장비의 공격력 보너스 합산 반환
    public int GetTotalAttackBonus()
    {
        if (string.IsNullOrEmpty(equippedItemID)) return 0;
        EquipmentData item = database.GetItemByID(equippedItemID);
        return item != null ? item.attackBonus : 0;
    }

    public void SaveInventory()
    {
        PlayerPrefs.SetString("OwnedItems", string.Join(",", ownedItemIDs));
        PlayerPrefs.SetString("EquippedItem", equippedItemID);
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        string savedItems = PlayerPrefs.GetString("OwnedItems", "");
        if (!string.IsNullOrEmpty(savedItems))
            ownedItemIDs = new List<string>(savedItems.Split(','));

        equippedItemID = PlayerPrefs.GetString("EquippedItem", "");
    }
}