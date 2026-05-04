using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
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
        weaponManager = GetComponent<Weaponmanager>();
        playerCombat = GetComponent<PlayerCombat>();
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        rb.freezeRotation = true;
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
            playerCombat.OnHeavyAttack();         // กด → ดาบ/เวทย์ทำงาน
            playerCombat.OnHeavyAttackHeld();     // กด → โล่เริ่มป้องกัน
        }
        else
        {
            playerCombat.OnHeavyAttackReleased(); // ปล่อย → โล่หยุดป้องกัน
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

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3f);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("WeaponSelector"))
            {
                weaponManager.SwitchWeapon(hitCollider.gameObject.name);
                break;
            }
        }
    }

    void Update()
    {
        if (!isDashing) LookAtMouse();
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

        rb.linearVelocity = transform.forward * dashSpeed;
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
            Vector3 targetPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(targetPoint);
        }
    }
}