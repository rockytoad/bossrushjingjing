using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Weaponmanager weaponManager;
    private PlayerCombat playerCombat;

    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Visual & Sprites")]
    public Transform playerVisual;      // ลาก playersprite มาใส่
    public SpriteRenderer spriteRenderer; // ลาก SpriteRenderer ของ playersprite มาใส่
    public Transform weaponPivot;

    [Space(10)]
    public Sprite spriteRight; // รูปหันขวา
    public Sprite spriteLeft;  // รูปหันซ้าย
    public Sprite spriteUp;    // รูปหันหลัง/ขึ้นบน (มีหรือไม่มีก็ได้)
    public Sprite spriteDown;  // รูปหันหน้า/ลงล่าง (มีหรือไม่มีก็ได้)

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing;
    private bool canDash = true;

    private Rigidbody rb;
    private Vector2 moveInput;

    void Start()
    {
        weaponManager = GetComponent<Weaponmanager>();
        playerCombat = GetComponent<PlayerCombat>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (spriteRenderer == null && playerVisual != null)
        {
            spriteRenderer = playerVisual.GetComponent<SpriteRenderer>();
        }
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    public void OnDash(InputValue value)
    {
        if (canDash && !isDashing)
            StartCoroutine(DashRoutine());
    }

    public void OnAttack(InputValue value)
    {
        if (value.Get<float>() > 0.5f)
            playerCombat.OnLightAttack();
    }

    public void OnHeavyAttack(InputValue value)
    {
        if (value.Get<float>() > 0.5f)
        {
            playerCombat.OnHeavyAttack();
            playerCombat.OnHeavyAttackHeld();
        }
        else
        {
            playerCombat.OnHeavyAttackReleased();
        }
    }

    public void OnSkill(InputValue value)
    {
        if (value.Get<float>() > 0.5f)
            playerCombat.OnSkill();
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        // กวาดหาระยะ 1.5f รอบตัว
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.5f);

        Collider closestWeapon = null;
        float minDistance = Mathf.Infinity;

        // วนลูปหาอันที่อยู่ใกล้ที่สุด
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("WeaponSelector"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestWeapon = hitCollider;
                }
            }
        }

        // สลับอาวุธชิ้นที่ใกล้ที่สุด
        if (closestWeapon != null)
        {
            weaponManager.SwitchWeapon(closestWeapon.gameObject.name);
        }
    }

    void Update()
    {
        UpdateCharacterSprite();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
    }

    IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        Vector3 dashDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        if (dashDir == Vector3.zero) dashDir = transform.forward;

        rb.linearVelocity = dashDir * dashSpeed;
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    void UpdateCharacterSprite()
    {
        if(spriteRenderer == null) return;

        // เช็กการกดปุ่มเปลี่ยนรูป + หมุนทิศทางการยิงกระสุน (weaponPivot)
        if (moveInput.x > 0.1f) // หันขวา
        {
            if (spriteRight != null) spriteRenderer.sprite = spriteRight;
            if (weaponPivot != null) weaponPivot.rotation = Quaternion.Euler(0, 90, 0); // หันไปทางขวา (แกน Y 90 องศา)
        }
        else if (moveInput.x < -0.1f) // หันซ้าย
        {
            if (spriteLeft != null) spriteRenderer.sprite = spriteLeft;
            if (weaponPivot != null) weaponPivot.rotation = Quaternion.Euler(0, -90, 0); // หันไปทางซ้าย (แกน Y -90 องศา)
        }
        else if (moveInput.y > 0.1f) // หันขึ้น/หลัง
        {
            if (spriteUp != null) spriteRenderer.sprite = spriteUp;
            if (weaponPivot != null) weaponPivot.rotation = Quaternion.Euler(0, 0, 0); // หันขึ้นข้างบน/ไปข้างหน้า
        }
        else if (moveInput.y < -0.1f) // หันลง/หน้า
        {
            if (spriteDown != null) spriteRenderer.sprite = spriteDown;
            if (weaponPivot != null) weaponPivot.rotation = Quaternion.Euler(0, 180, 0); // หันลงข้างล่าง
        }
    }
}