using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Inventory/Equipment")]
public class EquipmentData : ScriptableObject
{
    public string id;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("능력치")]
    public int attackBonus; // 착용 시 증가할 공격력
}