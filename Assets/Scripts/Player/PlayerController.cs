using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float maxSpeed = 6f;

    [Header("Cámara")]
    public Camera playerCamera;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    private bool flashlightActive = true;
    public GameObject ambientLight;

    [Header("General")]
    [SerializeField] private Difficulty difficulty;

    private Rigidbody rb;
    private float xRotation = 0f;
    private Vector3 input;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ambientLight.SetActive(false);
    }

    void Update()
    {
        ReadInput();
        HandleMouseLook();
        HandleFlashlightToggle();
    }

    void FixedUpdate()
    {
        Move();
    }

    void HandleFlashlightToggle()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlightActive = !flashlightActive;
            playerCamera.transform.GetChild(0).gameObject.SetActive(flashlightActive);
        }
    }

    void ReadInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        input = (transform.right * x + transform.forward * z).normalized;
    }

    void Move()
    {
        Vector3 targetVelocity = input * moveSpeed;
        Vector3 velocity = rb.linearVelocity;

        Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0f, velocity.z);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public Difficulty GetDifficulty() => difficulty;
    public bool GetFlashlightActive() => flashlightActive;
}
