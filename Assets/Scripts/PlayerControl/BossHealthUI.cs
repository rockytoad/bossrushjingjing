using UnityEngine;
using UnityEngine.UI;
using TMPro; // สำหรับใช้ TextMeshPro

public class BossHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public TextMeshProUGUI bossNameText;
    public float lerpSpeed = 3f; // ความเร็วหลอดไหล (ลดนุ่มๆ)

    private float targetHealth;

    // ฟังก์ชันสำหรับเปิดตัวบอส (เรียกใช้ตอนเริ่มสู้)
    public void SetupBoss(string name, float maxHP)
    {
        gameObject.SetActive(true); // เปิด UI
        bossNameText.text = name;
        healthSlider.maxValue = maxHP;
        healthSlider.value = 0; // เริ่มจาก 0 เพื่อทำ Animation วิ่งไปเต็ม
        targetHealth = maxHP;
    }

    // ฟังก์ชันอัปเดตเลือด
    public void UpdateBossHealth(float currentHP)
    {
        targetHealth = currentHP;
    }

    void Update()
    {
        // ทำให้หลอดค่อยๆ ขยับ (Smooth Slider)
        if (healthSlider.value != targetHealth)
        {
            healthSlider.value = Mathf.MoveTowards(healthSlider.value, targetHealth, lerpSpeed * Time.deltaTime * healthSlider.maxValue * 0.2f);
        }

        // ถ้าเลือดหมด ให้ปิด UI (หรือรอจังหวะบอสตายจบ Animation ก่อนค่อยปิด)
        if (targetHealth <= 0)
        {
            // ทำอะไรบางอย่าง เช่น ค่อยๆ จางหายไป
        }
    }
}
