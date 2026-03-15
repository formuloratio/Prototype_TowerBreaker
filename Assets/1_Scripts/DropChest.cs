using System.Collections;
using UnityEngine;

public class DropChest : MonoBehaviour
{
    public AudioClip chestHitSfx;
    public AudioClip chestBreakSfx;

    [Header("상자 설정")]
    public int hp = 50;
    public string saveKey = "SavedChestCount";

    private EnemyTrigger myTrigger;
    private SpriteRenderer sr;
    private Color originalColor;
    private bool isDead = false;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
        }
    }

    // 트리거 등록 함수
    public void RegisterTrigger(EnemyTrigger trigger)
    {
        myTrigger = trigger;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        SoundEvents.NotifySfx(chestHitSfx);
        hp -= damage;

        // 피격 시 빨간색 반짝임
        StopAllCoroutines();
        StartCoroutine(FlashRedRoutine());

        if (hp <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRedRoutine()
    {
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
        }
    }

    private void Die()
    {
        if (isDead) return;
        SoundEvents.NotifySfx(chestBreakSfx);
        isDead = true;

        // 데이터 저장 로직
        if (GameManager.Instance != null)
        {
            GameManager.Instance.totalChests += 1;
            PlayerPrefs.SetInt(saveKey, GameManager.Instance.totalChests);
            PlayerPrefs.Save();
        }

        // 트리거에 알림
        if (myTrigger != null)
        {
            myTrigger.OnChestDestroyed();
        }

        Debug.Log("상자 파괴 및 데이터 저장 완료");
        Destroy(gameObject);
    }
}