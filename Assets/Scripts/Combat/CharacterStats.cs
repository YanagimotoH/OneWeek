using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Character Stats")]
public class CharacterStats : ScriptableObject
{
    [SerializeField] int maxHp = 10;
    [SerializeField] int attackPower = 1;

    public int MaxHp => maxHp;
    public int AttackPower => attackPower;
}
