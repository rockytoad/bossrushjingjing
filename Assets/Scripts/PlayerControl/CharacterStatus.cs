using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    private PlayerCombat playerCombat;
    [Header("Health")]
    public float maxHp = 100f;
    public float currentHp;

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
        currentHp = maxHp;
        currentMana = maxMana;
        currentStamina = maxStamina;
        playerCombat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        
        
            // Stamina regen เฉพาะตอนไม่ได้ป้องกัน
            if (currentStamina < maxStamina && !playerCombat.isBlocking)
                currentStamina = Mathf.Clamp(currentStamina + staminaRegenRate * Time.deltaTime, 0, maxStamina);

            // Mana regen ปกติ
            if (currentMana < maxMana)
                currentMana = Mathf.Clamp(currentMana + manaRegenRate * Time.deltaTime, 0, maxMana);
        
    }

    public float GetSwordDamage() => swordDamage;
    public float GetMagicDamage() => magicDamage;

    public void TakeDamage(float amount)
    {
        currentHp = Mathf.Clamp(currentHp - amount, 0, maxHp);
        if (currentHp <= 0) Debug.Log("Player Dead!");
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