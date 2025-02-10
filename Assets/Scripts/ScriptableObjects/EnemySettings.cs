using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Settings/Enemy")]
public class EnemySettings : ScriptableObject
{
    public int maxHealth = 100;
    public float moveSpeed = 3f;
    public int attackDamage = 10;
    public float attackInterval = 1.5f;
    public float impulseForceBullet = 10f;
}
