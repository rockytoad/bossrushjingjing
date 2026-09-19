using System.Collections;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float damage = 25f;
    public float attackDuration = 0.2f; // ระยะเวลาเปิด Hitbox ตอนฟัน
    private BoxCollider hitCollider;

    void Start()
    {
        hitCollider = GetComponent<BoxCollider>();
        if (hitCollider != null)
        {
            hitCollider.enabled = false; // ปิดกล่องไว้ก่อนในเวลาปกติ
        }
    }

    public void Swing()
    {
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
        if (other.CompareTag("Boss"))
        {
            BossStatus boss = other.GetComponent<BossStatus>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Debug.Log("⚔️ ฟันโดนบอสเต็มๆ!");
            }
        }
    }
}