using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float damage = 20f;

    private void OnTriggerEnter(Collider other)
    {
        // ถ้าชนวัตถุที่มี Tag ว่า Enemy
        if (other.CompareTag("Enemy"))
        {
            // สั่งลดเลือด (ใช้สคริปต์ EnemyHealth ที่เราเตรียมไว้เมื่อกี้)
            if (other.GetComponent<EnemyHealth>())
            {
                other.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
        }
    }
}
