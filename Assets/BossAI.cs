using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float stopDistance = 2f;

    // --- เพิ่มตรงนี้เพื่อเชื่อมกับ Manager ---
    private BossPhaseManager phaseManager;

    void Start()
    {
        phaseManager = GetComponent<BossPhaseManager>();
    }
    // ------------------------------------

    void Update()
    {
        if (player != null)
        {
            // 1. หันหน้าหาผู้เล่น (ให้หันหน้าได้ตลอด แม้จะหยุดเดิน)
            Vector3 direction = player.position - transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction), 0.1f);
            }

            // 2. เช็กเฟสก่อนเดิน! (สำคัญมาก)
            // ถ้าเป็นเฟส 3 ให้หยุดการทำงานข้างล่างนี้ทันที
            if (phaseManager != null && phaseManager.currentPhase == 3)
            {
                return; // จบการทำงานในเฟรมนี้ตรงนี้เลย ไม่ไปบรรทัดเดินข้างล่าง
            }

            // 3. ส่วนการเดินเดิมของนาย
            if (Vector3.Distance(transform.position, player.position) > stopDistance)
            {
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
            }
            else
            {
                Attack();
            }
        }
    }

    void Attack() { /* ... */ }
}