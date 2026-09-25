using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    private PlayerCombat playerCombat;
    public HealthBar healthBarUI;
    private float nextDamageTime;
    public float damageCooldown = 0.5f;
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Mana")]
    public float maxMana = 50f;
    public float currentMana;
    public float manaRegenRate = 10f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;

    [Header("Combat Stats")]
    public float swordDamage = 15f;
    public float magicDamage = 20f;
    

    void Awake()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;
        playerCombat = GetComponent<PlayerCombat>();
    }
    void Start()
    {
        // ตั้งค่าเริ่มต้นให้ UI
        healthBarUI.SetMaxHealth(maxHealth);
        healthBarUI.SetMaxMana(maxMana);
        healthBarUI.SetMaxStamina(maxStamina);
    }

    void Update()
    {
            // Stamina regen เฉพาะตอนไม่ได้ป้องกัน
            if (currentStamina < maxStamina && !playerCombat.isBlocking)
                currentStamina = Mathf.Clamp(currentStamina + staminaRegenRate * Time.deltaTime, 0, maxStamina);

            // Mana regen ปกติ
            if (currentMana < maxMana)
                currentMana = Mathf.Clamp(currentMana + manaRegenRate * Time.deltaTime, 0, maxMana);

        healthBarUI.SetHealth(currentHealth);
        healthBarUI.SetMana(currentMana);
        healthBarUI.SetStamina(currentStamina);

    }

    public float GetSwordDamage() => swordDamage;
    public float GetMagicDamage() => magicDamage;

    public void TakeDamage(float amount)
    {
        // 1. เช็กอมตะ (I-Frame) ของเดิม - ถ้ายังอยู่ในช่วงอมตะจะไม่ได้รับดาเมจ
        if (Time.time < nextDamageTime) return;

        // 2. [แทรกระบบโล่เพิ่มตรงนี้] ถ้ากำลังตั้งการ์ดอยู่ ให้ลดทอนดาเมจก่อน
        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (combat != null && combat.isBlocking)
        {
            amount *= 0.3f; // ลดดาเมจลง 70% (โดนจริงแค่ 30%)
            bool hasStamina = UseStamina(15f); // หัก Stamina จากแรงกระแทกที่บล็อกไว้

            Debug.Log("<color=blue>🛡️ ยกโล่กันไว้ได้! ดาเมจลดเหลือ: " + amount + "</color>");

            // ถ้า Stamina หมดจากการกันรอบนี้ สั่งให้การ์ดแตกทันที
            if (!hasStamina || currentStamina <= 0)
            {
                combat.StopBlocking();
                Debug.Log("<color=orange>⚠️ การ์ดแตก! Stamina หมด</color>");
            }
        }

        // 3. ลดเลือดและ Clamp ค่าไว้ไม่ให้เกิน Max หรือต่ำกว่า 0 (ของเดิม)
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        // 4. สั่งให้หลอดเลือดบนหน้าจอขยับตาม (ของเดิม)
        if (healthBarUI != null)
        {
            healthBarUI.SetHealth(currentHealth);
        }

        // 5. เช็กสถานะตาย (ของเดิม)
        if (currentHealth <= 0)
        {
            Debug.Log("Player Dead!");
            Die();
        }

        // 6. เซตเวลาอมตะ I-Frame (ของเดิม)
        nextDamageTime = Time.time + damageCooldown;
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina < amount) return false;
        currentStamina -= amount;
        return true;
    }

    // เพิ่ม UseMana ให้ consistent กับ UseStamina
    public bool UseMana(float amount)
    {
        if (currentMana < amount) return false;
        currentMana -= amount;
        return true;
    }
    void Die()
    {
        
        // ใส่พวกหน้าจอ Game Over หรือสั่งให้ตัวละครเล่นท่าล้มลงตรงนี้
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ตัวอย่าง: โหลดด่านใหม่ทันที
    }
}