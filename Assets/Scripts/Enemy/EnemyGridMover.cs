using System.Collections;
using UnityEngine;

public class EnemyGridMover : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float gridSize = 1f;
    [SerializeField] Vector2 gridOffset = new Vector2(0.5f, 0.5f);
    [SerializeField] float moveIntervalSeconds = 0.5f;
    [SerializeField] float moveDuration = 0.1f;
    [SerializeField] string targetTag = "Player";

    bool isMoving;
    IGridMoveValidator[] moveValidators;

    void Awake()
    {
        moveValidators = GetComponents<IGridMoveValidator>();
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

        transform.position = SnapToGrid(transform.position);
        StartCoroutine(MoveLoop());
    }

    IEnumerator MoveLoop()
    {
        while (true)
        {
            if (!isMoving && target != null)
            {
                Vector3 next = GetNextGridStep(target.position);
                if (CanMoveTo(next))
                {
                    yield return StartCoroutine(MoveTo(next));
                }
            }

            yield return new WaitForSeconds(moveIntervalSeconds);
        }
    }

    bool CanMoveTo(Vector3 targetPosition)
    {
        if (moveValidators == null || moveValidators.Length == 0)
        {
            return true;
        }

        foreach (IGridMoveValidator validator in moveValidators)
        {
            if (validator != null && !validator.CanMoveTo(targetPosition))
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator MoveTo(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            transform.position = Vector3.Lerp(start, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    Vector3 GetNextGridStep(Vector3 targetPosition)
    {
        Vector3 current = SnapToGrid(transform.position);
        Vector3 desired = SnapToGrid(targetPosition);
        Vector3 delta = desired - current;
        delta.z = 0f;

        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
        {
            current.x += Mathf.Sign(delta.x) * gridSize;
        }
        else
        {
            current.y += Mathf.Sign(delta.y) * gridSize;
        }

        current = SnapToGrid(current);
        return current;
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round((position.x - gridOffset.x) / gridSize) * gridSize + gridOffset.x;
        float y = Mathf.Round((position.y - gridOffset.y) / gridSize) * gridSize + gridOffset.y;
        return new Vector3(x, y, position.z);
    }
}
