using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour
{
    private bool forcedAwake;

    [Header("Movimiento")]
    [SerializeField] private GameObject player;

    [Header("Detección de luz")]
    [SerializeField] private Transform flashlight;
    [SerializeField] private float flashlightAngle = 100f;
    [SerializeField] private float flashlightDistance = 15f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Tipo de NPC")]
    [SerializeField] private EnemyCategory enemyCategory = EnemyCategory.LightTriggered;
    [SerializeField] private EnemyType enemyType = EnemyType.LightFear;

    [Header("Proximidad")]
    [SerializeField] private float activationDistance = 5f;

    private EnemyState state;
    private NavMeshAgent agent;
    PlayerController pc;

    [Header("Audio")]
    private bool hasPlayedAwakeSound = false;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip forcedWakeSound;
    [SerializeField] private AudioClip AwakingSound;
    
    [Header("Whispers Audio")]
    [SerializeField] private AudioSource whispersSource;
    [SerializeField] private AudioClip[] whispersClips;
    [SerializeField] private float whispersMaxVolume = 0.2f;
    [SerializeField] private float whispersFadeSpeed = 1f;
    [SerializeField] private float whispersMinDelay = 0.5f;
    [SerializeField] private float whispersMaxDelay = 2f;
    [SerializeField] private float whispersActivationDistance = 10f;

    [Header("General")]
    [SerializeField] private Difficulty difficulty;
    [SerializeField] private Material hardModeMaterial;
    [SerializeField] private GameObject skull;

    private void Awake()
    {
        CacheWakeSound();
    }

    private void OnEnable()
    {
        if (!sfxSource)
            CacheWakeSound();
    }

    private void CacheWakeSound()
    {
        sfxSource = GetComponentInChildren<AudioSource>(true);
    }

    void Start()
    {
        if (whispersSource == null)
        {
            Transform whispersTransform = transform.Find("WhispersAudio");
            if (whispersTransform != null)
            {
                whispersSource = whispersTransform.GetComponent<AudioSource>();
            }

            if (whispersSource == null)
            {
                Debug.LogError("No se encontró AudioSource de susurros en " + name);
                return;
            }
        }

        if (whispersSource != null && whispersClips.Length > 0)
        {
            // whispersSource.clip = whispersClip;
            whispersSource.volume = 0f;
            whispersSource.Play();
        }

        pc = FindFirstObjectByType<PlayerController>();

        if (pc != null)
        {
            player = pc.gameObject;
            difficulty = pc.GetDifficulty();
            StartCoroutine(PlayWhispersLoop());
        }
        else
        {
            Debug.LogError("PlayerController no encontrado", this);
        }

        if (flashlight == null)
        {
            var fl = FindFirstObjectByType<FlashlightController>();
            if (fl != null)
                flashlight = fl.transform;
        }

        if (difficulty == Difficulty.Hard && skull != null && hardModeMaterial != null)
        {
            var renderer = skull.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = hardModeMaterial;
            }
        }

        
        agent = GetComponent<NavMeshAgent>();

        switch (enemyCategory)
        {
            case EnemyCategory.AlwaysActive:
                state = EnemyState.Active;
                break;
            case EnemyCategory.LightTriggered:
            case EnemyCategory.ProximityTriggered:
                state = EnemyState.Inactive;
                break;
        }
    }

    void Update()
    {
        switch (state)
        {
            case EnemyState.Inactive:
                HandleInactiveState();
                break;

            case EnemyState.Active:
                HandleActiveState();
                break;
        }
    }
    
    void HandleInactiveState()
    {
        agent.isStopped = true;

        if (enemyCategory == EnemyCategory.LightTriggered && IsIlluminated())
        {
            // 🔊 SONAR SOLO LA PRIMERA VEZ
            if (!hasPlayedAwakeSound)
            {
                sfxSource.volume = .5f;
                sfxSource.PlayOneShot(AwakingSound);
                hasPlayedAwakeSound = true;
            }

            state = EnemyState.Active;
            agent.isStopped = false;
            agent.SetDestination(transform.position);
        }

        if (enemyCategory == EnemyCategory.ProximityTriggered)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= activationDistance)
            {
                state = EnemyState.Active;
                agent.isStopped = false;
                agent.SetDestination(transform.position);
            }
        }
    }

    void HandleActiveState()
    {
        bool illuminated = IsIlluminated();

        bool whispersActive = !illuminated && state == EnemyState.Active;

        if (whispersSource != null)
        {
            float targetVolume = whispersActive ? whispersMaxVolume : 0f;
            whispersSource.volume = Mathf.MoveTowards(
                whispersSource.volume,
                targetVolume,
                whispersFadeSpeed * Time.deltaTime
            );
        }

        if (forcedAwake && !illuminated)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            return;
        }


        if (illuminated && enemyType == EnemyType.LightFear)
        {
            agent.isStopped = true;
            return;
        }

        if (!illuminated && enemyType == EnemyType.DarkFear)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.transform.position);
    }

    bool IsIlluminated()
    {
        if (pc == null)
            return false;

        if (!pc.GetFlashlightActive())
            return false;

        if (flashlight == null)
            return false;

        Collider col = GetComponent<Collider>();
        if (col == null)
            return false;

        Vector3 targetPoint = col.bounds.center;
        Vector3 dirToNPC = targetPoint - flashlight.position;
        float distance = dirToNPC.magnitude;

        if (distance > flashlightDistance)
            return false;

        float angle = Vector3.Angle(flashlight.forward, dirToNPC);
        if (angle > flashlightAngle * 0.5f)
            return false;

        if (Physics.Raycast(
            flashlight.position,
            dirToNPC.normalized,
            out RaycastHit hit,
            distance,
            obstacleMask))
            return false;

        return true;
    }

    public bool IsInactive() => (state == EnemyState.Inactive);

    private bool hasBeenForcedAwake = false;

    public bool CanBeForcedAwake() => !hasBeenForcedAwake && state == EnemyState.Inactive;

    public void WakeUp()
    {
        if (hasBeenForcedAwake) return;

        sfxSource.volume = .1f;
        sfxSource.PlayOneShot(forcedWakeSound);

        hasBeenForcedAwake = true;
        state = EnemyState.Active;
        forcedAwake = true;
        agent.isStopped = false;

        if (player != null)
            agent.SetDestination(player.transform.position);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("💀 Jugador tocado por el enemigo: " + name);
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoseScene");
        }
    }

    private IEnumerator PlayWhispersLoop()
    {
        while (true)
        {
            bool playerClose = player != null && Vector3.Distance(transform.position, player.transform.position) <= whispersActivationDistance;
            bool whispersActive = playerClose && !IsIlluminated() && state == EnemyState.Active;

            // Fade del volumen
            float targetVolume = whispersActive ? whispersMaxVolume : 0f;
            whispersSource.volume = Mathf.MoveTowards(
                whispersSource.volume,
                targetVolume,
                whispersFadeSpeed * Time.deltaTime
            );

            // Reproducir clip si está activo y no se está reproduciendo nada
            if (whispersActive && !whispersSource.isPlaying)
            {
                AudioClip clip = whispersClips[Random.Range(0, whispersClips.Length)];
                whispersSource.clip = clip;
                whispersSource.Play();

                yield return new WaitForSeconds(clip.length);
                float delay = Random.Range(whispersMinDelay, whispersMaxDelay);
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
        }
    }
}
