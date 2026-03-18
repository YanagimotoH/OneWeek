using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerGridMover : MonoBehaviour
{
    [SerializeField] float gridSize = 1f;
    [SerializeField] float moveDuration = 0.1f;

    bool isMoving;
    IGridMoveValidator[] moveValidators;

    public Vector2 LastMoveDirection { get; private set; } = Vector2.up;

    void Awake()
    {
        moveValidators = GetComponents<IGridMoveValidator>();
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

        if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
        {
            return Vector3.up;
        }

        if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
        {
            return Vector3.down;
        }

        if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
        {
            return Vector3.left;
        }

        if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
        {
            return Vector3.right;
        }

        return Vector3.zero;
#else
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            return Vector3.up;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            return Vector3.down;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            return Vector3.left;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
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
    }
}
