using UnityEngine;
using UnityEngine.SceneManagement;
using unityroom.Api;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] string resultSceneName = "ResultScene";
    [SerializeField] bool sendScoreToUnityroom = true;
    [SerializeField] int scoreboardNo = 1;
    [SerializeField] ScoreboardWriteMode writeMode = ScoreboardWriteMode.Always;

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

        if (sendScoreToUnityroom)
        {
            float score = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0f;
            if (UnityroomApiClient.Instance != null)
            {
                UnityroomApiClient.Instance.SendScore(scoreboardNo, score, writeMode);
            }
        }

        ScoreManager.Instance?.SetLastGameplayScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(resultSceneName);
    }
}
