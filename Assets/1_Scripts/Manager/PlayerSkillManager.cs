using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public AudioClip skill1Sfx;
    public AudioClip skill2Sfx;
    public AudioClip skill3Sfx;

    [Header("참조 설정")]
    public PlayerController player;

    [Header("스킬 1: 대지진 (넉백)")]
    public float skill1Cooldown = 10f;
    public float knockbackForce = 30f;
    private float lastSkill1Time = -100f;

    [Header("스킬 2: 장풍 (투사체)")]
    public GameObject projectilePrefab;
    public float skill2Cooldown = 20f;
    public int skill2Damage = 40;
    private float lastSkill2Time = -100f;

    [Header("스킬 3: 재생 (체력 증가)")]
    public float skill3Cooldown = 30f;
    public int healAmount = 50;
    private float lastSkill3Time = -100f;

    public void UseSkill1()
    {
        if (Time.time < lastSkill1Time + skill1Cooldown) return;
        if (player.currentState != PlayerStateEnum.Idle) return;
        SoundEvents.NotifySfx(skill1Sfx);

        lastSkill1Time = Time.time;
        Debug.Log("스킬 1: 대지진 발동!");

        if (AttackEffectsManager.Instance != null)
        {
            AttackEffectsManager.Instance.PlayScreenShake(0.5f, 0.4f);
        }

        // player에 저장된 currentEnemyTrigger를 직접 참조
        if (player.currentEnemyTrigger != null)
        {
            List<Enemy> targets = new List<Enemy>(player.currentEnemyTrigger.activeEnemies);

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                if (targets[i] != null)
                {
                    targets[i].ApplyKnockback(knockbackForce);
                }
            }
        }
    }

    public void UseSkill2()
    {
        if (Time.time < lastSkill2Time + skill2Cooldown) return;
        if (player.currentState != PlayerStateEnum.Idle) return;

        SoundEvents.NotifySfx(skill2Sfx);

        lastSkill2Time = Time.time;
        Debug.Log("스킬 2: 신풍 발사!");

        if (projectilePrefab != null)
        {
            // 플레이어 위치에서 발사
            GameObject proj = Instantiate(projectilePrefab, player.transform.position, Quaternion.identity);
            SkillProjectile script = proj.GetComponent<SkillProjectile>();
            if (script != null)
            {
                script.Setup(Vector2.right, skill2Damage);
            }
        }
    }

    public void UseSkill3()
    {
        if (Time.time < lastSkill3Time + skill3Cooldown) return;
        if (player.currentState != PlayerStateEnum.Idle) return;

        lastSkill3Time = Time.time; // 쿨타임 변수 수정
        SoundEvents.NotifySfx(skill3Sfx);
        Debug.Log("스킬 3: 불굴의 의지! 5초간 제자리에 고정됩니다.");

        StartCoroutine(UnstoppableRoutine(5f));
    }

    private IEnumerator UnstoppableRoutine(float duration)
    {
        // 상태 활성화: X축 위치 고정 및 초록색 변경
        player.rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();
        Color originalColor = Color.white;

        if (sr != null)
        {
            originalColor = sr.color;
            sr.color = Color.green;
        }

        yield return new WaitForSeconds(duration);

        // 상태 복구: X축 고정 해제
        player.rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (sr != null)
        {
            sr.color = originalColor;
        }

        Debug.Log("스킬 3 종료: 이제 다시 움직이거나 밀릴 수 있습니다.");
    }
}