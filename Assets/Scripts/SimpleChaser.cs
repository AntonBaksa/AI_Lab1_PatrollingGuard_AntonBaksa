using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrolling,
    Chasing,
    ReturningToPatrol
}

[RequireComponent(typeof(NavMeshAgent))]
public class SimpleChaser : MonoBehaviour
{

    [Header("References")]
    public Transform target;

    [Header("Chase settings")]
    public float chaseRange = 10f;

    public Transform[] waypoints;
    public float waypointTolerence = 0.5f;

    int currentIndex = 0;
    private NavMeshAgent agent;
    private EnemyState currentState = EnemyState.Patrolling;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (waypoints.Length > 0)
        { 
            agent.SetDestination(waypoints[currentIndex].position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            return;
        }
        if (waypoints.Length == 0)
        {
            return;
        }


        if (!agent.pathPending && agent.remainingDistance <= waypointTolerence)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentIndex].position);
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        switch (currentState)
        { 
            case EnemyState.Patrolling:
                UpdatePatrolling(distanceToTarget); 
                break;
            case EnemyState.Chasing:
                UpdateChasing(distanceToTarget); 
                break;
            case EnemyState.ReturningToPatrol:
                UpdateReturning(distanceToTarget);
                break;
        }
    }

    void UpdatePatrolling(float distanceToTarget)
    {
        if (!agent.pathPending && agent.remainingDistance <= waypointTolerence)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentIndex].position);
        }

        if (distanceToTarget <= chaseRange) 
        {
            currentState = EnemyState.Chasing;
        }
    }

    void UpdateChasing(float distanceToTarget)
    {
        agent.SetDestination(target.position);
        if (distanceToTarget > chaseRange)
        {
            currentState = EnemyState.ReturningToPatrol;
            agent.ResetPath();
        }
    }

    void UpdateReturning(float distanceToTarget)
    {

        agent.SetDestination(waypoints[currentIndex].position);
        if (agent.remainingDistance <= waypointTolerence)
        {
            currentState = EnemyState.Patrolling;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
