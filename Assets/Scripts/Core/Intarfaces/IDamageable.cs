using UnityEngine;

public interface IDamageable
{
    int Health { get; }
    void TakeDamage(int damage, Vector2 damageSource = default);
    void Die();
}
