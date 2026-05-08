using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    // ลากตัวบอส (ที่มีสคริปต์ BossStatus) มาใส่ช่องนี้
    public BossStatus boss;

    void OnTriggerEnter(Collider other)
    {
        // ถ้าผู้เล่นเดินเข้ามาชน
        if (other.CompareTag("Player"))
        {
            // สั่งให้บอสเริ่มโชว์ UI และเริ่มสู้
            boss.StartBossFight();

            // ทำลายตัว Trigger ทิ้ง จะได้ไม่ทำงานซ้ำเวลาเดินเข้าออก
            Destroy(gameObject);
        }
    }
}