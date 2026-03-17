using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Gamemanager gamemanager;
    [SerializeField] float attackRadius = 2f;
    [SerializeField] float attackAngle = 90f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] LayerMask enemyLayer;

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
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);
        Vector3 forward = transform.forward;

        foreach (Collider hit in hits)
        {
            Vector3 direction = hit.transform.position - transform.position;
            direction.y = 0f;
            if (direction == Vector3.zero)
            {
                continue;
            }

            float angle = Vector3.Angle(forward, direction);
            if (angle > attackAngle * 0.5f)
            {
                continue;
            }

            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }
        }
    }
}
