using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    private PlayerCombat playerCombat;
    public HealthBar healthBarUI;
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
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        if (currentHealth <= 0) Debug.Log("Player Dead!");
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
}