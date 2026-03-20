using UnityEngine;

public class ScoreOnDeath : MonoBehaviour
{
    [SerializeField] int scoreOnDeath = 10;
    [SerializeField] Health health;

    void Awake()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }

    void OnEnable()
    {
        if (health != null)
        {
            health.Died += OnDied;
        }
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.Died -= OnDied;
        }
    }

    void OnDied(Health diedHealth)
    {
        ScoreManager.Instance?.AddScore(scoreOnDeath);
        Destroy(diedHealth.gameObject);
    }
}
