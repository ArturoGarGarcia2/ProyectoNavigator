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
    [SerializeField] private NPCType npcType = NPCType.AlwaysActive;

    [Header("Proximidad")]
    [SerializeField] private float activationDistance = 5f; // para NPCs que se activan por proximidad

    private NPCState state;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Estado inicial según tipo
        switch (npcType)
        {
            case NPCType.AlwaysActive:
                state = NPCState.Active;
                break;
            case NPCType.LightTriggered:
            case NPCType.ProximityTriggered:
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
        if (npcType == NPCType.LightTriggered && IsIlluminated())
        {
            state = NPCState.Active;
            agent.isStopped = false;
            agent.SetDestination(transform.position); // empieza quieto
        }

        // Se activa si es ProximityTriggered y el jugador está cerca
        if (npcType == NPCType.ProximityTriggered)
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
        // TODOS los NPCs se detienen si les da la luz
        if (IsIlluminated())
        {
            agent.isStopped = true;
            return;
        }

        // Si no hay luz, se mueve hacia el jugador
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
    AlwaysActive,       // Siempre activo
    LightTriggered,     // Se activa con la luz
    ProximityTriggered  // Se activa por cercanía al jugador
}

public enum NPCState
{
    Inactive,
    Active
}
