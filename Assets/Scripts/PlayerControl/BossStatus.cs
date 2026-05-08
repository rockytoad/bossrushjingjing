using UnityEngine;

public class BossStatus : MonoBehaviour
{
    public string bossName = "Fucking Ratatui";
    public float maxHealth = 1000f;
    public float currentHealth;

    public BossHealthUI bossUI; // ลาก BossHealthPanel มาใส่ช่องนี้

    void Start()
    {
        currentHealth = maxHealth;
        // ปิด UI บอสไว้ก่อนตอนเริ่มเกม
        bossUI.gameObject.SetActive(false);
    }

    // เรียกฟังก์ชันนี้ผ่าน Event หรือ Trigger ตอนผู้เล่นเดินเข้าห้องบอส
    public void StartBossFight()
    {
        bossUI.SetupBoss(bossName, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        bossUI.UpdateBossHealth(currentHealth);

        if (currentHealth <= 0) Die();
    }

    void Die() {
        Destroy(gameObject);
    }
}
