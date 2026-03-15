using System.Collections;
using UnityEngine;

public enum EnemyState { Idle, Start, Dead }

public class Enemy : MonoBehaviour
{
    [Header("적 데이터 설정")]
    public EnemyData enemyData;
    public EnemyState currentState = EnemyState.Idle;

    public Animator anim;
    protected EnemyTrigger myTrigger; // ⭐ 나를 관리하는 트리거 저장

    [HideInInspector] public int currentHP;
    private Rigidbody2D rb;
    private float currentSpeed;
    private bool isKnockbacked = false; // ⭐ 스스로 상태를 제어하기 위한 변수

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (enemyData != null)
        {
            currentHP = enemyData.maxHP;
            currentSpeed = enemyData.initialSpeed;
        }
    }

    protected virtual void FixedUpdate()
    {
        // ⭐ 넉백 중이 아닐 때만 이동 로직 수행
        if (currentState == EnemyState.Start && !isKnockbacked)
        {
            MoveLeftWithAcceleration();
        }
    }

    private void MoveLeftWithAcceleration()
    {
        currentSpeed += enemyData.acceleration * Time.fixedDeltaTime;
        if (currentSpeed > enemyData.maxSpeed) currentSpeed = enemyData.maxSpeed;
        rb.velocity = new Vector2(-currentSpeed, rb.velocity.y);
    }

    // ⭐ 플레이어로부터 "밀려나라"는 요청을 받으면 실행됨
    public void ApplyKnockback(float force)
    {
        if (currentState == EnemyState.Dead) return;

        // 1. 기존 이동 루틴 및 속도 초기화
        StopAllCoroutines();
        rb.velocity = Vector2.zero;
        currentSpeed = enemyData.initialSpeed;

        // 2. 넉백 물리 적용 (스스로에게 힘을 가함)
        rb.AddForce(Vector2.right * force, ForceMode2D.Impulse);

        // 3. 넉백 제어 코루틴 시작
        StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnockbacked = true; // 이동 로직 잠금

        // 0.2초간 물리 엔진이 AddForce의 결과를 온전히 처리하도록 대기
        yield return new WaitForSeconds(0.2f);

        isKnockbacked = false; // 이동 로직 해제
    }

    public void ActivateEnemy(EnemyTrigger trigger)
    {
        myTrigger = trigger; // 관리자 등록
        if (currentState == EnemyState.Idle) StartCoroutine(StartAfterDelay(1f));
    }

    private IEnumerator StartAfterDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        currentState = EnemyState.Start;
        anim.SetBool("1_Move", true);
    }

    public void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Dead) return;
        anim.SetTrigger("3_Damaged");
        currentHP -= damage;
        if (currentHP <= 0) Die();
    }

    protected virtual void Die()
    {
        // 1. 상태를 즉시 Dead로 변경
        currentState = EnemyState.Dead;

        // ⭐ 죽기 전에 트리거에게 나 죽었다고 알림
        if (myTrigger != null)
        {
            myTrigger.OnEnemyDestroyed(this);
        }

        // 2. 물리 속도 정지
        if (rb != null) rb.velocity = Vector2.zero;

        // ⭐ 3. 콜라이더 즉시 비활성화 (물리 판정 제거)
        // 적에게 붙어있는 모든 종류의 Collider2D를 찾아 꺼버립니다.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        anim.SetTrigger("4_Death");
        Destroy(gameObject, 0.4f);
    }
}