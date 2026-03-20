using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] float smoothTime = 0.1f;
    [SerializeField] bool followInFixedUpdate;
    [SerializeField] bool useCustomRotation;
    [SerializeField] Vector3 customEulerAngles;

    Vector3 velocity;
    Quaternion fixedRotation;

    void Awake()
    {
        fixedRotation = useCustomRotation ? Quaternion.Euler(customEulerAngles) : transform.rotation;
    }

    void LateUpdate()
    {
        if (followInFixedUpdate)
        {
            return;
        }

        Follow(Time.deltaTime);
        transform.rotation = fixedRotation;
    }

    void FixedUpdate()
    {
        if (!followInFixedUpdate)
        {
            return;
        }

        Follow(Time.fixedDeltaTime);
    }

    void Follow(float deltaTime)
    {
        if (target == null)
        {
            return;
        }

        Vector3 desired = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime, Mathf.Infinity, deltaTime);
    }
}
