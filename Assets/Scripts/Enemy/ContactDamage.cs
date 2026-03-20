using System.Collections.Generic;
using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField] string targetTag = "Player";
    [SerializeField] int damagePerTick = 1;
    [SerializeField] CharacterStats stats;

    readonly HashSet<Collider2D> damagedTargets = new HashSet<Collider2D>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag))
        {
            return;
        }

        if (damagedTargets.Contains(other))
        {
            return;
        }

        Health health = other.GetComponent<Health>();
        if (health == null)
        {
            return;
        }

        int damage = stats != null ? stats.AttackPower : damagePerTick;
        health.TakeDamage(damage);
        damagedTargets.Add(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (damagedTargets.Contains(other))
        {
            damagedTargets.Remove(other);
        }
    }

    void OnDisable()
    {
        damagedTargets.Clear();
    }
}
