using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [Header("적 감지 설정")]
    public LayerMask enemyLayer;

    [Header("클리어 설정")]
    public GameObject stageTransition; // 클리어시 활성화할 오브젝트

    public List<Enemy> activeEnemies = new List<Enemy>();
    private Collider2D triggerCollider;
    private bool isTriggered = false;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    // ⭐ 상위 오브젝트 활성화 시 호출되는 초기화 로직
    private void OnEnable()
    {
        isTriggered = false; // 트리거 작동 여부 초기화
        activeEnemies.Clear(); // 관리 리스트 초기화

        if (stageTransition != null)
            stageTransition.SetActive(false); // 클리어 오브젝트 다시 숨김

        Debug.Log("EnemyTrigger 초기화 완료");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered && collision.CompareTag("Player"))
        {
            isTriggered = true;

            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(enemyLayer);
            filter.useLayerMask = true;

            List<Collider2D> enemiesInTrigger = new List<Collider2D>();
            Physics2D.OverlapCollider(triggerCollider, filter, enemiesInTrigger);

            foreach (Collider2D enemyCol in enemiesInTrigger)
            {
                Enemy enemy = enemyCol.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.ActivateEnemy(this);
                    activeEnemies.Add(enemy);
                }
            }
            CheckClearCondition();
        }
    }

    public void OnEnemyDestroyed(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
        CheckClearCondition();
    }

    private void CheckClearCondition() // 적이 모두 제거되었는지 확인
    {
        if (isTriggered && activeEnemies.Count == 0)
        {
            if (stageTransition != null)
            {
                stageTransition.SetActive(true);
                GameManager.Instance.CompleteStage(); // 스테이지 점수 증가
                Debug.Log("스테이지 클리어! 다음 단계로 이동 준비 완료.");
            }
        }
    }
}