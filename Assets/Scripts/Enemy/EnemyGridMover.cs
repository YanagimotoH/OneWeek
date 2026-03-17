using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGridMover : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float gridSize = 1f;
    [SerializeField] float moveIntervalSeconds = 0.5f;
    [SerializeField] float navMeshSearchDistance = 2f;

    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        EnsureOnNavMesh();
        StartCoroutine(MoveLoop());
    }

    void EnsureOnNavMesh()
    {
        if (agent == null || !agent.enabled)
        {
            return;
        }

        if (agent.isOnNavMesh)
        {
            return;
        }

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshSearchDistance, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }

    IEnumerator MoveLoop()
    {
        while (true)
        {
            if (target != null && agent != null && agent.enabled && agent.isOnNavMesh)
            {
                Vector3 next = GetNextGridStep(target.position);
                agent.SetDestination(next);
            }

            yield return new WaitForSeconds(moveIntervalSeconds);
        }
    }

    Vector3 GetNextGridStep(Vector3 targetPosition)
    {
        Vector3 current = transform.position;
        Vector3 delta = targetPosition - current;

        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.z))
        {
            current.x += Mathf.Sign(delta.x) * gridSize;
        }
        else
        {
            current.z += Mathf.Sign(delta.z) * gridSize;
        }

        current.x = Mathf.Round(current.x / gridSize) * gridSize;
        current.z = Mathf.Round(current.z / gridSize) * gridSize;
        return current;
    }
}
