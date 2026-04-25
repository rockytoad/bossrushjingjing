using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform player; // ลาก Capsule ตัวละครเรามาใส่ช่องนี้
    public float moveSpeed = 3f;
    public float stopDistance = 2f; // ระยะที่จะหยุดรอโจมตี

    void Update()
    {
        if (player != null)
        {
            // 1. คำนวณทิศทางหาผู้เล่น
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // ล็อกแกน Y ไม่ให้บอสลอยหรือจมพื้น

            // 2. หันหน้าไปหาผู้เล่น
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction), 0.1f);
            }

            // 3. เดินเข้าไปหาถ้ายังอยู่ไกล
            if (Vector3.Distance(transform.position, player.position) > stopDistance)
            {
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
            }
            else
            {
                // ถ้าใกล้พอแล้ว ให้บอส "ทำอะไรสักอย่าง" (เช่น ตบ หรือ ปล่อยหนวด)
                Attack();
            }
        }
    }

    void Attack()
    {
        // เดี๋ยวเรามาเขียน Logic การโจมตีตรงนี้ต่อ
        Debug.Log("Boss: ย้ากกก ตบด้วยหนวดวุ้น!");
    }
}
