using UnityEngine;

public class BossSpawner : EnemySpawner
{
    [Header("보스 프리팹 설정")]
    public GameObject bossPusherPrefab;
    public GameObject bossHealerPrefab;

    [Header("확률 설정")]
    [Range(0, 1)] public float spawnChance = 0.5f;

    // 부모의 OnEnable을 그대로 사용하므로 따로 적지 않아도 
    // "기존 자식 삭제 -> SpawnEnemies 호출"이 자동으로 실행됩니다.

    protected override void SpawnEnemies()
    {
        // ⭐ 1. 부모 로직 실행 (일반몹 생성)
        // 여기서 일반몹이 안 나온다면 부모의 프리팹 슬롯이 비었거나 GameManager.Instance가 null인 것임
        Debug.Log("일반몹 생성 시작");
        base.SpawnEnemies();

        // 2. 보스 생성 확률 체크
        float roll = Random.value;
        Debug.Log($"보스 확률 주사위: {roll} (필요값: {spawnChance} 이하)");

        if (roll > spawnChance)
        {
            Debug.Log("이번 층 보스 생성 실패 (확률)");
            return;
        }

        GameObject selectedBoss = GetRandomBossPrefab();

        if (selectedBoss != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(4f, 0, 0);
            Instantiate(selectedBoss, spawnPos, Quaternion.identity, transform);
            Debug.Log($"보스 [{selectedBoss.name}] 생성 완료");
        }
        else
        {
            Debug.LogError("보스 프리팹이 할당되지 않았습니다!");
        }
    }

    // 보스 전용 랜덤 함수 (일반몹용 GetRandomEnemyPrefab과 이름이 겹치지 않게 하거나 override)
    private GameObject GetRandomBossPrefab()
    {
        return Random.value < 0.5f ? bossPusherPrefab : bossHealerPrefab;
    }
}