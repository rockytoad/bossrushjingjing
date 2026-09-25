using UnityEngine;
using System.Collections.Generic;

public class DamageDealer : MonoBehaviour
{
    public float baseDamage = 0f;
    private float currentMultiplier = 1f;
    private CharacterStatus status;
    private Collider hitCollider;

    private List<Collider> hitEnemies = new List<Collider>();

    void Awake()
    {
        hitCollider = GetComponent<Collider>();
        if (hitCollider != null) hitCollider.enabled = false; // ปิดกล่องฟันไว้ก่อน
    }

    void Start()
    {
        status = GetComponentInParent<CharacterStatus>();
    }

    // ฟังก์ชันสั่งฟัน (เรียกจาก PlayerCombat)
    public void Swing(float damageMultiplier = 1.0f)
    {
        if (status != null) baseDamage = status.GetSwordDamage();

        currentMultiplier = damageMultiplier;
        hitEnemies.Clear();

        CancelInvoke(nameof(DisableHitbox));
        if (hitCollider != null) hitCollider.enabled = true;

        Invoke(nameof(DisableHitbox), 0.25f); // ฟันเสร็จปิดกล่องใน 0.25 วินาที
    }

    void DisableHitbox()
    {
        if (hitCollider != null) hitCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Boss") || hitEnemies.Contains(other)) return;

        BossStatus boss = other.GetComponent<BossStatus>();
        if (boss != null)
        {
            float finalDamage = baseDamage * currentMultiplier;
            boss.TakeDamage(finalDamage);
            hitEnemies.Add(other);

            Debug.Log("<color=red>ฟันบอสเข้าแล้ว! ดาเมจ: " + finalDamage + "</color>");
        }
    }
}