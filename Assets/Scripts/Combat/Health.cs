using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHp = 100;

    public int CurrentHp { get; private set; }

    void Awake()
    {
        CurrentHp = maxHp;
    }

    public void TakeDamage(int amount)
    {
        if (CurrentHp <= 0)
        {
            return;
        }

        CurrentHp = Mathf.Max(CurrentHp - amount, 0);
    }
}
