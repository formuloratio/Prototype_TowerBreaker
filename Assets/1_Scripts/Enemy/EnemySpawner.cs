using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("등급별 적 프리팹")]
    public GameObject lv1Prefab;
    public GameObject lv2Prefab;
    public GameObject lv3Prefab;

    [Header("스폰 설정")]
    public float xInterval = 1.4f; // 적 간의 X축 간격
    public int maxEnemyCount = 20; // 최대 스폰 제한 수치

    public GameManager gameManagerObj;

    protected virtual void OnEnable()
    {
        // 초기화
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        SpawnEnemies();
    }

    protected virtual void SpawnEnemies()
    {
        if (gameManagerObj == null)
        {
            Debug.LogError("일반몹 생성 실패: GameManager가 없습니다!");
            return;
        }

        // 스테이지에 따른 마릿수 계산 후 최대치로 제한
        int calculatedCount = 6 + (gameManagerObj.currentStage - 1);
        int enemyCount = Mathf.Min(calculatedCount, maxEnemyCount);

        Debug.Log($"[일반몹] 생성 시도 마릿수: {enemyCount} (계산값: {calculatedCount}, 최대제한: {maxEnemyCount})");

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
                Debug.LogError($"[일반몹] {i + 1}번째 생성 실패: 선택된 프리팹이 NULL입니다.");
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