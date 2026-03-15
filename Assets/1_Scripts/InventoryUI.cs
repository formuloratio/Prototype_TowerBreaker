using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("아이템 표시 UI 그룹")]
    public GameObject itemDisplayGroup;

    [Header("정보 출력 UI")]
    public Image itemIcon;                 // 장비 아이콘
    public TextMeshProUGUI itemNameText;   // 장비 이름
    public TextMeshProUGUI itemDescText;   // 장비 설명

    [Header("버튼 제어")]
    public Button equipButton;             // 장착 버튼
    public TextMeshProUGUI buttonText;     // 버튼 텍스트 (장착/해제)

    private EquipmentData currentItem;

    private void Start()
    {
        // 시작 시 데이터 로드 및 UI 갱신
        RefreshUI();
    }

    //인벤토리 상태 새로고침
    public void RefreshUI()
    {
        // InventoryManager에 저장된 아이템이 있는지 확인
        if (InventoryManager.Instance.ownedItemIDs.Count > 0)
        {
            // 정보 가져오기
            string id = InventoryManager.Instance.ownedItemIDs[0];
            EquipmentData data = InventoryManager.Instance.database.GetItemByID(id);

            if (data != null)
            {
                currentItem = data;
                ShowItemInfo();
            }
        }
        else
        {
            // 아이템이 하나도 없는 경우
            currentItem = null;
            if (itemDisplayGroup != null) itemDisplayGroup.SetActive(false);
        }
    }

    // 아이템 정보창 활성화 및 데이터 매칭
    private void ShowItemInfo()
    {
        if (itemDisplayGroup != null) itemDisplayGroup.SetActive(true);

        itemIcon.sprite = currentItem.icon;
        itemNameText.text = currentItem.itemName;
        itemDescText.text = currentItem.description;

        UpdateConfirmButton();
    }

    // 현재 장착 상태에 따른 버튼 및 텍스트 업데이트
    private void UpdateConfirmButton()
    {
        if (currentItem == null) return;

        // PlayerPrefs 데이터를 참조하는 InventoryManager에서 현재 장착 ID 확인
        bool isEquipped = InventoryManager.Instance.equippedItemID == currentItem.id;

        if (isEquipped)
        {
            buttonText.text = "해제하기";
            itemNameText.text = $"{currentItem.itemName} <color=green>(장착 중)</color>";
        }
        else
        {
            buttonText.text = "장착하기";
            itemNameText.text = currentItem.itemName;
        }
    }

    // 장착/해제 버튼
    public void OnClickEquipToggle()
    {
        if (currentItem == null) return;

        bool isEquipped = InventoryManager.Instance.equippedItemID == currentItem.id;

        if (isEquipped)
        {
            // 장착 해제
            InventoryManager.Instance.EquipItem("");
        }
        else
        {
            // 장착
            InventoryManager.Instance.EquipItem(currentItem.id);
        }

        // UI 상태 즉시 갱신
        UpdateConfirmButton();
    }
}