using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("스테이지 설정")]
    public int currentStage = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        currentStage = PlayerPrefs.GetInt("HighestStage", 1);
    }

    // 스테이지 클리어 시 호출 (EnemyTrigger 등에서 사용 가능)
    public void CompleteStage()
    {
        currentStage++;
        PlayerPrefs.SetInt("HighestStage", currentStage); // 최고 스테이지 저장
        Debug.Log($"스테이지 클리어! 현재 스테이지: {currentStage}");
    }
}