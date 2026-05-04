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

    [Header("Resource Costs")]
    public float chargedSlashStaminaCost = 30f;
    public float blockStaminaDrainRate = 15f; // Stamina ต่อวินาทีขณะกดค้าง
    public float magicExplosionManaCost = 25f;
    public float swordSkillManaCost = 20f;
    public float meteorSkillManaCost = 40f;
    public float shieldSkillManaCost = 20f;

    private float lastActionTime = -1f;
    private float actionCooldown = 0.2f;

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
        else if (type.Contains("shield")) ShieldCombo(); // คอมโบเหมือนดาบ
    }

    public void OnHeavyAttack()
    {
        if (!CanAct()) return;
        string type = weaponManager.currentWeaponType.ToLower();
        if (type.Contains("sword")) StartCoroutine(ChargedSlash());
        else if (type.Contains("magic")) MagicExplosion();
        // โล่จัดการผ่าน OnHeavyAttackHeld/Released แทน
    }

    // เรียกจาก PlayerController ตอนกดค้าง
    public void OnHeavyAttackHeld()
    {
        string type = weaponManager.currentWeaponType.ToLower();
        if (type.Contains("shield")) StartBlocking();
    }

    // เรียกจาก PlayerController ตอนปล่อย
    public void OnHeavyAttackReleased()
    {
        string type = weaponManager.currentWeaponType.ToLower();
        if (type.Contains("shield")) StopBlocking();
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
        if (!status.UseStamina(chargedSlashStaminaCost))
        {
            Debug.Log("Stamina ไม่พอ! ชาร์จไม่ได้!");
            yield break;
        }
        Debug.Log("เริ่มชาร์จ... เสีย Stamina " + chargedSlashStaminaCost);
        yield return new WaitForSeconds(1f);
        Debug.Log("ฟันโช๊ะ!");
    }

    void SwordAuraSkill()
    {
        if (!status.UseMana(swordSkillManaCost))
        {
            Debug.Log("Mana ไม่พอ! ใช้สกิลไม่ได้!");
            return;
        }
        Debug.Log("เปิดใช้งานดาบออร่า! เพิ่มดาเมจ 20% เสีย Mana " + swordSkillManaCost);
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

    void MagicExplosion()
    {
        if (!status.UseMana(magicExplosionManaCost))
        {
            Debug.Log("Mana ไม่พอ! ระเบิดไม่ออก!");
            return;
        }
        Debug.Log("เวทระเบิด! เสีย Mana " + magicExplosionManaCost);
    }

    void MeteorSkill()
    {
        if (!status.UseMana(meteorSkillManaCost))
        {
            Debug.Log("Mana ไม่พอ! เรียกอุกกาบาตไม่ได้!");
            return;
        }
        Debug.Log("เรียกอุกกาบาต! ตู้มมม เสีย Mana " + meteorSkillManaCost);
    }

    // --- Shield ---
    void ShieldCombo()
    {
        if (!status.UseStamina(10f))
        {
            Debug.Log("เหนื่อยเกินไป กระแทกไม่ไหว! Stamina: " + status.currentStamina);
            return;
        }

        if (Time.time - lastComboTime > comboResetDelay) comboStep = 0;
        comboStep++;
        lastComboTime = Time.time;
        Debug.Log("Shield Combo Stage: " + comboStep + " | Stamina คงเหลือ: " + status.currentStamina);
        if (comboStep >= 3) comboStep = 0;
    }

    void StartBlocking()
    {
        if (status.currentStamina <= 0)
        {
            Debug.Log("Stamina หมด! ป้องกันไม่ได้!");
            return;
        }
        isBlocking = true;
        if (shieldModel) shieldModel.SetActive(true);
        Debug.Log("เริ่มป้องกัน... Stamina: " + status.currentStamina);
    }

    void StopBlocking()
    {
        isBlocking = false;
        if (shieldModel) shieldModel.SetActive(false);

        // เช็กเพิ่มตรงนี้
        if (status.currentStamina <= 0)
        {
            Debug.Log("Stamina หมด! การป้องกันถูกยกเลิก");
        }
        else
        {
            Debug.Log("หยุดป้องกัน | Stamina คงเหลือ: " + status.currentStamina);
        }
    }

    void Update()
    {
        // ลด Stamina เรื่อยๆ ขณะกดค้างป้องกัน
        if (isBlocking)
        {
            if (status.currentStamina > 0)
            {
                status.currentStamina -= blockStaminaDrainRate * Time.deltaTime;
                status.currentStamina = Mathf.Max(status.currentStamina, 0);
            }
            else
            {
                StopBlocking();
                Debug.Log("Stamina หมด! หยุดป้องกันอัตโนมัติ");
            }
        }
    }

    void ShieldStunSkill()
    {
        if (!status.UseMana(shieldSkillManaCost))
        {
            Debug.Log("Mana ไม่พอ! ใช้สกิลไม่ได้!");
            return;
        }
        Debug.Log("กระแทกโล่! ศัตรูติดมึน เสีย Mana " + shieldSkillManaCost);
    }
}