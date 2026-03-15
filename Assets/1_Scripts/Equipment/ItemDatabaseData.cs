using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]
public class ItemDatabaseData : ScriptableObject
{
    public List<EquipmentData> allEquipments;

    public EquipmentData GetItemByID(string id)
    {
        return allEquipments.Find(x => x.id == id);
    }
}