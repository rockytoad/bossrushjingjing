using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkills : MonoBehaviour
{
    [Header("Target & Layers")]
    public Transform player;
    public LayerMask playerLayer;
    public LayerMask wallLayer;

    [Header("Telegraph Prefabs (แถบเตือนบนพื้น)")]
    public GameObject lineTelegraphPrefab;     // แถบแดงสี่เหลี่ยมยาว (Rushing / Bite / Tentacle)
    public GameObject circleTelegraphPrefab;   // วงกลมแดงเตือน (Tentacle V1/V2)
    public GameObject coneTelegraphPrefab;     // รูปกรวย 90 องศา (Jellythrust)
    public GameObject shadowTelegraphPrefab;   // เงาสีดำขยายบนพื้น (Slam)

    [Header("Global SFX Setup")]
    public AudioSource audioSource;

    [Header("1. Bite VFX & SFX")]
    public GameObject biteVFXPrefab;           // ประกายแสงสีขาวตอนกัด
    public AudioClip biteSFX;                  // เสียงกัด/ของแหลมคม

    [Header("2. Rushing VFX & SFX")]
    public GameObject stunBirdsVFXPrefab;      // นกบินวนบนหัวตอนติด Stun
    public AudioClip growlSFX;                 // เสียงขู่ตั้งท่า
    public AudioClip dashSFX;                  // เสียงพุ่งแหวกอากาศ
    public AudioClip stunBirdsSFX;             // เสียงนกมึนหัว

    [Header("3. Slam VFX & SFX")]
    public GameObject shockwaveFXPrefab;       // วงคลื่นกระแทกสีขาวตอนบอสตกถึงพื้น
    public AudioClip windUpSFX;                 // เสียงลมพุ่งขึ้นตอนกระโดด
    public AudioClip whistleSFX;                // เสียงหวีดสูงตอนตกลงมา
    public AudioClip explosionSFX;              // เสียงระเบิดตอนกระแทกพื้น

    [Header("4. Jellythrust VFX & SFX")]
    public GameObject jellyThrustVFXPrefab;    // Particle หนวด 5 เส้นพุ่งออกจากปาก
    public AudioClip jellyThrustSFX;           // เสียงเยลลี่ดิ้น / เสียงอ้วก

    [Header("5. Tentacle Slam VFX & SFX")]
    public GameObject tentacleImpactVFXPrefab; // รอยแตกบนพื้น / เศษเยลลี่กระจาย
    public AudioClip whipSFX;                  // เสียงแส้ฟาดแหวกอากาศ
    public AudioClip tentacleImpactSFX;        // เสียงฟาดพื้นหนักๆ

    [Header("Skill Status")]
    public bool isExecutingSkill = false;       // ตัวแปรเช็กว่าบอสกำลังรำท่าสกิลอยู่ไหม

    [Header("Phase 3 Tentacle Setup")]
    public GameObject tentaclePrefab;           // Prefab ของหนวดที่จะโผล่จากพื้น
    public float tentacleSpawnRadius = 5f;       // รัศมีระยะห่างจากตัวบอส
    private List<GameObject> activeTentacles = new List<GameObject>();

    private BossPhaseManager phaseManager;

    void Start()
    {
        phaseManager = GetComponent<BossPhaseManager>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    // =========================================================================
    // ⚔️ PHASE 1 & 2 SKILLS
    // =========================================================================

    public IEnumerator Bite(float damage)
    {
        isExecutingSkill = true;
        Debug.Log("🐉 [Bite] บอสเริ่มอ้าปากชาร์จ...");

        Vector3 targetDir = (player.position - transform.position).normalized;
        targetDir.y = 0;
        transform.rotation = Quaternion.LookRotation(targetDir);

        yield return new WaitForSeconds(0.4f);

        if (audioSource != null && biteSFX != null)
            audioSource.PlayOneShot(biteSFX);

        if (Vector3.Distance(transform.position, player.position) <= 2.5f)
        {
            if (biteVFXPrefab != null)
                Instantiate(biteVFXPrefab, player.position + Vector3.up * 1f, Quaternion.identity);

            CharacterStatus pStatus = player.GetComponent<CharacterStatus>();
            if (pStatus != null) pStatus.TakeDamage(damage);
            Debug.Log($"💥 [Bite] งับโดนผู้เล่น! ดาเมจ: {damage}");
        }

        yield return new WaitForSeconds(0.3f);
        isExecutingSkill = false;
    }

    public IEnumerator Rushing(float rushSpeed, float damage)
    {
        isExecutingSkill = true;
        Debug.Log("🐗 [Rushing] บอสโก้งโค้งล็อกเป้าเตรียมพุ่ง!");

        Vector3 rushDir = (player.position - transform.position).normalized;
        rushDir.y = 0;
        transform.rotation = Quaternion.LookRotation(rushDir);

        if (audioSource != null && growlSFX != null)
            audioSource.PlayOneShot(growlSFX);

        GameObject lineVFX = null;
        if (lineTelegraphPrefab != null)
            lineVFX = Instantiate(lineTelegraphPrefab, transform.position, Quaternion.LookRotation(rushDir));

        yield return new WaitForSeconds(1.0f);
        if (lineVFX != null) Destroy(lineVFX);

        if (audioSource != null && dashSFX != null)
            audioSource.PlayOneShot(dashSFX);

        float maxRushTime = 1.5f;
        float timer = 0f;
        bool hitWall = false;

        while (timer < maxRushTime && !hitWall)
        {
            transform.position += rushDir * rushSpeed * Time.deltaTime;
            timer += Time.deltaTime;

            Collider[] hitPlayers = Physics.OverlapSphere(transform.position, 1.8f, playerLayer);
            if (hitPlayers.Length > 0)
            {
                CharacterStatus pStatus = hitPlayers[0].GetComponent<CharacterStatus>();
                if (pStatus != null) pStatus.TakeDamage(damage);
                Debug.Log($"💥 [Rushing] พุ่งชนผู้เล่นเต็มแรง! ดาเมจ: {damage}");
            }

            Collider[] hitWalls = Physics.OverlapSphere(transform.position, 1.8f, wallLayer);
            if (hitWalls.Length > 0)
            {
                hitWall = true;
            }

            yield return null;
        }

        if (hitWall)
        {
            Debug.Log("<color=yellow>💫 [Rushing] บอสพุ่งชนกำแพง! ติด Stun 3 วินาที!</color>");

            GameObject birdsVFX = null;
            if (stunBirdsVFXPrefab != null)
                birdsVFX = Instantiate(stunBirdsVFXPrefab, transform.position + Vector3.up * 2.5f, Quaternion.identity, transform);

            if (audioSource != null && stunBirdsSFX != null)
                audioSource.PlayOneShot(stunBirdsSFX);

            yield return new WaitForSeconds(3.0f);

            if (birdsVFX != null) Destroy(birdsVFX);
        }

        isExecutingSkill = false;
    }

    public IEnumerator Slam(float damage)
    {
        isExecutingSkill = true;
        Debug.Log("💥 [Slam] บอสกระโดดขึ้นฟ้า!");

        Vector3 originalPos = transform.position;
        Vector3 jumpPos = originalPos + Vector3.up * 8f;
        transform.position = jumpPos;

        if (audioSource != null && windUpSFX != null)
            audioSource.PlayOneShot(windUpSFX);

        Vector3 landTarget = new Vector3(player.position.x, player.position.y + 0.02f, player.position.z);

        GameObject shadowVFX = null;
        GameObject prefabToUse = shadowTelegraphPrefab != null ? shadowTelegraphPrefab : circleTelegraphPrefab;

        if (prefabToUse != null)
        {
            shadowVFX = Instantiate(prefabToUse, landTarget, Quaternion.identity);
            shadowVFX.transform.localScale = Vector3.zero;
        }

        if (audioSource != null && whistleSFX != null)
            audioSource.PlayOneShot(whistleSFX);

        float hoverDuration = 1.2f;
        float elapsedTime = 0f;
        Vector3 targetScale = new Vector3(4.5f, 0.01f, 4.5f);

        while (elapsedTime < hoverDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / hoverDuration;

            if (shadowVFX != null)
                shadowVFX.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);

            yield return null;
        }

        if (shadowVFX != null) Destroy(shadowVFX);

        transform.position = landTarget;

        if (shockwaveFXPrefab != null)
            Instantiate(shockwaveFXPrefab, landTarget, Quaternion.identity);

        if (audioSource != null && explosionSFX != null)
            audioSource.PlayOneShot(explosionSFX);

        Collider[] hitPlayers = Physics.OverlapSphere(landTarget, 4.5f, playerLayer);
        foreach (var p in hitPlayers)
        {
            CharacterStatus pStatus = p.GetComponent<CharacterStatus>();
            if (pStatus != null) pStatus.TakeDamage(damage);
            Debug.Log($"💥 [Slam] Shockwave กระแทกโดนผู้เล่น! ดาเมจ: {damage}");
        }

        yield return new WaitForSeconds(0.5f);
        isExecutingSkill = false;
    }

    // =========================================================================
    // 🐙 PHASE 3 SKILLS
    // =========================================================================

    public void SpawnTentaclesAround()
    {
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(0, 0, tentacleSpawnRadius),
            new Vector3(0, 0, -tentacleSpawnRadius),
            new Vector3(tentacleSpawnRadius, 0, 0),
            new Vector3(-tentacleSpawnRadius, 0, 0)
        };

        foreach (Vector3 offset in offsets)
        {
            Vector3 spawnPos = transform.position + offset;

            if (tentaclePrefab != null)
            {
                GameObject tentacle = Instantiate(tentaclePrefab, spawnPos, Quaternion.identity);
                activeTentacles.Add(tentacle);
            }
        }

        Debug.Log("<color=green>🐙 เสกหนวด 4 เส้นขึ้นมารอบตัวบอสเรียบร้อย!</color>");
    }

    public IEnumerator Jellythrust(float damage)
    {
        isExecutingSkill = true;
        Debug.Log("🐙 [Jellythrust] บอสชาร์จหนวดในปาก...");

        Vector3 targetDir = (player.position - transform.position).normalized;
        targetDir.y = 0;
        transform.rotation = Quaternion.LookRotation(targetDir);

        GameObject coneVFX = null;
        if (coneTelegraphPrefab != null)
            coneVFX = Instantiate(coneTelegraphPrefab, transform.position, Quaternion.LookRotation(targetDir));

        yield return new WaitForSeconds(1.5f);
        if (coneVFX != null) Destroy(coneVFX);

        if (audioSource != null && jellyThrustSFX != null)
            audioSource.PlayOneShot(jellyThrustSFX);

        if (jellyThrustVFXPrefab != null)
            Instantiate(jellyThrustVFXPrefab, transform.position + transform.forward * 1f, Quaternion.LookRotation(targetDir));

        // เช็กระยะและมุมหนวด 5 เส้น (-45, -22.5, 0, 22.5, 45 องศา) เพื่อให้มี Safe Zone ระหว่างหนวด
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= 8f)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.SignedAngle(transform.forward, dirToPlayer, Vector3.up);

            float[] tentacleAngles = new float[] { -45f, -22.5f, 0f, 22.5f, 45f };
            bool isHitByTentacle = false;

            foreach (float targetAngle in tentacleAngles)
            {
                // โดนดาเมจเฉพาะเมื่อยืนตรงทิศทางหนวด (+- 4 องศา)
                if (Mathf.Abs(angleToPlayer - targetAngle) <= 4f)
                {
                    isHitByTentacle = true;
                    break;
                }
            }

            if (isHitByTentacle)
            {
                CharacterStatus pStatus = player.GetComponent<CharacterStatus>();
                if (pStatus != null) pStatus.TakeDamage(damage);
                Debug.Log($"💥 [Jellythrust] โดนหนวดแทงเต็มๆ! ดาเมจ: {damage}");
            }
        }

        yield return new WaitForSeconds(0.8f);
        isExecutingSkill = false;
    }

    public IEnumerator TentacleslamV1(float damage)
    {
        isExecutingSkill = true;
        Debug.Log("🐙 [Tentacleslam V1] ฟาดหนวดใส่ผู้เล่น!");

        Vector3 targetPos = player.position;
        Vector3 dirToTarget = (targetPos - transform.position).normalized;
        dirToTarget.y = 0;

        if (audioSource != null && whipSFX != null)
            audioSource.PlayOneShot(whipSFX);

        GameObject lineVFX = null;
        if (lineTelegraphPrefab != null)
            lineVFX = Instantiate(lineTelegraphPrefab, transform.position, Quaternion.LookRotation(dirToTarget));

        yield return new WaitForSeconds(1.0f);
        if (lineVFX != null) Destroy(lineVFX);

        if (audioSource != null && tentacleImpactSFX != null)
            audioSource.PlayOneShot(tentacleImpactSFX);

        if (tentacleImpactVFXPrefab != null)
            Instantiate(tentacleImpactVFXPrefab, targetPos, Quaternion.identity);

        if (Vector3.Distance(transform.position, player.position) <= 10f)
        {
            CharacterStatus pStatus = player.GetComponent<CharacterStatus>();
            if (pStatus != null) pStatus.TakeDamage(damage);
            Debug.Log($"💥 [Tentacleslam V1] หนวดฟาดโดนผู้เล่น! ดาเมจ: {damage}");
        }

        yield return new WaitForSeconds(0.5f);
        isExecutingSkill = false;
    }

    public IEnumerator TentacleslamV2(float damage)
    {
        isExecutingSkill = true;
        Debug.Log("🐙 [Tentacleslam V2] ฟาดหนวดคอมโบ + สปอว์นหนวดรอบสนาม!");

        yield return StartCoroutine(TentacleslamV1(damage));

        isExecutingSkill = true;

        activeTentacles.RemoveAll(t => t == null);

        if (activeTentacles.Count < 4)
        {
            SpawnTentaclesAround();
        }

        yield return new WaitForSeconds(0.5f);
        isExecutingSkill = false;
    }
    void Update()
    {
        // กดปุ่มเลข 1-6 บนคีย์บอร์ดเพื่อสั่งออกสกิลทดสอบทันที
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isExecutingSkill) StartCoroutine(Bite(10f));
        if (Input.GetKeyDown(KeyCode.Alpha2) && !isExecutingSkill) StartCoroutine(Rushing(10f, 20f));
        if (Input.GetKeyDown(KeyCode.Alpha3) && !isExecutingSkill) StartCoroutine(Slam(30f));
        if (Input.GetKeyDown(KeyCode.Alpha4) && !isExecutingSkill) StartCoroutine(Jellythrust(25f));
        if (Input.GetKeyDown(KeyCode.Alpha5) && !isExecutingSkill) StartCoroutine(TentacleslamV1(20f));
        if (Input.GetKeyDown(KeyCode.Alpha6) && !isExecutingSkill) StartCoroutine(TentacleslamV2(20f));
    }
}