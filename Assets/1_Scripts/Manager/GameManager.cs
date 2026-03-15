using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int totalChests;

    [Header("스테이지 설정")]
    public int currentStage = 1;

    private void Awake()
    {
        Instance = this;
        LoadGameData();
    }

    private void LoadGameData()
    {
        currentStage = PlayerPrefs.GetInt("HighestStage", 1);
        totalChests = PlayerPrefs.GetInt("SavedChestCount", 0);
    }

    public void CompleteStage()
    {
        currentStage++;
        PlayerPrefs.SetInt("HighestStage", currentStage);
        PlayerPrefs.Save();
        Debug.Log($"스테이지 저장 완료: {currentStage}");
    }

    // 상자 개수 변경 시 공용으로 사용할 함수
    public void UpdateChestCount(int amount)
    {
        totalChests += amount;
        PlayerPrefs.SetInt("SavedChestCount", totalChests);
        PlayerPrefs.Save();
    }
}