using System.Collections;
using UnityEngine;

public enum BossType { Pusher, Healer }

public class BossEnemy : Enemy
{
    public AudioClip pushSfx;
    public AudioClip healSfx;
    public AudioClip chestDropSfx;

    [Header("보스 설정")]
    public BossType bossType;
    protected BossData bossData;
    private float attackTimer;

    private GameObject player;
    private Rigidbody2D playerRb;

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
            SoundEvents.NotifySfx(pushSfx);
            // 플레이어를 왼쪽으로 강하게 밀침
            playerRb.AddForce(Vector2.left * bossData.skillValue, ForceMode2D.Impulse);
            Debug.Log("보스가 플레이어를 밀쳐냈습니다!");
        }
    }

    // --- 보스 2: 아군 힐 ---
    private IEnumerator HealerSkill()
    {
        yield return new WaitForSeconds(0.5f);
        SoundEvents.NotifySfx(healSfx);

        if (myTrigger != null)
        {
            int healAmount = (int)bossData.skillValue;
            Debug.Log($"<color=green>보스가 아군을 {healAmount}만큼 치유합니다!</color>");

            foreach (var ally in myTrigger.activeEnemies)
            {
                // 나 자신을 제외하고 살아있는 아군만 체크
                if (ally != null && ally != this && ally.currentState != EnemyState.Dead)
                {
                    // 최대 체력 넘지 않게
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
            GameObject obj = Instantiate(bossData.chestPrefab, transform.position, Quaternion.identity);
            DropChest chest = obj.GetComponent<DropChest>();

            if (myTrigger != null && chest != null)
            {
                myTrigger.SetChest(chest); // 트리거에 상자 등록
            }
        }

        SoundEvents.NotifySfx(chestDropSfx);
        base.Die(); // 기존 죽음 로직 실행
    }
}