using System.Collections;
using UnityEngine;

public class EnemyLineAttack : MonoBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] string targetTag = "Player";
    [SerializeField] float attackIntervalSeconds = 1f;
    [SerializeField] float attackDurationSeconds = 0.25f;
    [SerializeField] float maxDistance = 20f;
    [SerializeField] float lineWidth = 0.1f;
    [SerializeField] float lineAutoHideSeconds = 0.4f;
    [SerializeField] float lineFinishHideDelay = 0.1f;
    [SerializeField] float lineSafetyHideSeconds = 0.6f;
    [SerializeField] Color lineColor = new Color(1f, 0f, 1f, 1f);
    [SerializeField] LayerMask wallMask;
    [SerializeField] float hitTolerance = 0.4f;
    [SerializeField] int sortingOrder = 6;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] EnemyGridMover mover;

    Transform target;
    bool isAttacking;
    bool lineVisible;
    Coroutine lineHideRoutine;
    Coroutine finishHideRoutine;
    float nextAttackTime;
    Health health;
    float lineDisableAt;

    void Awake()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        if (mover == null)
        {
            mover = GetComponent<EnemyGridMover>();
        }

        health = GetComponent<Health>();
        ApplyLineStyle();
    }

    void OnEnable()
    {
        if (target == null)
        {
            GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
            if (targetObject != null)
            {
                target = targetObject.transform;
            }
        }

        if (health != null)
        {
            health.DamageTaken += OnDamaged;
        }

        HideLine();
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.DamageTaken -= OnDamaged;
        }

        CancelAttack();
    }

    void Update()
    {
        if (lineRenderer != null && lineRenderer.enabled && Time.time >= lineDisableAt && !isAttacking)
        {
            HideLine();
        }

        if (isAttacking || Time.time < nextAttackTime)
        {
            return;
        }

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackIntervalSeconds;

        if (mover != null)
        {
            mover.SetPaused(true);
        }

        Vector3 origin = transform.position;
        Vector3 direction = transform.up;
        Vector3 end = GetLineEnd(origin, direction, out float maxDistanceHit);
        bool hasHitTarget = false;

        ShowLine(origin, origin);

        float elapsed = 0f;
        while (elapsed < attackDurationSeconds)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / attackDurationSeconds);
            Vector3 currentEnd = Vector3.Lerp(origin, end, t);
            ShowLine(origin, currentEnd);

            if (!hasHitTarget)
            {
                float currentDistance = Vector3.Distance(origin, currentEnd);
                hasHitTarget = TryHitTarget(origin, direction, currentDistance);
            }

            yield return null;
        }

        ShowLine(origin, end);
        if (!hasHitTarget)
        {
            hasHitTarget = TryHitTarget(origin, direction, maxDistanceHit);
        }

        HideLine();
        ScheduleFinishHide();

        if (mover != null)
        {
            mover.SetPaused(false);
        }

        isAttacking = false;
    }

    Vector3 GetLineEnd(Vector3 origin, Vector3 direction, out float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, wallMask);
        if (hit.collider != null)
        {
            distance = hit.distance;
            return hit.point;
        }

        distance = maxDistance;
        return origin + direction * maxDistance;
    }

    bool TryHitTarget(Vector3 origin, Vector3 direction, float distance)
    {
        if (target == null)
        {
            return false;
        }

        Vector3 toTarget = target.position - origin;
        float forwardDistance = Vector3.Dot(toTarget, direction);
        if (forwardDistance < 0f || forwardDistance > distance)
        {
            return false;
        }

        Vector3 closest = origin + direction * forwardDistance;
        Vector2 targetPosition = new Vector2(target.position.x, target.position.y);
        Vector2 closestPosition = new Vector2(closest.x, closest.y);
        if (Vector2.Distance(targetPosition, closestPosition) > hitTolerance)
        {
            return false;
        }

        Health health = target.GetComponent<Health>();
        if (health == null)
        {
            return false;
        }

        int damage = stats != null ? stats.AttackPower : 1;
        health.TakeDamage(damage);
        return true;
    }

    void ApplyLineStyle()
    {
        if (lineRenderer == null)
        {
            return;
        }

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.positionCount = 2;
        lineRenderer.sortingOrder = sortingOrder;
    }

    void ShowLine(Vector3 start, Vector3 end)
    {
        if (lineRenderer == null)
        {
            return;
        }

        if (!lineVisible)
        {
            lineVisible = true;
            StartLineAutoHide();
        }

        lineDisableAt = Time.time + Mathf.Max(lineAutoHideSeconds, lineSafetyHideSeconds);
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    void HideLine()
    {
        if (lineRenderer == null)
        {
            return;
        }

        lineRenderer.enabled = false;
        lineVisible = false;
        if (lineHideRoutine != null)
        {
            StopCoroutine(lineHideRoutine);
            lineHideRoutine = null;
        }
    }

    void StartLineAutoHide()
    {
        if (lineAutoHideSeconds <= 0f)
        {
            return;
        }

        if (lineHideRoutine != null)
        {
            StopCoroutine(lineHideRoutine);
        }

        lineHideRoutine = StartCoroutine(LineAutoHideRoutine());
    }

    IEnumerator LineAutoHideRoutine()
    {
        yield return new WaitForSeconds(lineAutoHideSeconds);
        HideLine();
    }

    void ScheduleFinishHide()
    {
        if (lineFinishHideDelay <= 0f)
        {
            return;
        }

        if (finishHideRoutine != null)
        {
            StopCoroutine(finishHideRoutine);
        }

        finishHideRoutine = StartCoroutine(FinishHideRoutine());
    }

    IEnumerator FinishHideRoutine()
    {
        yield return new WaitForSeconds(lineFinishHideDelay);
        HideLine();
        finishHideRoutine = null;
    }

    void CancelAttack()
    {
        StopAllCoroutines();
        HideLine();
        isAttacking = false;
        if (mover != null)
        {
            mover.SetPaused(false);
        }
    }

    void OnDamaged(int amount)
    {
        if (isAttacking)
        {
            CancelAttack();
        }
    }
}
