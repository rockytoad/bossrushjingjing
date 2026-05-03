using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // 1. ประกาศชื่อสมุดที่เราจะเรียกใช้
    private Weaponmanager weaponManager;
    private PlayerCombat playerCombat;
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing;
    private bool canDash = true;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Camera mainCamera;

    void Start()
    {
        // 2. สั่งให้มันไป "หยิบ" สคริปต์ที่แปะอยู่บนตัวละครเดียวกันมาเก็บไว้
        weaponManager = GetComponent<Weaponmanager>();
        playerCombat = GetComponent<PlayerCombat>();
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        // ป้องกันไม่ให้ตัวละครล้ม
        rb.freezeRotation = true;
    }
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    // รับค่าปุ่ม Dash (Space)
    public void OnDash(InputValue value)
    {
        if (canDash && !isDashing)
        {
            StartCoroutine(DashRoutine());
        }
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            // 3. เรียกใช้งานข้ามไฟล์ได้เลย! 
            // เหมือนสั่งว่า "เฮ้ เล่ม Combat ทำงานส่วนโจมตีซิ"
            playerCombat.OnLightAttack();
        }
    }
    public void OnHeavyAttack(InputValue value) // รับค่าจาก Action "HeavyAttack"
    {
        if (value.isPressed)
        {
            playerCombat.OnHeavyAttack(); // ส่งไปหาฟังก์ชัน OnHeavyAttack ใน PlayerCombat
        }
    }

    public void OnSkill(InputValue value) // รับค่าจาก Action "Skill"
    {
        if (value.isPressed)
        {
            playerCombat.OnSkill(); // ส่งไปหาฟังก์ชัน OnSkill ใน PlayerCombat
        }
    }
    void Update()
    {
        if (isDashing) return;
        LookAtMouse();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        rb.linearVelocity = moveDir * moveSpeed;
    }
    IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        Vector3 dashDir = transform.forward;
        rb.linearVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            // สร้างวงรัศมีตรวจจับ (อย่าลืมประกาศตัวแปรคลาส WeaponManager ไว้ที่ Start ด้วยนะ)
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3f);
            foreach (Collider hitCollider in hitColliders) // ต้องมีคำว่า Collider นำหน้า
            {
                if (hitCollider.CompareTag("WeaponSelector"))
                {
                    // สั่งข้ามไฟล์ไปที่ Manager
                    GetComponent<Weaponmanager>().SwitchWeapon(hitCollider.gameObject.name);
                    break;
                }
            }
        }
    }
    void LookAtMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPoint = hit.point;
            targetPoint.y = transform.position.y;
            transform.LookAt(targetPoint);
        }
    }


}
