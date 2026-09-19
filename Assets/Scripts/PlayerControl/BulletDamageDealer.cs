using UnityEngine;

// ใช้สำหรับกระสุน (รับ damage จาก WeaponShooter)
public class BulletDamageDealer : MonoBehaviour
{
    public float damage = 0f;
    public float lifeTime = 5f; // [เพิ่ม] เวลาทำลายตัวเอง ป้องกันกระสุนค้างใน Memory

    private void Start()
    {
        // [เพิ่ม] ถ้ายิงไม่โดนอะไรเลย ครบ 5 วินาทีให้ลบตัวเองทิ้งทันที
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // เช็กทั้ง Boss และ Enemy ทั่วไป
        if (other.CompareTag("Boss"))
        {
            BossStatus boss = other.GetComponent<BossStatus>();
            if (boss != null)
            {
                boss.TakeDamage(damage); // ส่งดาเมจไปลดเลือดบอส + ลดหลอด UI
                Destroy(gameObject);     // กระสุนหายไปเมื่อชน
            }
        }
        // [เพิ่ม] ชนกำแพงหรือสิ่งกีดขวาง ให้ทำลายกระสุนทิ้งด้วย
        else if (other.CompareTag("Wall") || other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}