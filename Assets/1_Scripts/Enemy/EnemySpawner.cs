using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("등급별 적 프리팹")]
    public GameObject lv1Prefab;
    public GameObject lv2Prefab;
    public GameObject lv3Prefab;

    [Header("스폰 설정")]
    public float xInterval = 1.4f; // 적 간의 X축 간격

    public GameManager gameManagerObj;

    protected virtual void OnEnable()
    {
        // ⭐ 1. 기존에 생성되어 자식으로 붙어있던 적들을 모두 제거 (초기화)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // ⭐ 2. 현재 GameManager의 스테이지 값을 반영하여 새로 생성
        SpawnEnemies();
    }

    protected virtual void SpawnEnemies()
    {
        if (gameManagerObj == null)
        {
            Debug.LogError("일반몹 생성 실패: GameManager가 없습니다!");
            return;
        }

        int enemyCount = 6 + (gameManagerObj.currentStage - 1);
        Debug.Log($"[일반몹] 생성 시도 마릿수: {enemyCount} (현재 스테이지: {gameManagerObj.currentStage})");

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(i * xInterval, 0, 0);
            GameObject selectedPrefab = GetRandomEnemyPrefab();

            if (selectedPrefab != null)
            {
                GameObject obj = Instantiate(selectedPrefab, spawnPos, Quaternion.identity, transform);
                Debug.Log($"[일반몹] {i + 1}번째 적 생성 성공: {obj.name}");
            }
            else
            {
                Debug.LogError($"[일반몹] {i + 1}번째 생성 실패: 선택된 프리팹이 NULL입니다. 인스펙터를 확인하세요!");
            }
        }
    }

    protected virtual GameObject GetRandomEnemyPrefab()
    {
        float rand = Random.value;
        if (rand < 0.7f) return lv1Prefab;
        else if (rand < 0.9f) return lv2Prefab;
        else return lv3Prefab;
    }
}