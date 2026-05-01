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

        // เปิดตามชื่อ (เปลี่ยนคำใน "" ให้ตรงกับชื่อก้อนสีของนายนะ)
        if (weaponName.Contains("sword")) swordObject.SetActive(true);
        else if (weaponName.Contains("magic")) gunObject.SetActive(true);
        else if (weaponName.Contains("shield")) shieldObject.SetActive(true);
    }
}