using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = 9.81f;

    [Header("Mouse Ayarları")]
    public float mouseSensitivity = 2f;
    public float lookXLimit = 90f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;
    private float rotationX = 0;
    private float verticalVelocity = 0;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>(); // Animator karakterin içindeyse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // === Kamera Kontrolü ===
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(0, mouseX, 0);

        // === Hareket Girişi ===
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move *= moveSpeed;

        // === Animasyon: isWalking ===
        bool isWalking = (moveX != 0 || moveZ != 0);
        animator.SetBool("isWalking", isWalking);

        // === Zıplama ve Yerçekimi ===
        isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            animator.SetBool("isJumping", false); // yere indiğinde

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
                animator.SetBool("isJumping", true); // zıplarken
            }
            else
            {
                verticalVelocity = -1f; // yere yapışık kal
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;

        // === Hareket Uygulama ===
        controller.Move(move * Time.deltaTime);
    }
}
