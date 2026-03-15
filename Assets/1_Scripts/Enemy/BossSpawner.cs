using UnityEngine;

public class BossSpawner : EnemySpawner
{
    [Header("보스 프리팹 설정")]
    public GameObject bossPusherPrefab;
    public GameObject bossHealerPrefab;

    [Header("확률 설정")]
    [Range(0, 1)] public float spawnChance = 0.5f;

    protected override void SpawnEnemies()
    {
        // 일반몹 생성
        Debug.Log("일반몹 생성 시작");
        base.SpawnEnemies();

        // 보스 생성 확률 체크
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

    // 보스 전용 랜덤 함수
    private GameObject GetRandomBossPrefab()
    {
        return Random.value < 0.5f ? bossPusherPrefab : bossHealerPrefab;
    }
}