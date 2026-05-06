using UnityEngine;

// ใช้สำหรับอาวุธระยะประชิด (ดาบ, โล่)
public class DamageDealer : MonoBehaviour
{
    public float damage = 0f;

    void Awake()
    {
        // ดึง damage จาก CharacterStatus บน player อัตโนมัติ
        CharacterStatus status = GetComponentInParent<CharacterStatus>();
        if (status != null)
            damage = status.GetSwordDamage();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyHealth eh = other.GetComponent<EnemyHealth>();
        if (eh != null)
            eh.TakeDamage(damage);
    }
}