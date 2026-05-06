using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider HealthSlider;
    public Slider ManaSlider;
    public Slider StaminaSlider; 

    // --- สุขภาพ ---
    public void SetMaxHealth(float Health) // เปลี่ยนเป็น float ให้ตรงกับ CharacterStatus
    {
        HealthSlider.maxValue = Health;
        HealthSlider.value = Health;
    }
    public void SetHealth(float Health) { HealthSlider.value = Health; }

    // --- มานา ---
    public void SetMaxMana(float Mana)
    {
        ManaSlider.maxValue = Mana;
        ManaSlider.value = Mana;
    }
    public void SetMana(float Mana) { ManaSlider.value = Mana; }

    // --- สเตมินา (เพิ่มใหม่) ---
    public void SetMaxStamina(float Stamina)
    {
        StaminaSlider.maxValue = Stamina;
        StaminaSlider.value = Stamina;
    }
    public void SetStamina(float Stamina) { StaminaSlider.value = Stamina; }
}
