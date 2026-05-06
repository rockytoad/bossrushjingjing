using UnityEngine;

// ใช้สำหรับกระสุน (รับ damage จาก WeaponShooter)
public class BulletDamageDealer : MonoBehaviour
{
    public float damage = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyHealth eh = other.GetComponent<EnemyHealth>();
        if (eh != null)
        {
            eh.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
