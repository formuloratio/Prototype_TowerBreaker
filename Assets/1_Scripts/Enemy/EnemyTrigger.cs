using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [Header("적 감지 설정")]
    public LayerMask enemyLayer;

    [Header("클리어 설정")]
    public GameObject stageTransition; // 클리어 시 활성화할 오브젝트

    [Header("실시간 상태")]
    public List<Enemy> activeEnemies = new List<Enemy>();
    public DropChest currentChest; // 현재 필드에 존재하는 상자

    private Collider2D triggerCollider;
    private bool isTriggered = false;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    // 오브젝트 활성화 시 초기화하기 위함
    private void OnEnable()
    {
        isTriggered = false;
        activeEnemies.Clear();
        currentChest = null; // 상자 참조 초기화

        if (stageTransition != null)
            stageTransition.SetActive(false);

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

            List<Collider2D> results = new List<Collider2D>();
            Physics2D.OverlapCollider(triggerCollider, filter, results);

            foreach (Collider2D col in results)
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.ActivateEnemy(this);
                    activeEnemies.Add(enemy);
                }

                DropChest chest = col.GetComponent<DropChest>();
                if (chest != null)
                {
                    SetChest(chest);
                }
            }
            CheckClearCondition();
        }
    }

    // 외부에서 상자가 생성되었을 때 호출할 함수
    public void SetChest(DropChest chest)
    {
        currentChest = chest;
        currentChest.RegisterTrigger(this);
        Debug.Log("트리거에 보상 상자가 등록되었습니다.");
    }

    public void OnEnemyDestroyed(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
        CheckClearCondition();
    }

    // 상자가 파괴되었을 때 호출될 함수
    public void OnChestDestroyed()
    {
        currentChest = null; // 상자 참조 제거
        Debug.Log("상자가 파괴되어 클리어 조건을 다시 확인합니다.");
        CheckClearCondition();
    }

    private void CheckClearCondition()
    {
        // 적이 0마리이고 상자도 없을 때만 클리어 처리
        if (isTriggered && activeEnemies.Count == 0 && currentChest == null)
        {
            if (stageTransition != null)
            {
                stageTransition.SetActive(true);
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.CompleteStage();
                }
                Debug.Log("스테이지 클리어! 모든 적과 상자가 제거되었습니다.");
            }
        }
    }
}