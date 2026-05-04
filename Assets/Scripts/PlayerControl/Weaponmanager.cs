using UnityEngine;

public class Weaponmanager : MonoBehaviour
{
    [Header("Weapon Models")]
    public GameObject swordObject;
    public GameObject gunObject;
    public GameObject shieldObject;

    public string currentWeaponType = "None";

    public void SwitchWeapon(string weaponName)
    {
        currentWeaponType = weaponName;

        // ปิดอาวุธทุกชิ้นก่อน
        if (swordObject) swordObject.SetActive(false);
        if (gunObject) gunObject.SetActive(false);
        if (shieldObject) shieldObject.SetActive(false);

        // เปิดตามชื่อ พร้อม null check
        if (weaponName.Contains("sword") && swordObject) swordObject.SetActive(true);
        else if (weaponName.Contains("magic") && gunObject) gunObject.SetActive(true);
        else if (weaponName.Contains("shield") && shieldObject) shieldObject.SetActive(true);
        else Debug.LogWarning("ไม่พบอาวุธชื่อ: " + weaponName);
    }
}