using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Settings/Player")]
public class PlayerSettings : ScriptableObject
{
    public int maxHealth = 100;
    public int attackDamage = 10;
    public float attackInterval = 1.5f;
    public float knockBackForce = 5;
}
