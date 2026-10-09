using UnityEngine;
using UnityEngine.UIElements;

public class MeteorProjectile : MonoBehaviour
{
    public float fallSpeed = 25f;       // ความเร็วในการร่วง
    public float explosionRadius = 4f;  // รัศมีระเบิดวงกว้าง (AOE)

    private Vector3 targetPoint;
    private float damage;
    private bool hasExploded = false;

    public void Setup(Vector3 target, float damageAmount)
    {
        targetPoint = target;
        damage = damageAmount;
    }

    void Update()
    {
        if (hasExploded) return;

        // ดันตัวอุกกาบาตร่วงลงหาเป้าหมายบนพื้น
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, fallSpeed * Time.deltaTime);

        // เมื่อร่วงลงถึงพื้น (หรือเข้าใกล้เป้าหมายมากพอ)
        if (Vector3.Distance(transform.position, targetPoint) < 0.2f)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        // สแกนวัตถุทั้งหมดในรัศมีระเบิด
        Collider[] hitColliders = Physics.OverlapSphere(targetPoint, explosionRadius);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Boss") || hit.CompareTag("Enemy"))
            {
                // ส่งดาเมจใส่ Boss หรือ Enemy
                BossStatus boss = hit.GetComponent<BossStatus>();
                if (boss != null)
                {
                    boss.TakeDamage(damage);
                }

                Debug.Log($"<color=red>💥 อุกกาบาตระเบิดใส่ {hit.name} ดาเมจ: {damage}</color>");
            }
        }

        Destroy(gameObject); // ลบตัวเองทิ้งเมื่อระเบิดจบ
    }

    // วาดวงกลมสีแดงในหน้า Scene View ให้เห็นขนาดรัศมี AOE ชัดเจนตอนทดสอบ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
