using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    public void Shoot(float damage)
    {
        if (bulletPrefab == null || firePoint == null) return;

        // 1. เสกกระสุน ณ ตำแหน่ง firePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 2. ให้กระสุนหันหน้าเข้าหากล้องทันที
        if (Camera.main != null)
        {
            bullet.transform.rotation = Camera.main.transform.rotation;
        }

        // 3. หมุนรูป Sprite ของกระสุนตามทิศทางที่ weaponPivot หันไป (ชี้ขึ้น/ลง/ซ้าย/ขวา)
        float zAngle = firePoint.eulerAngles.y; // ดึงมุม Y จาก Pivot มาปรับใช้กับแกน Z ของกระสุน
        BulletBillboard billboard = bullet.GetComponent<BulletBillboard>();
        if (billboard != null)
        {
            billboard.SetDirectionAngle(-zAngle);
        }

        // 4. ส่ง Damage
        BulletDamageDealer dd = bullet.GetComponent<BulletDamageDealer>();
        if (dd != null) dd.damage = damage;

        // 5. ดันกระสุนให้พุ่งไปข้างหน้าตามทิศทางไฟจริงใน 3D
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }
}