using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public ItemDatabaseData database;
    public List<string> ownedItemIDs = new List<string>(); // 소지 중인 아이템 ID
    public string equippedItemID = ""; // 현재 장착 중인 아이템 ID

    private void Awake()
    {
        // DontDestroyOnLoad를 제거했습니다. 씬마다 새로 생성되지만 데이터는 불러옵니다.
        Instance = this;
        LoadInventory();
    }

    // 장비 획득 시 호출
    public bool AddItem(EquipmentData item)
    {
        if (ownedItemIDs.Contains(item.id)) return false;

        ownedItemIDs.Add(item.id);
        SaveInventory(); // 즉시 저장
        return true;
    }

    // 장착 시 호출
    public void EquipItem(string id)
    {
        equippedItemID = id;
        SaveInventory(); // 즉시 저장
    }

    // 장착 해제 시 호출
    public void UnEquip()
    {
        equippedItemID = "";
        SaveInventory();
    }

    public int GetTotalAttackBonus()
    {
        if (string.IsNullOrEmpty(equippedItemID)) return 0;
        EquipmentData item = database.GetItemByID(equippedItemID);
        return item != null ? item.attackBonus : 0;
    }

    // ⭐ PlayerPrefs 저장 로직
    public void SaveInventory()
    {
        // 리스트를 "ID1,ID2" 형태의 문자열로 변환하여 저장
        string joinedIDs = string.Join(",", ownedItemIDs);
        PlayerPrefs.SetString("OwnedItems", joinedIDs);
        PlayerPrefs.SetString("EquippedItem", equippedItemID);
        PlayerPrefs.Save(); // 디스크에 즉시 물리적 저장
        Debug.Log("인벤토리 데이터가 PlayerPrefs에 저장되었습니다.");
    }

    // ⭐ PlayerPrefs 불러오기 로직
    private void LoadInventory()
    {
        string savedItems = PlayerPrefs.GetString("OwnedItems", "");
        if (!string.IsNullOrEmpty(savedItems))
        {
            ownedItemIDs = new List<string>(savedItems.Split(','));
        }

        equippedItemID = PlayerPrefs.GetString("EquippedItem", "");
        Debug.Log($"데이터 로드 완료: 보유 {ownedItemIDs.Count}개, 장착 중: {equippedItemID}");
    }
}