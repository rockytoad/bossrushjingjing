using UnityEngine;

public class BossStatus : MonoBehaviour
{
    public string bossName = "Fucking Ratatui";
    public float maxHealth = 1000f;
    public float currentHealth;

    public BossHealthUI bossUI;

    // เพิ่มตรงนี้: เพื่อดึงสถานะอมตะมาเช็ก
    private BossPhaseManager phaseManager;

    void Start()
    {
        currentHealth = maxHealth;
        phaseManager = GetComponent<BossPhaseManager>(); // เชื่อมต่อกับ Manager

        if (bossUI != null) bossUI.gameObject.SetActive(false);
    }

    public void StartBossFight()
    {
        if (bossUI != null)
        {
            bossUI.gameObject.SetActive(true);
            bossUI.SetupBoss(bossName, maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        // --- ส่วนสำคัญ: เช็กว่า Manager สั่งให้อมตะอยู่ไหม ---
        if (phaseManager != null && phaseManager.isInvulnerable)
        {
            Debug.Log("โจมตีไม่เข้า! บอสกำลังอยู่ในช่วงอมตะเปลี่ยนเฟส");
            return;
        }
        // ---------------------------------------------

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

        if (bossUI != null) bossUI.UpdateBossHealth(currentHealth);

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Debug.Log(bossName + " ตายแล้ว!");
        // อาจจะใส่ Effect ระเบิดตรงนี้ก่อน Destroy
        Destroy(gameObject);
    }
}