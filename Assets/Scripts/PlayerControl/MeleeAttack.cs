using System.Collections;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float baseDamage = 25f;
    public float attackDuration = 0.2f; // ระยะเวลาเปิด Hitbox ตอนฟัน
    private Collider hitCollider;
    private float currentMultiplier = 1f;

    void Start()
    {
        hitCollider = GetComponent<Collider>();
        if (hitCollider != null)
        {
            hitCollider.enabled = false; // ปิดกล่องไว้ก่อนในเวลาปกติ
        }
    }

    // ปรับให้รับค่า damageMultiplier เข้ามาได้
    public void Swing(float damageMultiplier = 1.0f)
    {
        currentMultiplier = damageMultiplier;
        StopAllCoroutines(); // กันลูปชนกันเวลากดฟันรัวๆ
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        if (hitCollider != null) hitCollider.enabled = true; // เปิดกล่องฟัน
        yield return new WaitForSeconds(attackDuration);
        if (hitCollider != null) hitCollider.enabled = false; // ฟันเสร็จแล้วปิดกล่อง
    }

    private void OnTriggerEnter(Collider other)
    {
        // เช็กทั้ง BossStatus และ Tag เผื่อไว้กันพลาด
        BossStatus boss = other.GetComponent<BossStatus>();
        if (boss != null)
        {
            float finalDamage = baseDamage * currentMultiplier;
            boss.TakeDamage(finalDamage);
            Debug.Log("<color=red>⚔️ ฟันโดนบอส! ดาเมจ: " + finalDamage + "</color>");
        }
    }
}