using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float damage = 0f;

    // แนะนำให้ใช้ OnEnable หรือเรียกใช้จากสคริปต์ Combat ตอนฟัน
    // เพื่ออัปเดตดาเมจล่าสุด (เผื่อมีการอัปเกรดระหว่างเล่น)
    void Start()
    {
        // ค้นหา CharacterStatus ในตัว Player (Parent)
        CharacterStatus status = GetComponentInParent<CharacterStatus>();
        if (status != null)
            damage = status.GetSwordDamage();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Boss")) return;

        BossStatus boss = other.GetComponent<BossStatus>();
        if (boss != null)
        {
            boss.TakeDamage(damage);

            // --- จุดสำคัญ ---
            // ห้ามใส่ Destroy(gameObject); ตรงนี้เด็ดขาดถ้าเป็นดาบ!
            // ไม่งั้นดาบจะหายไปจากมือนายครับ

            Debug.Log("ฟันบอสเข้าแล้ว! ดาเมจ: " + damage);
        }
    }
}