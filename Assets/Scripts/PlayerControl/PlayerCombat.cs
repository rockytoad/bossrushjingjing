using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    private Weaponmanager weaponManager;
    private WeaponShooter weaponShooter;
    private CharacterStatus status;

    [Header("Sword Combo")]
    public int comboStep = 0;
    public float comboResetDelay = 1.0f;
    private float lastComboTime;
    public Collider swordCollider;
    public float attackDuration = 0.3f;

    [Header("Shield Settings")]
    public bool isBlocking = false;
    public GameObject shieldModel;

    private bool isAttacking = false;
    private float lastActionTime = -1f;
    private float actionCooldown = 0.2f; // กัน trigger ซ้ำในช่วง 200ms

    void Start()
    {
        weaponManager = GetComponent<Weaponmanager>();
        weaponShooter = GetComponent<WeaponShooter>();
        status = GetComponent<CharacterStatus>();
    }

    bool CanAct()
    {
        if (Time.unscaledTime - lastActionTime < actionCooldown) return false;
        lastActionTime = Time.unscaledTime;
        return true;
    }

    public void OnLightAttack()
    {
        if (!CanAct()) return;
        string type = weaponManager.currentWeaponType.ToLower();
        if (type.Contains("sword")) SwordCombo();
        else if (type.Contains("magic")) MagicShoot();
        else if (type.Contains("shield")) ShieldBash();
    }

    public void OnHeavyAttack()
    {
        if (!CanAct()) return;
        string type = weaponManager.currentWeaponType.ToLower();
        if (type.Contains("sword")) StartCoroutine(ChargedSlash());
        else if (type.Contains("magic")) MagicExplosion();
        else if (type.Contains("shield")) BlockAndParry();
    }

    public void OnSkill()
    {
        if (!CanAct()) return;
        string type = weaponManager.currentWeaponType.ToLower();
        if (type.Contains("sword")) SwordAuraSkill();
        else if (type.Contains("magic")) MeteorSkill();
        else if (type.Contains("shield")) ShieldStunSkill();
    }

    // --- Sword ---
    void SwordCombo()
    {
        if (!status.UseStamina(10f))
        {
            Debug.Log("เหนื่อยเกินไป ฟันไม่ไหว!");
            return;
        }

        if (Time.time - lastComboTime > comboResetDelay) comboStep = 0;

        comboStep++;
        lastComboTime = Time.time;
        StartCoroutine(EnableSwordHitbox());
        Debug.Log("Sword Combo Stage: " + comboStep);

        if (comboStep >= 3) comboStep = 0;
    }

    IEnumerator EnableSwordHitbox()
    {
        if (swordCollider) swordCollider.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        if (swordCollider) swordCollider.enabled = false;
    }

    IEnumerator ChargedSlash()
    {
        
        Debug.Log("เริ่มชาร์จ...");
        yield return new WaitForSeconds(1f);
        Debug.Log("ฟันโช๊ะ!");
       
    }

    void SwordAuraSkill()
    {
        Debug.Log("เปิดใช้งานดาบออร่า! เพิ่มดาเมจ 20%");
    }

    // --- Magic ---
    void MagicShoot()
    {
        if (!status.UseMana(10f))
        {
            Debug.Log("Mana ไม่พอ! ยิงไม่ออก");
            return;
        }

        weaponShooter.Shoot(status.GetMagicDamage());
        Debug.Log("ยิงเวท! Mana คงเหลือ: " + status.currentMana);
    }

    void MagicExplosion() { Debug.Log("เวทระเบิด!"); }

    void MeteorSkill() { Debug.Log("เรียกอุกกาบาต! ตู้มมม"); }

    // --- Shield ---
    void ShieldBash() { Debug.Log("เอาโล่กระแทก!"); }

    void BlockAndParry() { Debug.Log("ป้องกัน!"); }

    void ShieldStunSkill() { Debug.Log("กระแทกโล่! ศัตรูติดมึน"); }
}