using System.Collections;
using UnityEngine;

public enum BossType { Pusher, Healer }

public class BossEnemy : Enemy
{
    [Header("보스 설정")]
    public BossType bossType;
    protected BossData bossData; // 상속받은 데이터 캐스팅용
    private float attackTimer;

    private GameObject player; // 플레이어 참조
    private Rigidbody2D playerRb; // 플레이어 Rigidbody2D 참조

    protected override void Start()
    {
        base.Start(); // 기존 체력 설정 등 실행
        bossData = enemyData as BossData;
        attackTimer = bossData.attackInterval;

        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (currentState != EnemyState.Start) return;

        // 공격 타이머 작동
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            ExecuteBossSkill();
            attackTimer = bossData.attackInterval;
        }
    }

    private void ExecuteBossSkill()
    {
        anim.SetTrigger("2_Attack"); // 보스 공격 애니메이션

        if (bossType == BossType.Pusher)
            StartCoroutine(PusherSkill());
        else
            StartCoroutine(HealerSkill());
    }

    // --- 보스 1: 플레이어 밀치기 ---
    private IEnumerator PusherSkill()
    {
        yield return new WaitForSeconds(0.5f); // 애니메이션 싱크 대기

        if (player != null)
        {
            // 플레이어를 왼쪽으로 강하게 밀침
            playerRb.AddForce(Vector2.left * bossData.skillValue, ForceMode2D.Impulse);
            Debug.Log("보스가 플레이어를 밀쳐냈습니다!");
        }
    }

    // --- 보스 2: 아군 힐 ---
    private IEnumerator HealerSkill()
    {
        yield return new WaitForSeconds(0.5f);

        if (myTrigger != null)
        {
            int healAmount = (int)bossData.skillValue;
            Debug.Log($"<color=green>보스가 아군을 {healAmount}만큼 치유합니다!</color>");

            foreach (var ally in myTrigger.activeEnemies)
            {
                // 나 자신을 제외하고 살아있는 아군만 체크
                if (ally != null && ally != this && ally.currentState != EnemyState.Dead)
                {
                    // ⭐ [핵심 로직] 현재 체력 + 힐량과 최대 체력 중 더 작은 값을 선택
                    // 이렇게 하면 최대 체력을 절대 넘지 않습니다.
                    int maxHP = ally.enemyData.maxHP;
                    ally.currentHP = Mathf.Min(ally.currentHP + healAmount, maxHP);

                    Debug.Log($"{ally.name} 치유됨 (현재 체력: {ally.currentHP}/{maxHP})");
                }
            }
        }
    }

    protected override void Die()
    {
        // 상자 드롭 로직 추가
        if (bossData.chestPrefab != null)
        {
            Instantiate(bossData.chestPrefab, transform.position, Quaternion.identity);
            Debug.Log("보스 상자 드롭!");
        }

        base.Die(); // 기존 죽음 로직(애니메이션, 파괴) 실행
    }
}