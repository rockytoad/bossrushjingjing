using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    public void Shoot(float damage)
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // ใช้ linearVelocity สำหรับ Unity เวอร์ชั่นใหม่ (2023+) 
                // หรือเปลี่ยนเป็น .velocity ถ้าเป็นเวอร์ชั่นเก่า
                rb.linearVelocity = firePoint.forward * bulletSpeed;
            }
            Debug.Log("ยิงกระสุนออกไปแล้ว!");
        }
        else
        {
            Debug.LogWarning("ลืมใส่ Prefab หรือ FirePoint หรือเปล่านาย!");
        }
    }
}