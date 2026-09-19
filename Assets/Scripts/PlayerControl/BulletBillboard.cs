using UnityEngine;

public class BulletBillboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // บังคับให้หน้ากระสุนตั้งขึ้นหันเข้าหากล้องเสมอ
            transform.rotation = mainCamera.transform.rotation;
        }
    }

    // ฟังก์ชันสั่งหมุนรูป Sprite ตามองศาทิศทางยิง
    public void SetDirectionAngle(float angleZ)
    {
        // สั่งหมุนเฉพาะแกน Z ของตัว Sprite หรือ Visual กระสุน
        transform.Rotate(0, 0, angleZ, Space.Self);
    }
}