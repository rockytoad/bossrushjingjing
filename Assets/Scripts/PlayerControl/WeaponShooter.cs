using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    public void Shoot(float damage)
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("ลืมใส่ Prefab หรือ FirePoint หรือเปล่านาย!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // ส่ง damage ไปให้ DamageDealer บนกระสุน
        DamageDealer dd = bullet.GetComponent<DamageDealer>();
        if (dd != null) dd.damage = damage;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = firePoint.forward * bulletSpeed;

        Debug.Log("ยิงกระสุนออกไปแล้ว! Damage: " + damage);
    }
}