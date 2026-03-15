using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("외형 설정")]
    public GameObject enemyVisualPrefab;

    [Header("기본 정보")]
    public string enemyName = "일반 적";
    public int maxHP = 100;

    [Header("이동 설정")]
    public float initialSpeed = 2f;    // 초기 이동 속도
    public float acceleration = 5f;    // 초당 증가하는 가속도
    public float maxSpeed = 15f;       // 최대 속도 제한
}