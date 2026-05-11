using UnityEngine;

public class BossDamageDealer : MonoBehaviour
{
    public float damage = 15f; // ดาเมจที่บอสทำได้

    private void OnTriggerEnter(Collider other)
    {
        // 1. เช็กว่าสิ่งที่ชนคือผู้เล่นใช่ไหม
        if (other.CompareTag("Player"))
        {
            // 2. ดึงสคริปต์ CharacterStatus จากตัวผู้เล่นมา
            CharacterStatus playerStatus = other.GetComponent<CharacterStatus>();

            if (playerStatus != null)
            {
                // 3. สั่งให้ผู้เล่นรับดาเมจ (ระบบเลือดเราจะลด และหลอด UI จะอัปเดต)
                playerStatus.TakeDamage(damage);

                Debug.Log("บอสตีผู้เล่น! ดาเมจ: " + damage);
            }
        }
    }
}
