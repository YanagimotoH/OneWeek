using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerDamageAudio : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] PlayerAttack playerAttack;
    [SerializeField] AudioClip takeDamageClip;
    [SerializeField] AudioClip dealDamageClip;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (health == null)
        {
            health = GetComponentInChildren<Health>();
        }

        if (playerAttack == null)
        {
            playerAttack = GetComponentInChildren<PlayerAttack>();
        }
    }

    void OnEnable()
    {
        if (health != null)
        {
            health.DamageTaken += OnDamageTaken;
        }

        if (playerAttack != null)
        {
            playerAttack.DamageDealt += OnDamageDealt;
        }
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.DamageTaken -= OnDamageTaken;
        }

        if (playerAttack != null)
        {
            playerAttack.DamageDealt -= OnDamageDealt;
        }
    }

    void OnDamageTaken(int amount)
    {
        if (takeDamageClip == null || audioSource == null)
        {
            return;
        }

        audioSource.PlayOneShot(takeDamageClip);
    }

    void OnDamageDealt()
    {
        if (dealDamageClip == null || audioSource == null)
        {
            return;
        }

        audioSource.PlayOneShot(dealDamageClip);
    }
}
