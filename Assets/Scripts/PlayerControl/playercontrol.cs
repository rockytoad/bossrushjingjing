using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;
    private Vector2 moveInput;
    private Rigidbody rb;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main; // เก็บตัวแปรกล้องไว้ใช้
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        LookAtMouse(); // หันตามเมาส์ตลอดเวลา (ใช้ใน Update จะลื่นกว่า)
    }

    void FixedUpdate()
    {
        // การเคลื่อนที่ยังเป็น WASD 8 ทิศทางเหมือนเดิม
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        rb.linearVelocity = moveDir * moveSpeed;
    }

    void LookAtMouse()
    {
        // 1. ยิงลำแสงจากตำแหน่งเมาส์บนหน้าจอลงไปในโลก 3D
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // 2. สร้างระนาบ (Plane) เสมือนที่ความสูงของตัวละครเพื่อใช้รับลำแสง
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            // 3. หาจุดที่ลำแสงตัดกับพื้น
            Vector3 pointToLook = ray.GetPoint(rayDistance);

            // 4. สั่งให้ตัวละครหันหน้าไปที่จุดนั้น (ล็อกแกน Y ไว้ไม่ให้ตัวละครก้มหรือเงย)
            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
    }
}