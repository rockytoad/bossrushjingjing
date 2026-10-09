using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    private Weaponmanager weaponManager;
    private WeaponShooter weaponShooter;
    private CharacterStatus status;
    [Header("Sword Setup")]
    public DamageDealer swordDamageDealer; 
    private bool isAuraActive = false;     
    [Header("Shield Setup")]
    public DamageDealer shieldDamageDealer; // ลาก DamageDealer บนโล่มาใส่
    [Header("Sword Combo")]
    public int comboStep = 0;
    public float comboResetDelay = 1.0f;
    private float lastComboTime;
    public Collider swordCollider;
    public float attackDuration = 0.3f;

    [Header("Shield Settings")]
    public bool isBlocking = false;
    public GameObject shieldModel; // ใส่ GameObject ของโล่ป้องกัน (หรือปรับตามโครงสร้างอาวุธ)
    [Header("Meteor Setup")]
    public GameObject meteorPrefab; // ลาก 3D Sphere ธรรมดามาใส่ทดสอบก่อนได้
    public float meteorSpawnHeight = 15f; // ความสูงที่สปอว์นอุกกาบาตก่อนร่วงลงมา
    [Header("Magic Explosion Setup")]
    public float explosionLockOnRadius = 12f;
    public float explosionDamageMultiplier = 2.5f; 
    public GameObject explosionVFXPrefab; // ใส่ Particle Effect ทีหลังได้

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
        return (Time.unscaledTime - lastActionTime >= actionCooldown);
    }

    public void OnLightAttack()
    {
        float diff = Time.unscaledTime - lastActionTime;
        Debug.Log($"[CHECK] CanAct: {CanAct()} | เวลาที่ผ่านไป: {diff:F2}s | Cooldown: {actionCooldown}s | อาวุธ: {weaponManager.currentWeaponType}");

        if (!CanAct()) return;

        string type = weaponManager.currentWeaponType.ToLower();
        if (type.Contains("sword")) SwordCombo();
        else if (type.Contains("magic")) MagicShoot();
        else if (type.Contains("shield")) ShieldCombo();
    }

    public void OnHeavyAttack()
    {
        if (!CanAct()) return;
        string type = weaponManager.currentWeaponType.ToLower();
        if (type.Contains("sword")) StartCoroutine(ChargedSlash());
        else if (type.Contains("magic")) MagicExplosion();
    }

    // เรียกจาก PlayerController ตอนกดค้าง (คลิกขวา)
    public void OnHeavyAttackHeld()
    {
        string type = weaponManager.currentWeaponType.ToLower();
        if (type.Contains("shield")) StartBlocking();
    }

    // เรียกจาก PlayerController ตอนปล่อย (ปล่อยคลิกขวา)
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
        lastActionTime = Time.unscaledTime;
        if (!status.UseStamina(10f)) return;

        if (Time.time - lastComboTime > comboResetDelay) comboStep = 0;
        comboStep++;
        lastComboTime = Time.time;

        float multiplier = (comboStep == 3) ? 1.5f : 1.0f; // จังหวะ 3 แรง 1.5 เท่า
        if (isAuraActive) multiplier *= 1.2f; // บัฟออร่า +20%

        // สั่งฟันปุ่มเดียวจบ!
        if (swordDamageDealer != null) swordDamageDealer.Swing(multiplier);

        if (comboStep >= 3) comboStep = 0;
    }

    
    IEnumerator ChargedSlash()
    {
        if (!status.UseStamina(chargedSlashStaminaCost))
        {
            Debug.Log("Stamina ไม่พอ! ชาร์จไม่ได้!");
            yield break;
        }

        Debug.Log("เริ่มชาร์จ... เสีย Stamina " + chargedSlashStaminaCost);
        yield return new WaitForSeconds(0.6f); // เวลาชาร์จฟัน 0.6 วินาที

        // คำนวณดาเมจ: ชาร์จฟันแรง 2.5 เท่า (ถ้าเปิดออร่าคูณเพิ่มอีก 20%)
        float multiplier = 2.5f;
        if (isAuraActive) multiplier *= 1.2f;

        // สั่งฟันผ่าน DamageDealer!
        if (swordDamageDealer != null)
        {
            swordDamageDealer.Swing(multiplier);
        }

        Debug.Log("<color=red>💥 ฟันชาร์จโช๊ะ!! Multiplier: " + multiplier + "</color>");
    }

    void SwordAuraSkill()
    {
        if (isAuraActive)
        {
            Debug.Log("ออร่าดาบทำงานอยู่แล้ว!");
            return;
        }

        if (!status.UseMana(swordSkillManaCost))
        {
            Debug.Log("Mana ไม่พอ! ใช้สกิลไม่ได้!");
            return;
        }

        StartCoroutine(AuraBuffRoutine(10f)); // เปิดบัฟนาน 10 วินาที
    }
    IEnumerator AuraBuffRoutine(float duration)
    {
        isAuraActive = true;
        Debug.Log("<color=cyan>✨ เปิดใช้งานดาบออร่า! เพิ่มดาเมจ 20% เป็นเวลา " + duration + " วินาที</color>");

        yield return new WaitForSeconds(duration);

        isAuraActive = false;
        Debug.Log("<color=white>✨ บัฟออร่าดาบหมดเวลาแล้ว!</color>");
    }

    // --- Magic ---
    void MagicShoot()
    {
        lastActionTime = Time.unscaledTime;
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
        lastActionTime = Time.unscaledTime; // ล็อก Cooldown กันกดเบิล

        // 1. ค้นหาเป้าหมายศัตรูที่ใกล้ที่สุดในระยะ
        Transform target = GetClosestTarget(explosionLockOnRadius);

        // ถ้าไม่มีศัตรูในระยะล็อกเลย ให้ตัดจบ (หรือไม่เสีย Mana)
        if (target == null)
        {
            Debug.Log("ไม่มีเป้าหมายในระยะล็อกระเบิด!");
            return;
        }

        // 2. เช็ก Mana
        if (!status.UseMana(magicExplosionManaCost))
        {
            Debug.Log("Mana ไม่พอ! ระเบิดไม่ออก!");
            return;
        }

        // 3. คำนวณดาเมจ
        float damage = status.GetMagicDamage() * explosionDamageMultiplier;

        // 4. ส่งดาเมจเข้าตัวเป้าหมายที่ล็อกไว้โดยตรง
        BossStatus boss = target.GetComponent<BossStatus>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
        }
        /* ถ้ามีศัตรูทั่วไปในอนาคต:
        EnemyStatus enemy = target.GetComponent<EnemyStatus>();
        if (enemy != null) enemy.TakeDamage(damage);
        */

        // 5. เสก Effect ระเบิดตรงตำแหน่งตัวศัตรู (ใส่ VFX ทีหลังได้)
        if (explosionVFXPrefab != null)
        {
            GameObject vfx = Instantiate(explosionVFXPrefab, target.position, Quaternion.identity);
            Destroy(vfx, 1.0f); // เสกเสร็จ 1 วินาที สั่งทำลายทิ้งทันทีไม่ให้รก Hierarchy
        }

        Debug.Log($"<color=magenta>💥 ระเบิดเวทล็อกเป้าใส่ [{target.name}] ดาเมจ: {damage}</color>");
    }

    void MeteorSkill()
    {
        lastActionTime = Time.unscaledTime; // ล็อก Cooldown กันกดเบิล

        if (!status.UseMana(meteorSkillManaCost))
        {
            Debug.Log("Mana ไม่พอ! เรียกอุกกาบาตไม่ได้!");
            return;
        }

        // 1. หาตำแหน่งเมาส์บนพื้นโลก 3D
        Vector3 targetPos = GetMouseGroundPosition();
        Vector3 spawnPos = targetPos + Vector3.up * meteorSpawnHeight;

        // 2. เสกอุกกาบาต (ทรงกลมสำรอง) เหนือเป้าหมาย
        if (meteorPrefab != null)
        {
            GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

            // ถ้า Prefab ยังไม่มีสคริปต์ MeteorProjectile ระบบจะแอดให้อัตโนมัติ
            MeteorProjectile meteorScript = meteor.GetComponent<MeteorProjectile>();
            if (meteorScript == null) meteorScript = meteor.AddComponent<MeteorProjectile>();

            // ส่งตำแหน่งเป้าหมาย + ดาเมจ (เช่น ความแรงเวท x 3 เท่า)
            meteorScript.Setup(targetPos, status.GetMagicDamage() * 3.0f);
        }

        Debug.Log("<color=orange>☄️ สั่งอุกกาบาตร่วงใส่จุด: " + targetPos + "</color>");
    }

    // --- Shield ---
    void ShieldCombo()
    {
        lastActionTime = Time.unscaledTime;
        if (!status.UseStamina(10f))
        {
            Debug.Log("เหนื่อยเกินไป กระแทกไม่ไหว! Stamina: " + status.currentStamina);
            return;
        }

        if (Time.time - lastComboTime > comboResetDelay) comboStep = 0;
        comboStep++;
        lastComboTime = Time.time;

        // 💥 เพิ่มบรรทัดสั่งเปิด Hitbox ทำดาเมจตรงนี้!
        float multiplier = (comboStep == 3) ? 1.5f : 1.0f; // จังหวะคอมโบที่ 3 แรงขึ้น 1.5 เท่า
        if (shieldDamageDealer != null)
        {
            shieldDamageDealer.Swing(multiplier);
        }

        Debug.Log("Shield Combo Stage: " + comboStep + " | Stamina คงเหลือ: " + status.currentStamina);
        if (comboStep >= 3) comboStep = 0;
    }

    public void StartBlocking()
    {
        // ถ้า Stamina หมดหรือน้อยเกินไป ห้ามยกโล่
        if (status.currentStamina <= 1f)
        {
            StopBlocking();
            Debug.Log("Stamina ไม่พอจะยกโล่!");
            return;
        }

        isBlocking = true;
        // ปรับตรงนี้: ถ้ามี Visual Effects หรือ Animation การตั้งการ์ดค่อยสั่งตรงนี้
        // ไม่สั่งปิด shieldModel เพื่อไม่ให้โมเดลอาวุธหาย
        Debug.Log("เริ่มป้องกัน... Stamina: " + status.currentStamina);
    }

    public void StopBlocking()
    {
        isBlocking = false;

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
        // คอยหัก Stamina เรื่อยๆ เมื่ออยู่ในสถานะป้องกัน
        if (isBlocking)
        {
            // ใช้ฟังก์ชัน UseStamina ของ CharacterStatus เพื่อหักค่าความลื่นไหล
            bool hasStamina = status.UseStamina(blockStaminaDrainRate * Time.deltaTime);

            // ถ้า Stamina หมด ให้ยกเลิกการตั้งการ์ดทันที
            if (!hasStamina || status.currentStamina <= 0)
            {
                StopBlocking();
            }
        }
    }

    void ShieldStunSkill()
    {
        lastActionTime = Time.unscaledTime; //  เพิ่มบรรทัดนี้เพื่อรีเซ็ต Cooldown!

        if (!status.UseMana(shieldSkillManaCost))
        {
            Debug.Log("Mana ไม่พอ! ใช้สกิลไม่ได้!");
            return;
        }

        if (shieldDamageDealer != null)
        {
            shieldDamageDealer.Swing(2.0f);
        }

        Debug.Log("<color=yellow>🛡️💥 สกิล Shield Bash! ดาเมจ 2.0x (เสีย Mana " + shieldSkillManaCost + ")</color>");

    }
    private Vector3 GetMouseGroundPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // ระนาบพื้นโลก

        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return transform.position + transform.forward * 5f; // สำรองกรณีเล็งหลุดจอ
    }
    // ฟังก์ชันค้นหาศัตรู (Boss/Enemy) ที่อยู่ใกล้ตัวผู้เล่นที่สุดในระยะ radius
    private Transform GetClosestTarget(float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (var col in hitColliders)
        {
            // ตรวจจับวัตถุที่มี Tag Boss หรือ Enemy
            if (col.CompareTag("Boss") || col.CompareTag("Enemy"))
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = col.transform;
                }
            }
        }

        return closest;
    }
}