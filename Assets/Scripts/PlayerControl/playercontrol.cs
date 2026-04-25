using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
        // ป้องกันไม่ให้ตัวละครล้ม
        rb.freezeRotation = true;
    }

    // รับค่าเดิน WASD
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    // รับค่าปุ่ม Dash (Space)
    public void OnDash(InputValue value)
    {
        if (canDash && !isDashing)
        {
            StartCoroutine(DashRoutine());
        }
    }

    // รับค่าปุ่ม Attack (คลิกซ้าย) ที่นายเพิ่งตั้งค่าไป!
    public void OnAttack(InputValue value)
    {
        if (value.isPressed && !isDashing)
        {
            Debug.Log("โจมตี! (คลิกซ้ายทำงานแล้ว)");
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
