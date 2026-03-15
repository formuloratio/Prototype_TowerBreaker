using UnityEngine;
using System.Collections.Generic;

public class ChestManager : MonoBehaviour
{
    public ItemDatabaseData database;

    public void OnClickOpenChest()
    {
        if (GameManager.Instance.totalChests <= 0)
        {
            Debug.Log("상자가 부족합니다!");
            return;
        }

        GameManager.Instance.totalChests--;
        PlayerPrefs.SetInt("SavedChestCount", GameManager.Instance.totalChests);

        // 1. 50% 확률 체크
        if (Random.value > 0.5f)
        {
            TryGiveEquipment();
        }
        else
        {
            Debug.Log("꽝! 다음 기회에...");
        }
    }

    private void TryGiveEquipment()
    {
        // 아직 얻지 못한 아이템들만 필터링
        List<EquipmentData> availableItems = database.allEquipments.FindAll(
            x => !InventoryManager.Instance.ownedItemIDs.Contains(x.id)
        );

        if (availableItems.Count > 0)
        {
            EquipmentData reward = availableItems[Random.Range(0, availableItems.Count)];
            InventoryManager.Instance.AddItem(reward);
            Debug.Log($"축하합니다! [{reward.itemName}]을(를) 획득했습니다.");
        }
        else
        {
            Debug.Log("모든 장비를 이미 소지하고 있습니다! (대체 보상 지급 로직 추가 가능)");
        }
    }
}