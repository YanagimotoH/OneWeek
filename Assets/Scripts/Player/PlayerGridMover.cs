using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerGridMover : MonoBehaviour
{
    [SerializeField] float gridSize = 1f;
    [SerializeField] float moveDuration = 0.1f;
    [SerializeField] Animator animator;
    [SerializeField] string movingBoolName = "IsMoving";

    bool isMoving;
    IGridMoveValidator[] moveValidators;

    public Vector2 LastMoveDirection { get; private set; } = Vector2.up;

    void Awake()
    {
        moveValidators = GetComponents<IGridMoveValidator>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (isMoving)
        {
            return;
        }

        Vector3 direction = GetInputDirection();
        if (direction == Vector3.zero)
        {
            return;
        }

        Vector3 target = transform.position + direction * gridSize;
        target.z = transform.position.z;
        if (!CanMoveTo(target))
        {
            return;
        }

        LastMoveDirection = new Vector2(direction.x, direction.y).normalized;
        ApplyFacing(LastMoveDirection);
        StartCoroutine(MoveTo(target));
    }

    Vector3 GetInputDirection()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return Vector3.zero;
        }

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            return Vector3.up;
        }

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            return Vector3.down;
        }

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            return Vector3.left;
        }

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            return Vector3.right;
        }

        return Vector3.zero;
#else
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            return Vector3.up;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            return Vector3.down;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            return Vector3.left;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            return Vector3.right;
        }

        return Vector3.zero;
#endif
    }

    bool CanMoveTo(Vector3 target)
    {
        if (moveValidators == null || moveValidators.Length == 0)
        {
            return true;
        }

        foreach (IGridMoveValidator validator in moveValidators)
        {
            if (validator != null && !validator.CanMoveTo(target))
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator MoveTo(Vector3 target)
    {
        isMoving = true;
        SetMoving(true);
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
        isMoving = false;
        SetMoving(false);
    }

    void SetMoving(bool moving)
    {
        if (animator == null || string.IsNullOrEmpty(movingBoolName))
        {
            return;
        }

        animator.SetBool(movingBoolName, moving);
    }

    void ApplyFacing(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void ForceSnapTo(Vector3 position)
    {
        StopAllCoroutines();
        isMoving = false;
        SetMoving(false);
        transform.position = position;
    }
}
