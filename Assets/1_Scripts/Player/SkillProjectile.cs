using UnityEngine;

public class SkillProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;
    private int damage;
    private Vector2 direction;

    public void Setup(Vector2 dir, int dmg)
    {
        direction = dir;
        damage = dmg;
        Destroy(gameObject, lifeTime); // 일정 시간 후 자동 파괴
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적 레이어 혹은 Enemy 스크립트 확인
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            // 이펙트 생성 로직 추가 가능
        }
    }
}