using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Weaponmanager weaponManager;

    void Start()
    {
        // ดึงสคริปต์ Manager มาใช้งาน
        weaponManager = GetComponent<Weaponmanager>();
    }

    public void Attack()
    {
        // เช็กอาวุธจาก Manager
        string type = weaponManager.currentWeaponType;

        if (type.Contains("Red")) SwordAttack();
        else if (type.Contains("Green")) GunAttack();
        else if (type.Contains("Blue")) SpearAttack();
    }

    void SwordAttack() { Debug.Log("ฟันดาบ!"); }
    void GunAttack() { Debug.Log("ยิงปืน!"); }
    void SpearAttack() { Debug.Log("แทงหอก!"); }
}
