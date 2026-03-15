using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ChestManager : MonoBehaviour
{
    public ItemDatabaseData database;
    public InventoryUI inventoryUI;

    [Header("UI 연결")]
    public TextMeshProUGUI chestCountText;
    public TextMeshProUGUI resultText;
    public CanvasGroup resultCanvasGroup;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        UpdateChestUI();
        if (resultCanvasGroup != null) resultCanvasGroup.alpha = 0;
    }

    public void OnClickOpenChest()
    {
        if (GameManager.Instance.totalChests <= 0)
        {
            ShowResultMessage("상자가 부족합니다!", Color.yellow);
            return;
        }

        // ⭐ GameManager를 통해 차감 및 자동 저장
        GameManager.Instance.UpdateChestCount(-1);
        UpdateChestUI();

        if (Random.value > 0.5f)
        {
            TryGiveEquipment();
        }
        else
        {
            ShowResultMessage("꽝! 다음 기회에...", Color.red);
        }
    }

    private void TryGiveEquipment()
    {
        List<EquipmentData> availableItems = database.allEquipments.FindAll(
            x => !InventoryManager.Instance.ownedItemIDs.Contains(x.id)
        );

        if (availableItems.Count > 0)
        {
            EquipmentData reward = availableItems[Random.Range(0, availableItems.Count)];

            // ⭐ AddItem 내부에서 자동으로 PlayerPrefs 저장됨
            InventoryManager.Instance.AddItem(reward);

            if (inventoryUI != null)
            {
                inventoryUI.RefreshUI();
            }

            ShowResultMessage($"[{reward.itemName}] 획득!", Color.green);
        }
        else
        {
            ShowResultMessage("모든 장비를 다 모았습니다!", Color.cyan);
        }
    }

    private void UpdateChestUI()
    {
        if (chestCountText != null)
        {
            chestCountText.text = $"보유 상자: {GameManager.Instance.totalChests}개";
        }
    }

    private void ShowResultMessage(string message, Color textColor)
    {
        if (resultText == null || resultCanvasGroup == null) return;

        resultText.text = message;
        resultText.color = textColor;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeInAndOutRoutine());
    }

    private IEnumerator FadeInAndOutRoutine()
    {
        float duration = 0.3f;
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            resultCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / duration);
            yield return null;
        }
        resultCanvasGroup.alpha = 1;

        yield return new WaitForSeconds(1.0f);

        timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            resultCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / duration);
            yield return null;
        }
        resultCanvasGroup.alpha = 0;
    }
}