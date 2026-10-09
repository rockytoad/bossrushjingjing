using UnityEngine;
using UnityEngine.AI; // ⚠️ ไลบรารีสำหรับใช้ NavMeshAgent

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

    // --- Component References ---
    private NavMeshAgent agent;
    private CharacterStatus playerStatus;
    private BossSkills bossSkills; // 👈 1. ประกาศตัวแปร bossSkills ตรงนี้แล้ว

    void Start()
    {
        phaseManager = GetComponent<BossPhaseManager>();
        agent = GetComponent<NavMeshAgent>();
        bossSkills = GetComponent<BossSkills>(); // 👈 2. ดึง Component BossSkills จากตัวบอส

        if (agent != null)
        {
            agent.stoppingDistance = stopDistance;
        }

        if (player != null)
        {
            playerStatus = player.GetComponent<CharacterStatus>();
        }
    }

    void Update()
    {
        // 👈 3. เช็กว่าบอสกำลังรำท่า / ชาร์จสกิลอยู่หรือไม่ (ใช้ b ตัวเล็ก)
        if (bossSkills != null && bossSkills.isExecutingSkill)
        {
            agent.isStopped = true;       // สั่งให้ NavMeshAgent หยุดเดินทันที
            agent.velocity = Vector3.zero; // ล้างความเร็วตกค้างกันบอสไถล
            return;                       // ตัดการทำงาน ไม่ให้รันโค้ดเดินตามด้านล่าง
        }

        if (player == null || agent == null) return;

        int currentPhase = (phaseManager != null) ? phaseManager.currentPhase : 1;

        // --- 1. เช็กเบรกมือเฟส 3 (หยุดเดิน แล้วสาดสกิลอย่างเดียว) ---
        if (currentPhase == 3)
        {
            agent.isStopped = true; // สั่งหยุดเดิน NavMesh
            LookAtPlayer();
            HandleAttackLogic(3);
            return;
        }

        // --- 2. อัปเดตความเร็ว NavMesh ตาม Phase ---
        agent.speed = (currentPhase == 2) ? p2Speed : p1Speed;

        // --- 3. สั่งให้ NavMeshAgent เดินอ้อมสิ่งกีดขวางไปหา Player ---
        agent.isStopped = false;
        agent.SetDestination(player.position);

        // --- 4. ระบบการโจมตีเมื่อถึงระยะ stopDistance ---
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= stopDistance)
        {
            LookAtPlayer(); // หันหน้าหาคนเล่นก่อนตบ
            HandleAttackLogic(currentPhase);
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction), 0.1f);
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
        if (playerStatus == null && player != null)
        {
            playerStatus = player.GetComponent<CharacterStatus>();
        }

        if (phase == 1)
        {
            Debug.Log("<color=white>Boss: [Phase 1] Bite Attack!</color>");
            if (bossSkills != null) StartCoroutine(bossSkills.Bite(p1Damage));
        }
        else if (phase == 2)
        {
            if (randomValCheck() < p2SpecialChance)
            {
                Debug.Log("<color=orange>Boss: [Phase 2] สุ่มใช้ Rushing / Slam!</color>");
                if (bossSkills != null)
                {
                    if (Random.value > 0.5f)
                        StartCoroutine(bossSkills.Rushing(12f, p2SpecialDamage));
                    else
                        StartCoroutine(bossSkills.Slam(p2SpecialDamage));
                }
            }
            else
            {
                Debug.Log("<color=yellow>Boss: [Phase 2] Bite!</color>");
                if (bossSkills != null) StartCoroutine(bossSkills.Bite(p2NormalDamage));
            }
        }
        else if (phase == 3)
        {
            Debug.Log("<color=red>Boss: [Phase 3] สาดสกิล Phase 3!</color>");
            if (bossSkills != null)
            {
                float rand = Random.value;
                if (rand < 0.33f) StartCoroutine(bossSkills.Jellythrust(p3SkillDamage));
                else if (rand < 0.66f) StartCoroutine(bossSkills.TentacleslamV1(p3SkillDamage));
                else StartCoroutine(bossSkills.TentacleslamV2(p3SkillDamage));
            }
        }
    }

    float randomValCheck() => Random.Range(0f, 100f);
}