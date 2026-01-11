using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private GameObject player;

    [Header("Detección de luz")]
    [SerializeField] private Transform flashlight;
    [SerializeField] private float flashlightAngle = 100f;
    [SerializeField] private float flashlightDistance = 15f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Tipo de NPC")]
    [SerializeField] private EnemyCategory enemyCategory = EnemyCategory.AlwaysActive;
    [SerializeField] private EnemyType enemyType = EnemyType.LightFear;

    [Header("Proximidad")]
    [SerializeField] private float activationDistance = 5f;

    private EnemyState state;
    private NavMeshAgent agent;
    PlayerController pc;

    [Header("General")]
    [SerializeField] private Difficulty difficulty;
    [SerializeField] private Material hardModeMaterial;
    [SerializeField] private GameObject skull;
    [SerializeField] private int flankThreshold = 3;
    [SerializeField] private float flankDistance = 3f;
    private bool isFlanker;


    void Start()
    {
        pc = FindFirstObjectByType<PlayerController>();

        if (pc != null)
        {
            player = pc.gameObject;
            difficulty = pc.GetDifficulty();
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
        // agent.SetDestination(player.transform.position);
        if (isFlanker)
            agent.SetDestination(GetGroupFlankPosition());
        else
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

    int CountNearbyEnemies(float radius)
    {
        Collider[] hits = Physics.OverlapSphere(
            player.transform.position,
            radius,
            LayerMask.GetMask("Enemy")
        );

        return hits.Length;
    }

    void DecideRole()
    {
        int nearby = CountNearbyEnemies(6f);

        if (nearby >= flankThreshold)
            isFlanker = Random.value > 0.5f;
        else
            isFlanker = false;
    }

    Vector3 GetGroupFlankPosition()
    {
        Vector3 playerPos = player.transform.position;

        Vector3[] offsets =
        {
            -player.transform.forward,
            player.transform.right,
            -player.transform.right
        };

        Vector3 chosen = offsets[Random.Range(0, offsets.Length)];
        Vector3 target = playerPos + chosen * flankDistance;

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            return hit.position;

        return playerPos;
    }
}
