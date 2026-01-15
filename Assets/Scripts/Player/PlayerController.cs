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

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepsSource;
    [SerializeField] private float footstepsVolume = 0.5f;
    [SerializeField] private float footstepsFadeSpeed = 5f;

    private Rigidbody rb;
    private float xRotation = 0f;
    private Vector3 input;
    private const string DIFFICULTY_KEY = "GameDifficulty";
    private bool flashlightDead = false;

    void Awake()
    {
        difficulty = (Difficulty)PlayerPrefs.GetInt(DIFFICULTY_KEY, (int)Difficulty.Normal);
        Debug.Log("Dificultad seleccionada: " + difficulty);
        ApplyDifficulty();
    }

    void ApplyDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.Easy: break;
            case Difficulty.Normal: break;
            case Difficulty.Hard: break;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // importante para que la física no gire el jugador

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ambientLight.SetActive(false);

        if (footstepsSource != null)
        {
            footstepsSource.volume = 0f;
            footstepsSource.loop = true;
            footstepsSource.Play();
        }
    }

    void Update()
    {
        ReadInput();
        HandleMouseLook();

        if (!flashlightDead)
            HandleFlashlightToggle();
    }

    void FixedUpdate()
    {
        Move();
        HandleFootsteps();
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
        // Mover usando Rigidbody.MovePosition para evitar conflictos de rotación
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 desiredVelocity = input * moveSpeed;
        Vector3 velocityChange = desiredVelocity - horizontalVelocity;

        // Aplicar fuerza solo en XZ
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Limitar velocidad máxima
        Vector3 clampedVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (clampedVelocity.magnitude > maxSpeed)
        {
            Vector3 limited = clampedVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }
    }

    void HandleMouseLook()
    {
        // Rotación de la cámara (pitch)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación del jugador (yaw)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, mouseX, 0f));
    }

    void HandleFootsteps()
    {
        if (footstepsSource == null) return;

        // Comprobar movimiento horizontal
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        bool isMoving = horizontalVelocity.magnitude > 0.1f;

        float targetVolume = isMoving ? footstepsVolume : 0f;
        footstepsSource.volume = Mathf.MoveTowards(
            footstepsSource.volume,
            targetVolume,
            footstepsFadeSpeed * Time.deltaTime
        );
    }

    public Difficulty GetDifficulty() => difficulty;
    public bool GetFlashlightActive() => flashlightActive;

    public void KillFlashlight()
    {
        flashlightDead = true;
        playerCamera.transform.GetChild(0).gameObject.SetActive(false);
    }
}
