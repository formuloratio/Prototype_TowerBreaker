using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStateEnum
{
    Idle,
    Moving,
    Attacking,
    Defending,
    UsingSkill
}

public class PlayerController : MonoBehaviour
{
    [Header("컴포넌트")]
    public Animator anim;
    public Rigidbody2D rb;
    public Collider2D playerCollider;

    [Header("상태")]
    public PlayerStateEnum currentState = PlayerStateEnum.Idle;

    [Header("움직임 세팅")]
    public float moveDistance = 10f; // 이동 거리
    public float moveCooldown = 1f;  // 이동 쿨타임
    public float dashSpeed = 30f;    // 이동 속도
    public LayerMask enemyLayer;     // 적 레이어
    private float nextMoveTime = 0f; // ⭐ 이동 쿨타임 추적용 변수

    [Header("전투 세팅")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public int attackDamage = 10;

    public float attackCooldown = 0.3f; // ⭐ 공격 쿨타임 추가
    private float nextAttackTime = 0f;  // ⭐ 공격 쿨타임 추적용 변수

    public float defendCooldown = 1f; // 방어 쿨타임
    private float nextDefendTime = 0f; // ⭐ 방어 쿨타임 추적용 변수
    private float knockbackForce = 30f;
    public float playerRecoilForce = 15f; // ⭐ 플레이어 반동 힘 추가

    [Header("외부 참조 (자동 할당)")]
    public EnemyTrigger currentEnemyTrigger; // 현재 플레이어가 딛고 있는 트리거

    // ==========================================
    // 1. 기본 버튼 기능 
    // ==========================================

    public void OnClickMove()
    {
        // 이동 쿨타임이 안 지났으면 무시
        if (Time.time < nextMoveTime)
        {
            Debug.Log("이동 쿨타임 중입니다!");
            return;
        }

        // ⭐ 2. 현재 적과 닿아있는지 bool 값으로 확인
        bool isTouchingEnemy = playerCollider.IsTouchingLayers(enemyLayer);

        // 닿아있다면(true) 이동 취소
        if (isTouchingEnemy)
        {
            Debug.Log("적과 맞닿아 있어 이동할 수 없습니다!");
            return;
        }

        // 3. 위 조건을 모두 통과했을 때만 이동 실행
        anim.Play("Move");
        StartCoroutine(MoveRoutine());
    }

    public void OnClickDefend()
    {
        // 1. 상태 및 쿨타임 체크
        if (currentState != PlayerStateEnum.Idle) return;
        if (Time.time < nextDefendTime)
        {
            Debug.Log("방어 쿨타임 중입니다!");
            return;
        }

        // ⭐ [핵심 조건] 적 레이어와 하나라도 닿아있을 때만 방어 버튼 활성화
        // 몸에 적이 붙어있지 않으면 방어 자세를 취하지 않습니다.
        bool isTouchingAnyEnemy = playerCollider.IsTouchingLayers(enemyLayer);

        if (!isTouchingAnyEnemy)
        {
            Debug.Log("주변에 적이 없어 방어가 발동되지 않습니다!");
            return;
        }

        // 조건을 통과하면 방어 실행
        StartCoroutine(DefendRoutine());
    }

    public void OnClickAttack()
    {
        // 공격 쿨타임이 안 지났으면 무시
        if (Time.time < nextAttackTime)
        {
            Debug.Log("공격 쿨타임 중입니다!");
            return;
        }

        // 공격 코루틴 시작
        StartCoroutine(AttackRoutine());
    }

    // ==========================================
    // 2. 스킬 버튼 기능
    // ==========================================

    public void OnClickSkill1() { if (currentState != PlayerStateEnum.Idle) return; Debug.Log("스킬 1 사용!"); }
    public void OnClickSkill2() { if (currentState != PlayerStateEnum.Idle) return; Debug.Log("스킬 2 사용!"); }
    public void OnClickSkill3() { if (currentState != PlayerStateEnum.Idle) return; Debug.Log("스킬 3 사용!"); }

    // ==========================================
    // 내부 로직 (코루틴)
    // ==========================================

    private IEnumerator MoveRoutine()
    {
        currentState = PlayerStateEnum.Moving;
        nextMoveTime = Time.time + moveCooldown;

        Vector2 startPos = rb.position;
        Vector2 direction = Vector2.right;

        // 1. 출발 전 거리를 계산 (적이 가만히 있을 때를 대비한 1차 안전장치)
        RaycastHit2D hit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, direction, moveDistance, enemyLayer);
        Vector2 targetPos = startPos + (direction * moveDistance);

        if (hit.collider != null)
        {
            float safeDistance = Mathf.Max(0f, hit.distance);
            targetPos = startPos + (direction * safeDistance);
        }

        // 2. 이동 시작
        while (Vector2.Distance(rb.position, targetPos) > 0.01f)
        {
            // ⭐ [핵심 추가] 이동 도중 다가오는 적과 부딪혔다면 즉시 루틴 탈출(break)
            if (playerCollider.IsTouchingLayers(enemyLayer))
            {
                break;
            }

            rb.MovePosition(Vector2.MoveTowards(rb.position, targetPos, dashSpeed * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }

        // 3. 마지막 위치 보정 
        // (적과 부딪혀서 강제로 멈춘 것이 아닐 때만 목표 위치에 딱 맞춰줌)
        if (!playerCollider.IsTouchingLayers(enemyLayer))
        {
            rb.MovePosition(targetPos);
        }

        // 4. 상태 및 애니메이션 초기화
        currentState = PlayerStateEnum.Idle;
        anim.Play("Idle");
    }

    // --- 유니티 충돌 이벤트 추가 ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 오브젝트에 EnemyTrigger 스크립트가 있다면 참조 저장
        if (collision.CompareTag("EnemyTriggerTag")) // 트리거 오브젝트에 전용 태그를 설정하면 좋습니다.
        {
            currentEnemyTrigger = collision.GetComponent<EnemyTrigger>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 트리거 구역을 벗어날 때 참조 해제
        if (currentEnemyTrigger != null && collision.gameObject == currentEnemyTrigger.gameObject)
        {
            currentEnemyTrigger = null;
        }
    }

    private IEnumerator DefendRoutine()
    {
        currentState = PlayerStateEnum.Defending;
        nextDefendTime = Time.time + defendCooldown;

        anim.Play("Defend");

        // ⭐ 해당 트리거 구역 내의 모든 적을 한꺼번에 넉백
        if (currentEnemyTrigger != null && currentEnemyTrigger.activeEnemies.Count > 0)
        {
            for (int i = currentEnemyTrigger.activeEnemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = currentEnemyTrigger.activeEnemies[i];

                if (enemy == null)
                {
                    currentEnemyTrigger.activeEnemies.RemoveAt(i);
                    continue;
                }

                // 닿아있지 않은 적들도 리스트에 있다면 모두 밀쳐냅니다.
                enemy.ApplyKnockback(knockbackForce);
            }

            // 모든 적을 밀어냈으므로 플레이어 반동과 화면 흔들림 실행
            rb.AddForce(Vector2.left * playerRecoilForce, ForceMode2D.Impulse);

            if (AttackEffectsManager.Instance != null)
            {
                AttackEffectsManager.Instance.PlayScreenShake(0.1f, 0.1f);
            }
        }

        yield return new WaitForSeconds(0.5f);
        currentState = PlayerStateEnum.Idle;
    }

    private IEnumerator AttackRoutine()
    {
        currentState = PlayerStateEnum.Attacking;
        nextAttackTime = Time.time + attackCooldown;

        anim.Play("Attack");

        int finalDamage = attackDamage + InventoryManager.Instance.GetTotalAttackBonus();

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // 한 번이라도 적을 맞췄는지 체크 (중복 흔들림 방지)
        if (hitObjects.Length > 0 && AttackEffectsManager.Instance != null)
        {
            // ⭐ 화면만 0.1초 동안 흔듭니다.
            AttackEffectsManager.Instance.PlayScreenShake(0.1f, 0.15f);
        }

        foreach (Collider2D col in hitObjects)
        {
            // 1. 적(Enemy)인 경우
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(finalDamage);
            }

            // 2. 상자(DropChest)인 경우 ⭐ 추가됨
            DropChest chest = col.GetComponent<DropChest>();
            if (chest != null)
            {
                chest.TakeDamage(finalDamage);
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        currentState = PlayerStateEnum.Idle;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}