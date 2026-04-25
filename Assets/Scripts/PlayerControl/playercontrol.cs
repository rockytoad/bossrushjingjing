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
            if (value.isPressed && !isDashing)
            {
                // ใช้คำสั่ง switch เพื่อแยกการทำงานตามสีอาวุธ
                switch (weaponType)
                {
                    case "sword":
                        SwordAttack();
                        break;
                    case "magic":
                        GunAttack();
                        break;
                    case "shield":
                        ShieldAttack();
                        break;
                    default:
                        Debug.Log("ยังไม่ได้เลือกอาวุธเลยนาย!");
                        break;
                }
            }
        }
    }
    // 1. ฟังก์ชันฟันดาบ (สีแดง)
    void SwordAttack()
    {
        Debug.Log("ฟันดาบยักษ์! (เน้นดาเมจรอบตัว)");
        // เดี๋ยวเราจะใส่โค้ดเปิด-ปิด Collider ที่นี่
    }

    // 2. ฟังก์ชันยิงปืน (สีเขียว)
    void GunAttack()
    {
        Debug.Log("ยิงปืนฉีดน้ำหวาน! (เน้นระยะไกล)");
        // เดี๋ยวเราจะใส่โค้ด Instantiate กระสุนที่นี่
    }

    // 3. ฟังก์ชันแทงหอก (สีน้ำเงิน)
    void ShieldAttack()
    {
        Debug.Log("โล่! (ข้างหน้า)");
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

    [Header("Interact Settings")]
    public float interactRange = 3f; // ระยะที่มือเอื้อมถึง
    public MeshRenderer handRenderer; // ช่องนี้เอาไว้ลาก "มือ" มาใส่ใน Inspector
    public string weaponType = "None";

    // ฟังก์ชันนี้จะทำงานเมื่อนายกดปุ่ม E (หรือปุ่มที่ตั้งไว้ใน Input Action)
    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            // 1. ตรวจสอบวัตถุรอบตัวในระยะ interactRange
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);

            foreach (var hitCollider in hitColliders)
            {
                // 2. ถ้าเจอของที่มี Tag ว่า WeaponSelector
                if (hitCollider.CompareTag("WeaponSelector"))
                {
                    // 3. หยิบอาวุธและเปลี่ยนสีมือ
                    weaponType = hitCollider.gameObject.name;
                    handRenderer.material = hitCollider.GetComponent<MeshRenderer>().material;

                    Debug.Log("หยิบอาวุธสำเร็จ! ตอนนี้ใช้: " + weaponType);
                    break; // หยิบได้ทีละชิ้นพอ
                }
            }
            
        }
        if (value.isPressed)
        {
            Debug.Log("กดปุ่ม E แล้วนะ!"); // ถ้ากดแล้วอันนี้ไม่ขึ้น แสดงว่าเป็นที่ Input Action

            // โค้ด OverlapSphere เดิมของนาย...
        }

    }
  
}
