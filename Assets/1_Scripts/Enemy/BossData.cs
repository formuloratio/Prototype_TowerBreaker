using UnityEngine;

[CreateAssetMenu(fileName = "NewBossData", menuName = "Enemy/BossData")]
public class BossData : EnemyData
{
    [Header("보스 전용 설정")]
    public float attackInterval = 3f; // 공격 주기
    public float skillValue = 10f;    // 보스1: 밀치기 힘 / 보스2: 힐량
    public GameObject chestPrefab;    // 사망 시 드롭할 상자 프리팹
}