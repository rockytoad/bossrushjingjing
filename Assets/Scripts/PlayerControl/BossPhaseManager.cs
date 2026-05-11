using UnityEngine;
using UnityEngine.AI;

public class BossPhaseManager : MonoBehaviour
{
    private BossStatus bossStatus; // ตัวแปรสำหรับเชื่อมกับสคริปต์เลือด
    private NavMeshAgent agent;

    [Header("Phase Settings")]
    public float phase2Threshold = 0.6f;
    public float phase3Threshold = 0.3f;
    public int currentPhase = 1;

    public bool isInvulnerable = false;

    void Start()
    {
        // เชื่อมต่อกับสคริปต์อื่นๆ ในตัวบอส
        bossStatus = GetComponent<BossStatus>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // ให้มันเช็กเฟสตลอดเวลาจากเลือดใน BossStatus
        if (bossStatus != null)
        {
            CheckPhaseTransition();
        }
    }

    void CheckPhaseTransition()
    {
        // เปลี่ยนมาใช้ bossStatus.currentHealth แทนการใช้ตัวแปรในนี้เอง
        float hpPercent = bossStatus.currentHealth / bossStatus.maxHealth;

        if (hpPercent <= phase3Threshold && currentPhase < 3)
        {
            SwitchToPhase(3);
        }
        else if (hpPercent <= phase2Threshold && currentPhase < 2)
        {
            SwitchToPhase(2);
        }
    }

    void SwitchToPhase(int phase)
    {
        currentPhase = phase;
        Debug.Log("Boss เข้าสู่ Phase: " + currentPhase);

        if (currentPhase == 3)
        {
            StartPhase3();
        }

        // ถ้า Phase 2 นายอยากให้บอสเดินเร็วขึ้นตามตาราง ก็ใส่ตรงนี้ได้
        if (currentPhase == 2 && agent != null)
        {
            agent.speed = 4.5f; // ค่าจากตาราง Boss_Core_Stats
        }
    }

    void StartPhase3()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        // สั่งให้ BossStatus เป็นอมตะ (ถ้าใน BossStatus มีระบบนี้)
        // หรือคุมผ่านบูลีนในนี้แล้วส่งค่าไป
        isInvulnerable = true;
        Debug.Log("บอสหยุดนิ่งและเป็นอมตะ 3 วินาที!");
        Invoke("DisableInvulnerable", 3f);
    }

    void DisableInvulnerable()
    {
        isInvulnerable = false;
        Debug.Log("หมดเวลาอมตะ!");
    }
}