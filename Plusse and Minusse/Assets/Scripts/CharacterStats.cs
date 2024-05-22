using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/CharacterStats", order = 2)]
public class CharacterStats : ScriptableObject
{
    public int maxHealth;
    public int currHealth;
    public int attack;
    public int defence;
}