using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    private Weaponmanager weaponManager;
    private WeaponShooter weaponShooter;

    [Header("Sword Combo")]
    public int comboStep = 0;
    public float comboResetDelay = 1.0f;
    private float lastComboTime;
    public Collider swordCollider;
    public float attackDuration = 0.3f;

    [Header("Magic Settings")]
    public GameObject magicBulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    [Header("Shield Settings")]
    public bool isBlocking = false;
    public GameObject shieldModel;

    void Start()
    {
        weaponManager = GetComponent<Weaponmanager>();
        weaponShooter = GetComponent<WeaponShooter>();
    }

    // --- คลิกซ้าย ---
    public void OnLightAttack()
    {
        string type = weaponManager.currentWeaponType.ToLower();

        // เช็กทั้งชื่อสี และ ชื่ออาวุธ เพื่อกันเหนียว
        if (type.Contains("sword")) SwordCombo();
        else if (type.Contains("magic") ) MagicShoot();
        else if (type.Contains("shield") ) ShieldBash();
    }

    // --- คลิกขวา ---
    public void OnHeavyAttack()
    {
        string type = weaponManager.currentWeaponType.ToLower();
        if (type.Contains("sword")) ChargedSlash();
        else if (type.Contains("magic")) MagicExplosion();
        else if (type.Contains("shield")) BlockAndParry();
    }

    // --- ปุ่ม R ---
    public void OnSkill()
    {
        Debug.Log("กางอณาเขต");
    }

    // --- Logic ของแต่ละอาวุธ ---
    void SwordCombo()
    {
        if (Time.time - lastComboTime > comboResetDelay) comboStep = 0;
        comboStep++;
        lastComboTime = Time.time;

        Debug.Log("Sword Combo Stage: " + comboStep);
        StartCoroutine(EnableSwordHitbox());

        if (comboStep >= 3) comboStep = 0; // ครบ 3 ท่าแล้วเริ่มใหม่
    }

    IEnumerator EnableSwordHitbox()
    {
        if (swordCollider) swordCollider.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        if (swordCollider) swordCollider.enabled = false;
    }

    void MagicShoot()
    {
        if (weaponShooter != null)
        {
            weaponShooter.Shoot();
        }
    }

    void ShieldBash() { Debug.Log("เอาโล่กระแทก!"); }

    // --- ท่าคลิกขวา (สร้างฟังก์ชันเปล่ารอไว้ก่อน จะได้ไม่ Error) ---
    void ChargedSlash() { Debug.Log("ชาร์จฟัน!"); }
    void MagicExplosion() { Debug.Log("เวทระเบิด!"); }
    void BlockAndParry() { Debug.Log("ป้องกัน!"); }
}