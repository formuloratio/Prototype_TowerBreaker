using System.Collections;
using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    [Header("참조 설정")]
    public GameObject freezingEffect;    // 1순위: 활성화될 Freezing 오브젝트
    public GameObject gameOverUI;       // 2순위: 2초 후 뜰 게임오버 창

    private bool isGameOver = false; // 중복 실행 방지

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 플레이어와 충돌했는지 확인
        if (!isGameOver && collision.CompareTag("Player"))
        {
            isGameOver = true;
            Debug.Log("<color=red>게임 오버 조건 충족!</color>");

            // 2. 게임 오버 시퀀스 시작
            StartCoroutine(GameOverSequence(collision.gameObject));
        }
    }

    private IEnumerator GameOverSequence(GameObject player)
    {
        // ⭐ [단계 1] 플레이어를 빨갛게 물들임
        SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
        }

        // 플레이어 움직임 정지 (선택 사항: 물리 엔진 정지)
        //Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        //if (rb != null) rb.velocity = Vector2.zero;

        // ⭐ [단계 2] Freezing 오브젝트 활성화
        if (freezingEffect != null)
        {
            freezingEffect.SetActive(true);
        }

        // ⭐ [단계 3] 2초 대기
        yield return new WaitForSeconds(2.0f);

        // ⭐ [단계 4] Freezing 비활성화 및 게임오버 UI 활성화
        if (freezingEffect != null)
        {
            freezingEffect.SetActive(false);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // 필요 시 시간 정지
        // Time.timeScale = 0f; 
    }
}