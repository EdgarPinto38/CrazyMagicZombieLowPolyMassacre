using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public string playerTag = "Player"; // Tag del jugador
    public float followRange = 10f;
    public Transform[] patrolPoints; // Puntos de patrulla
    public float patrolWaitTime = 2f; // Tiempo de espera en cada punto de patrulla

    private NavMeshAgent agent;
    private Transform player;
    private int currentPatrolIndex;
    private bool waiting;
    private float waitTimer;

    // Tiempo de vida del enemigo en segundos
    public float lifeTime = 20f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Buscar el objeto con el tag especificado
        GameObject playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con el tag " + playerTag);
        }

        // Iniciar la destrucción del enemigo después del tiempo de vida
        Destroy(gameObject, lifeTime);

        // Iniciar la patrulla
        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = 0;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            if (distanceToPlayer <= followRange)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        if (!waiting && agent.remainingDistance < 0.5f)
        {
            waiting = true;
            waitTimer = 0f;
        }

        if (waiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                waiting = false;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            }
        }
    }
}

