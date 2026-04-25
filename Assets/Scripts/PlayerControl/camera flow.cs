using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ลากตัวละคร Capsule มาใส่ช่องนี้
    public Vector3 offset = new Vector3(0, 10, -10); // ระยะห่างที่นายตั้งไว้ในข้อ 1

    void LateUpdate()
    {
        if (target != null)
        {
            // ให้กล้องเคลื่อนที่ตามตำแหน่งตัวละคร + ระยะห่าง
            transform.position = target.position + offset;
        }
    }
}
