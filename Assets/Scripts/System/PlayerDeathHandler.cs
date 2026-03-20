using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] string resultSceneName = "ResultScene";

    bool triggered;

    void Awake()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }

        if (health == null)
        {
            health = GetComponentInChildren<Health>();
        }
    }

    void OnEnable()
    {
        if (health != null)
        {
            health.Died += OnPlayerDied;
        }
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.Died -= OnPlayerDied;
        }
    }

    void Update()
    {
        if (triggered)
        {
            return;
        }

        if (health != null && health.CurrentHp <= 0)
        {
            OnPlayerDied(health);
        }
    }

    void OnPlayerDied(Health diedHealth)
    {
        if (triggered)
        {
            return;
        }

        triggered = true;
        ScoreManager.Instance?.SetLastGameplayScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(resultSceneName);
    }
}
