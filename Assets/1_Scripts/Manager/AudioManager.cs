using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("오디오 소스")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("오디오 클립 데이터 (인스펙터에서 할당)")]
    public AudioClip bgmClip;
    public AudioClip attackClickSfx;
    public AudioClip hitSfx;
    public AudioClip enemyDeathSfx;
    public AudioClip playerDeathSfx;
    public AudioClip skill1Sfx;
    public AudioClip skill2Sfx;
    public AudioClip skill3Sfx;
    public AudioClip defendSuccessSfx;
    public AudioClip bossPushSfx;
    public AudioClip bossHealSfx;
    public AudioClip chestDropSfx;
    public AudioClip chestHitSfx;
    public AudioClip chestBreakSfx;
    public AudioClip uiConfirmSfx;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        PlayBGM(bgmClip);
    }

    // ⭐ 이벤트 구독 (활성화될 때)
    private void OnEnable()
    {
        // SoundEvents라는 스태틱 클래스를 만들어 관리하면 편리합니다.
        SoundEvents.OnSfxPlay += PlaySFX;
        SoundEvents.OnBgmChange += PlayBGM;
    }

    // ⭐ 이벤트 해제 (비활성화될 때 - 메모리 누수 방지)
    private void OnDisable()
    {
        SoundEvents.OnSfxPlay -= PlaySFX;
        SoundEvents.OnBgmChange -= PlayBGM;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource.clip == clip) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
}

// ⭐ 이벤트를 전역에서 쏠 수 있게 해주는 중계소 클래스
public static class SoundEvents
{
    public static Action<AudioClip> OnSfxPlay;
    public static Action<AudioClip> OnBgmChange;

    // 편의를 위해 "알림" 함수 생성
    public static void NotifySfx(AudioClip clip) => OnSfxPlay?.Invoke(clip);
}