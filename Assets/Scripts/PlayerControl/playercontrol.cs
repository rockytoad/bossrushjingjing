using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
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
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        rb.freezeRotation = true;
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    // ฟังก์ชันนี้จะทำงานเมื่อกดปุ่ม Space (ที่เราตั้งชื่อว่า Dash)
    public void OnDash(InputValue value)
    {
        if (canDash && !isDashing)
        {
            StartCoroutine(DashRoutine());
        }
    }

    void Update()
    {
        if (isDashing) return; // ถ้ากำลังแดชอยู่ ไม่ต้องหันตามเมาส์
        LookAtMouse();
    }

    void FixedUpdate()
    {
        if (isDashing) return; // ถ้ากำลังแดชอยู่ ไม่ต้องคุมเดินปกติ

        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        rb.linearVelocity = moveDir * moveSpeed;
    }

    IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        // ทิศทางที่แดช: พุ่งไปข้างหน้า (ที่ตัวละครหันไปหาเมาส์)
        Vector3 dashDir = transform.forward;

        rb.linearVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        // รอ Cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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