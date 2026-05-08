using UnityEngine;

// ใช้สำหรับกระสุน (รับ damage จาก WeaponShooter)
public class BulletDamageDealer : MonoBehaviour
{
    public float damage = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Boss")) return;

        BossStatus boss = other.GetComponent<BossStatus>();
        if (boss != null)
        {
            boss.TakeDamage(damage); // ส่งดาเมจไปลดเลือดบอส + ลดหลอด UI
            Destroy(gameObject);     // กระสุนหายไปเมื่อชน
        }
    }
}
