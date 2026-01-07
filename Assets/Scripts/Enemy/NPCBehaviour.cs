using UnityEngine;
using UnityEngine.AI;

public class NPC_Behaviour : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private GameObject player;

    [Header("Detección de luz")]
    [SerializeField] private Transform flashlight;
    [SerializeField] private float flashlightAngle = 100f;
    [SerializeField] private float flashlightDistance = 15f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Tipo de NPC")]
    [SerializeField] private NPCCategory npcCategory = NPCCategory.AlwaysActive;
    [SerializeField] private NPCType npcType = NPCType.LightFear;

    [Header("Proximidad")]
    [SerializeField] private float activationDistance = 5f;

    private NPCState state;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        switch (npcCategory)
        {
            case NPCCategory.AlwaysActive:
                state = NPCState.Active;
                break;
            case NPCCategory.LightTriggered:
            case NPCCategory.ProximityTriggered:
                state = NPCState.Inactive;
                break;
        }
    }

    void Update()
    {
        switch (state)
        {
            case NPCState.Inactive:
                HandleInactiveState();
                break;

            case NPCState.Active:
                HandleActiveState();
                break;
        }
    }

    void HandleInactiveState()
    {
        agent.isStopped = true;

        // Se activa si es LightTriggered y le da la luz
        if (npcCategory == NPCCategory.LightTriggered && IsIlluminated())
        {
            state = NPCState.Active;
            agent.isStopped = false;
            agent.SetDestination(transform.position); // empieza quieto
        }

        // Se activa si es ProximityTriggered y el jugador está cerca
        if (npcCategory == NPCCategory.ProximityTriggered)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= activationDistance)
            {
                state = NPCState.Active;
                agent.isStopped = false;
                agent.SetDestination(transform.position); // empieza quieto
            }
        }
    }

    void HandleActiveState()
    {
        if (IsIlluminated() && npcType == NPCType.LightFear){
            agent.isStopped = true;
            return;
        }
        if (!IsIlluminated() && npcType == NPCType.DarkFear){
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.transform.position);
    }

    bool IsIlluminated()
    {
        Vector3 dirToNPC = transform.position - flashlight.position;
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
            flashlightDistance,
            obstacleMask))
        {
            if (hit.transform != transform)
                return false;
        }

        return true;
    }
}

public enum NPCType
{
    LightFear,
    DarkFear,
}

public enum NPCCategory
{
    AlwaysActive,
    LightTriggered,
    ProximityTriggered
}

public enum NPCState
{
    Inactive,
    Active
}
