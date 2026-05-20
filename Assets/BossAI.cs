using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    public float stopDistance = 2f;

    [Header("Phase 1 - Stats")]
    public float p1Speed = 3f;
    public float p1Cooldown = 2.5f;
    public float p1Damage = 10f;

    [Header("Phase 2 - Stats")]
    public float p2Speed = 5f;
    public float p2Cooldown = 1.5f;
    [Range(0, 100)] public float p2SpecialChance = 30f;
    public float p2NormalDamage = 20f;
    public float p2SpecialDamage = 35f;

    [Header("Phase 3 - Stats")]
    public float p3Cooldown = 0.8f;
    public float p3SkillDamage = 15f;

    private BossPhaseManager phaseManager;
    private float nextAttackTime;
    private float currentMoveSpeed;

    // --- เปลี่ยนมาเชื่อมกับสคริปต์จริงของนายตรงนี้! ---
    private CharacterStatus playerStatus;

    void Start()
    {
        phaseManager = GetComponent<BossPhaseManager>();
        currentMoveSpeed = p1Speed;

        if (player != null)
        {
            playerStatus = player.GetComponent<CharacterStatus>();
        }
    }

    void Update()
    {
        if (player == null) return;

        // 1. หันหน้าหาผู้เล่น
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction), 0.1f);
        }

        // 2. เช็กเบรกมือเฟส 3
        if (phaseManager != null && phaseManager.currentPhase == 3)
        {
            HandleAttackLogic(3);
            return;
        }

        // 3. ระบบเดินเข้าหา
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
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

            if (currentPhase == 1) nextAttackTime = Time.time + p1Cooldown;
            else if (currentPhase == 2) nextAttackTime = Time.time + p2Cooldown;
            else if (currentPhase == 3) nextAttackTime = Time.time + p3Cooldown;
        }
    }

    void DetermineAttack(int phase)
    {
        // เช็กกันเหนี่ยวนิดนึงเผื่อหาไม่เจอตอนเริ่มเกม
        if (playerStatus == null && player != null)
        {
            playerStatus = player.GetComponent<CharacterStatus>();
        }

        if (phase == 1)
        {
            Debug.Log("<color=white>Boss: [Phase 1] ตบเบาๆ แปะ!</color>");
            if (playerStatus != null) playerStatus.TakeDamage(p1Damage); // เรียกใช้ฟังก์ชันใน CharacterStatus ของนาย
        }
        else if (phase == 2)
        {
            if (randomValCheck() < p2SpecialChance)
            {
                Debug.Log("<color=orange>Boss: [Phase 2] สุ่มใช้สกิลพิเศษ!!</color>");
                if (playerStatus != null) playerStatus.TakeDamage(p2SpecialDamage);
            }
            else
            {
                Debug.Log("<color=yellow>Boss: [Phase 2] ตบหนัก!</color>");
                if (playerStatus != null) playerStatus.TakeDamage(p2NormalDamage);
            }
        }
        else if (phase == 3)
        {
            Debug.Log("<color=red>Boss: [Phase 3] สาดสกิลรัวๆ!</color>");
            if (playerStatus != null) playerStatus.TakeDamage(p3SkillDamage);
        }
    }

    // ฟังก์ชันช่วยสุ่มเลข
    float randomValCheck() => Random.Range(0f, 100f);
}