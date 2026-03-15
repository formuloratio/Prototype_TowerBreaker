using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("아이템 표시 UI 그룹")]
    public GameObject itemDisplayGroup;    // 아이템이 있을 때 활성화될 부모 오브젝트
    //public GameObject emptyMessage;        // 아이템이 없을 때 보여줄 "장비 없음" 텍스트 (선택 사항)

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

    // 1. 인벤토리 상태 새로고침
    public void RefreshUI()
    {
        // InventoryManager에 저장된 아이템이 있는지 확인
        if (InventoryManager.Instance.ownedItemIDs.Count > 0)
        {
            // 첫 번째 아이템(단일 아이템) 정보 가져오기
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
            //if (emptyMessage != null) emptyMessage.SetActive(true);
        }
    }

    // 2. 아이템 정보창 활성화 및 데이터 매칭
    private void ShowItemInfo()
    {
        if (itemDisplayGroup != null) itemDisplayGroup.SetActive(true);
        //if (emptyMessage != null) emptyMessage.SetActive(false);

        itemIcon.sprite = currentItem.icon;
        itemNameText.text = currentItem.itemName;
        itemDescText.text = currentItem.description;

        UpdateConfirmButton();
    }

    // 3. 현재 장착 상태에 따른 버튼 및 텍스트 업데이트
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

    // 4. 장착/해제 버튼 클릭 이벤트 (버튼에 연결)
    public void OnClickEquipToggle()
    {
        if (currentItem == null) return;

        bool isEquipped = InventoryManager.Instance.equippedItemID == currentItem.id;

        if (isEquipped)
        {
            // 장착 해제: 빈 문자열을 저장 (InventoryManager 내부에서 PlayerPrefs 저장)
            InventoryManager.Instance.EquipItem("");
        }
        else
        {
            // 장착: 현재 아이템 ID 저장
            InventoryManager.Instance.EquipItem(currentItem.id);
        }

        // UI 상태 즉시 갱신
        UpdateConfirmButton();
    }
}