using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Gamemanager gamemanager;
    [SerializeField] PlayerGridMover gridMover;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] CharacterStats stats;
    [SerializeField] int baseDamage = 1;
    [SerializeField] float lineDistance = 2f;
    [SerializeField] float lineDurationSeconds = 0.2f;
    [SerializeField] float lineWidth = 0.1f;
    [SerializeField] float lineHitTolerance = 0.4f;
    [SerializeField] float lineAutoHideSeconds = 0.4f;
    [SerializeField] float lineFinishHideDelay = 0.1f;
    [SerializeField] float lineSafetyHideSeconds = 0.6f;
    [SerializeField] Color lineColor = new Color(1f, 0f, 1f, 1f);
    [SerializeField] int lineSortingOrder = 6;
    [SerializeField] LineRenderer lineRenderer;

    public event Action DamageDealt;

    bool isAttacking;
    bool damageNotified;
    bool lineVisible;
    Coroutine lineHideRoutine;
    Coroutine finishHideRoutine;
    Health health;
    float lineDisableAt;

    void Awake()
    {
        if (gridMover == null)
        {
            gridMover = GetComponent<PlayerGridMover>();
        }

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        health = GetComponent<Health>();
        ApplyLineStyle();
    }

    void OnEnable()
    {
        if (health != null)
        {
            health.DamageTaken += OnDamaged;
        }
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

        if (gamemanager == null || !gamemanager.IsBackSide)
        {
            if (isAttacking)
            {
                CancelAttack();
            }

            return;
        }

        if (isAttacking)
        {
            return;
        }

#if ENABLE_INPUT_SYSTEM
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(PerformAttack());
        }
#else
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(PerformAttack());
        }
#endif
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        damageNotified = false;

        Vector2 attackDir = gridMover != null ? gridMover.LastMoveDirection : Vector2.up;
        if (attackDir == Vector2.zero)
        {
            attackDir = Vector2.up;
        }

        Vector3 forward = new Vector3(attackDir.x, attackDir.y, 0f).normalized;

        int damage = stats != null ? stats.AttackPower : baseDamage;
        HashSet<Health> damagedTargets = new HashSet<Health>();

        ShowLine(transform.position, transform.position);

        float elapsed = 0f;
        while (elapsed < lineDurationSeconds)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lineDurationSeconds);
            Vector3 lineStart = transform.position;
            Vector3 lineEnd = lineStart + forward * lineDistance * t;
            ShowLine(lineStart, lineEnd);
            ApplyLineDamage(lineStart, lineEnd, damage, damagedTargets);
            yield return null;
        }

        Vector3 finalStart = transform.position;
        Vector3 finalEnd = finalStart + forward * lineDistance;
        ShowLine(finalStart, finalEnd);
        ApplyLineDamage(finalStart, finalEnd, damage, damagedTargets);
        HideLine();
        ScheduleFinishHide();

        isAttacking = false;
    }

    void ApplyLineDamage(Vector3 start, Vector3 end, int damage, HashSet<Health> damagedTargets)
    {
        float radius = lineDistance + lineHitTolerance;
        Collider2D[] hits = Physics2D.OverlapCircleAll(start, radius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health == null || damagedTargets.Contains(health))
            {
                continue;
            }

            if (!IsOnLine(hit.transform.position, start, end))
            {
                continue;
            }

            health.TakeDamage(damage);
            damagedTargets.Add(health);

            if (!damageNotified)
            {
                damageNotified = true;
                DamageDealt?.Invoke();
            }
        }
    }

    bool IsOnLine(Vector3 position, Vector3 start, Vector3 end)
    {
        Vector2 start2 = new Vector2(start.x, start.y);
        Vector2 end2 = new Vector2(end.x, end.y);
        Vector2 position2 = new Vector2(position.x, position.y);
        Vector2 segment = end2 - start2;
        float length = segment.magnitude;
        if (length <= Mathf.Epsilon)
        {
            return false;
        }

        Vector2 direction = segment / length;
        float projection = Vector2.Dot(position2 - start2, direction);
        if (projection < 0f || projection > length)
        {
            return false;
        }

        Vector2 closest = start2 + direction * projection;
        return Vector2.Distance(position2, closest) <= lineHitTolerance;
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
        lineRenderer.sortingOrder = lineSortingOrder;
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
    }

    void OnDamaged(int amount)
    {
        if (isAttacking)
        {
            CancelAttack();
        }
    }
}
