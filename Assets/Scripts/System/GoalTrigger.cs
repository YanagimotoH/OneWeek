using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] string targetTag = "Player";
    [SerializeField] int clearScore = 50;

    ProceduralMapGenerator generator;
    bool triggered;

    public void Initialize(ProceduralMapGenerator generatorInstance, string playerTag)
    {
        generator = generatorInstance;
        targetTag = playerTag;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag(targetTag))
        {
            return;
        }

        triggered = true;
        ScoreManager.Instance?.AddScore(clearScore);
        if (generator != null)
        {
            generator.RegenerateFromGoal();
        }
    }
}
