using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField] string targetTag = "Player";
    [SerializeField] int damagePerTick = 1;
    [SerializeField] float tickIntervalSeconds = 1f;

    float nextDamageTime;

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(targetTag))
        {
            return;
        }

        if (Time.time < nextDamageTime)
        {
            return;
        }

        Health health = other.GetComponent<Health>();
        if (health == null)
        {
            return;
        }

        health.TakeDamage(damagePerTick);
        nextDamageTime = Time.time + tickIntervalSeconds;
    }
}
