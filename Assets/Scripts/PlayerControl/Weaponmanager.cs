using UnityEngine;

public class Weaponmanager : MonoBehaviour
{
    [Header("Weapon Models")]
    public GameObject swordObject;
    public GameObject staffObject;
    public GameObject shieldObject;

    public string currentWeaponType = "None";
    private PlayerCombat playerCombat;

    void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
        SwitchWeapon(currentWeaponType); // เริ่มเกมปุ๊บ สลับอาวุธทันที
    }

    public void SwitchWeapon(string weaponName)
    {
        // แปลงข้อความให้เป็นตัวพิมพ์เล็กทั้งหมดเพื่อป้องกันพิมพ์ผิด (เช่น Magic -> magic)
        string nameLower = weaponName.ToLower();

        // 1. ปิดอาวุธทั้งหมดก่อน (มือว่างเปล่า)
        if (swordObject) swordObject.SetActive(false);
        if (staffObject) staffObject.SetActive(false);
        if (shieldObject) shieldObject.SetActive(false);

        // 2. ถ้าเป็น none ก็จบเลย ไม่ต้องทำต่อ
        if (nameLower == "none" || string.IsNullOrEmpty(nameLower))
        {
            currentWeaponType = "None";
            return;
        }

        // 3. ตรวจสอบการสลับอาวุธแบบแม่นยำ
        if (nameLower.Contains("sword"))
        {
            currentWeaponType = "Sword";
            if (swordObject)
            {
                swordObject.SetActive(true);
                if (playerCombat) playerCombat.swordCollider = swordObject.GetComponent<Collider>();
            }
        }
        else if (nameLower.Contains("magic") || nameLower.Contains("gun"))
        {
            currentWeaponType = "Magic";
            if (staffObject)
            {
                staffObject.SetActive(true);
            }
        }
        else if (nameLower.Contains("shield"))
        {
            currentWeaponType = "Shield";
            if (shieldObject)
            {
                shieldObject.SetActive(true);
                if (playerCombat) playerCombat.swordCollider = shieldObject.GetComponent<Collider>();
            }
        }
    }
}