using System.Collections;
using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    public AudioClip gameOverSfx;

    [Header("참조 설정")]
    public GameObject freezingEffect;
    public GameObject gameOverUI;

    private bool isGameOver = false; // 중복 실행 방지

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌했는지 확인
        if (!isGameOver && collision.CompareTag("Player"))
        {
            isGameOver = true;
            Debug.Log("<color=red>게임 오버 조건 충족!</color>");

            // 게임 오버 시퀀스 시작
            StartCoroutine(GameOverSequence(collision.gameObject));
        }
    }

    private IEnumerator GameOverSequence(GameObject player)
    {
        SoundEvents.NotifySfx(gameOverSfx);

        // 플레이어를 빨갛게 물들임
        SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
        }

        if (freezingEffect != null)
        {
            freezingEffect.SetActive(true);
        }

        yield return new WaitForSeconds(2.0f);

        if (freezingEffect != null)
        {
            freezingEffect.SetActive(false);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }
}