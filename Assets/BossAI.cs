using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player; // ลากตัวละครมาใส่ตรงนี้
    public float stopDistance = 2f;

    [Header("Phase 1 - Start")]
    public float p1Speed = 3f;
    public float p1Cooldown = 2.5f;

    [Header("Phase 2 - Angry")]
    public float p2Speed = 5f;
    public float p2Cooldown = 1.5f;
    [Range(0, 100)] public float p2SpecialChance = 30f; // โอกาสใช้สกิลพิเศษ (0-100)

    [Header("Phase 3 - Final")]
    public float p3Cooldown = 0.8f; // หยุดเดินแต่โจมตีรัวมาก

    // ตัวแปรที่ใช้ทำงานภายใน
    private BossPhaseManager phaseManager;
    private float nextAttackTime;
    private float currentMoveSpeed;

    void Start()
    {
        phaseManager = GetComponent<BossPhaseManager>();
        currentMoveSpeed = p1Speed; // เริ่มต้นด้วยความเร็วเฟส 1
    }

    void Update()
    {
        if (player == null) return;

        // 1. หันหน้าหาผู้เล่น (ทำตลอดทุกเฟส)
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction), 0.1f);
        }

        // 2. เช็กเบรกมือเฟส 3 (ถ้าเฟส 3 ให้หยุดเดินทันที)
        if (phaseManager != null && phaseManager.currentPhase == 3)
        {
            HandleAttackLogic(3); // ไปเช็กโจมตีอย่างเดียว ไม่เดิน
            return;
        }

        // 3. ระบบการเดินและโจมตี (เฟส 1 และ 2)
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            // ปรับความเร็วตามเฟสปัจจุบัน
            currentMoveSpeed = (phaseManager.currentPhase == 2) ? p2Speed : p1Speed;
            transform.position += direction.normalized * currentMoveSpeed * Time.deltaTime;
        }
        else
        {
            HandleAttackLogic(phaseManager.currentPhase);
        }
    }

    void HandleAttackLogic(int currentPhase)
    {
        if (Time.time >= nextAttackTime)
        {
            DetermineAttack(currentPhase);

            // ตั้งเวลาพัก (Cooldown) ตามเฟส
            if (currentPhase == 1) nextAttackTime = Time.time + p1Cooldown;
            else if (currentPhase == 2) nextAttackTime = Time.time + p2Cooldown;
            else if (currentPhase == 3) nextAttackTime = Time.time + p3Cooldown;
        }
    }

    void DetermineAttack(int phase)
    {
        float randomVal = Random.Range(0f, 100f);

        if (phase == 1)
        {
            Debug.Log("<color=white>Boss: [Phase 1] ตบเบาๆ แปะ!</color>");
        }
        else if (phase == 2)
        {
            if (randomVal < p2SpecialChance)
                Debug.Log("<color=orange>Boss: [Phase 2] สุ่มใช้สกิลพิเศษ!!</color>");
            else
                Debug.Log("<color=yellow>Boss: [Phase 2] ตบหนัก!</color>");
        }
        else if (phase == 3)
        {
            if (randomVal < 60)
                Debug.Log("<color=red>Boss: [Phase 3] ปล่อยสกิลรัวๆ!</color>");
            else
                Debug.Log("<color=red>Boss: [Phase 3] ตบกวาดพื้น!</color>");
        }
    }
}