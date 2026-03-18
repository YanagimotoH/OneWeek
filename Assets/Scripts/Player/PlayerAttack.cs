using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Gamemanager gamemanager;
    [SerializeField] PlayerGridMover gridMover;
    [SerializeField] Animator animator;
    [SerializeField] Transform attackEffect;
    [SerializeField] string attackTriggerName = "Attack";
    [SerializeField] float attackRadius = 2f;
    [SerializeField] float attackAngle = 90f;
    [SerializeField] float attackOffset = 0.5f;
    [SerializeField] LayerMask enemyLayer;

    void Awake()
    {
        if (gridMover == null)
        {
            gridMover = GetComponent<PlayerGridMover>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (attackEffect == null && animator != null && animator.transform != transform)
        {
            attackEffect = animator.transform;
        }
    }

    void Update()
    {
        if (gamemanager == null || !gamemanager.IsBackSide)
        {
            return;
        }

#if ENABLE_INPUT_SYSTEM
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            PerformAttack();
        }
#else
        if (Input.GetButtonDown("Fire1"))
        {
            PerformAttack();
        }
#endif
    }

    void PerformAttack()
    {
        Vector2 attackDir = gridMover != null ? gridMover.LastMoveDirection : Vector2.up;
        if (attackDir == Vector2.zero)
        {
            attackDir = Vector2.up;
        }

        Vector3 forward = new Vector3(attackDir.x, attackDir.y, 0f).normalized;
        Vector3 attackPosition = transform.position + forward * attackOffset;
        attackPosition.z = transform.position.z;

        if (attackEffect != null)
        {
            Vector3 effectPosition = attackPosition;
            effectPosition.z = attackEffect.position.z;
            attackEffect.position = effectPosition;

            float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg - 90f;
            attackEffect.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (animator != null && !string.IsNullOrEmpty(attackTriggerName))
        {
            animator.SetTrigger(attackTriggerName);
        }

        Collider[] hits = Physics.OverlapSphere(attackPosition, attackRadius, enemyLayer);

        foreach (Collider hit in hits)
        {
            Vector3 direction = hit.transform.position - attackPosition;
            direction.z = 0f;
            if (direction == Vector3.zero)
            {
                continue;
            }

            float angle = Vector3.Angle(forward, direction);
            if (angle > attackAngle * 0.5f)
            {
                continue;
            }
        }
    }
}
