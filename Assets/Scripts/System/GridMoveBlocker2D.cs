using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GridMoveBlocker2D : MonoBehaviour, IGridMoveValidator
{
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] float collisionInset = 0.02f;

    Collider2D cachedCollider;
    readonly Collider2D[] overlapResults = new Collider2D[4];

    void Awake()
    {
        cachedCollider = GetComponent<Collider2D>();
    }

    public bool CanMoveTo(Vector3 target)
    {
        if (cachedCollider == null)
        {
            return true;
        }

        Vector3 delta = target - transform.position;
        delta.z = 0f;
        if (delta.sqrMagnitude <= Mathf.Epsilon)
        {
            return true;
        }

        Vector2 size = cachedCollider.bounds.size;
        size -= Vector2.one * collisionInset;
        size = new Vector2(Mathf.Max(size.x, 0.01f), Mathf.Max(size.y, 0.01f));

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(obstacleMask);
        filter.useTriggers = false;

        int hitCount = Physics2D.OverlapBox(target, size, 0f, filter, overlapResults);
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = overlapResults[i];
            if (hit != null && hit != cachedCollider)
            {
                return false;
            }
        }

        return true;
    }
}
