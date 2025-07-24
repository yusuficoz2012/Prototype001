using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpForce = 5f;
    public float gravity = 9.81f;

    [Header("Kamera Ayarları")]
    public Transform cameraTransform;
    public float mouseSensitivity = 3f;
    public Vector3 cameraOffset = new Vector3(0f, 2f, -4f);
    public float minPitch = -30f;
    public float maxPitch = 60f;

    [Header("Animasyon")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 moveDirection;
    private float verticalVelocity;

    private float yaw;   // Yatay kamera açısı
    private float pitch; // Dikey kamera açısı

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        Vector3 angles = cameraTransform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Hareket inputları
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);

            
        }

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Kamera yönünü yatay düzleme indir
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 move = (forward * moveZ + right * moveX).normalized;

        if (move.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Zıplama & yerçekimi
        if (controller.isGrounded)
        {
            verticalVelocity = -1f;
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
                if (animator != null)
                    animator.SetBool("isJumping", true);
            }
            else
            {
                if (animator != null)
                    animator.SetBool("isJumping", false);
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        moveDirection = move * speed;
        moveDirection.y = verticalVelocity;

        controller.Move(moveDirection * Time.deltaTime);

        // Kamera kontrolü
        bool rightClickHeld = Input.GetMouseButton(1);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (rightClickHeld)
        {
            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        // Bırakınca açıyı koru, otomatik dönüş yok

        cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0);
        cameraTransform.position = transform.position + cameraTransform.rotation * cameraOffset;
    }
}
