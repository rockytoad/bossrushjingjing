using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    [Header("Health System")]
    public float maxHp = 100f;
    public float currentHp;

    [Header("Mana System")]
    public float maxMana = 50f;
    public float currentMana;
    public float manaRegenRate = 10f;

    [Header("Stamina System")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;

    [Header("Combat Stats")]
    public float swordDamage = 15f;
    public float magicDamage = 20f;
    

    void Awake()
    {
        // ตั้งค่าเริ่มต้นให้เต็มถังตอนเริ่มเกม
        currentHp = maxHp;
        currentMana = maxMana;
        currentStamina = maxStamina;
    }

    void Update()
    {
        // ฟื้นฟู Stamina อัตโนมัติ (เหมือนเกมแนว Souls/Hades)
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        }
        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        }
    }

    // --- ฟังก์ชันดึงค่าดาเมจ (เอาไปใช้ใน BulletScript หรือ PlayerCombat) ---
    public float GetMagicDamage() => magicDamage;
    public float GetSwordDamage() => swordDamage;

    // --- ฟังก์ชันจัดการทรัพยากร ---
    public void TakeDamage(float amount)
    {
        currentHp -= amount;
        if (currentHp <= 0) Debug.Log("Player Dead!");
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            return true; // Stamina พอ
        }
        return false; // Stamina ไม่พอ
    }

    public void CardValueAdjust(int value)
    {
        Debug.Log("Adjusting Card Value by: " + value);
    }
}