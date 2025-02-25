using UnityEngine;
public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] protected EnemySettings enemySettings;

    public int Health { get; protected set; }
    public float Speed { get; protected set; }
    public int Damage { get; protected set; }

    protected virtual void Awake()
    {
        Health = enemySettings.maxHealth;
        Speed = enemySettings.moveSpeed;
        Damage = enemySettings.attackDamage;

        HealthBarManager.Instance.RegisterEnemy(transform, Health, enemySettings.maxHealth);
    }

    protected virtual void Update()
    {
        UpdateBehavior();
    }

    public virtual void TakeDamage(int damage, Vector2 damageSource = default)
    {
        Health -= damage;
        HealthBarManager.Instance.UpdateEnemyHealth(transform, Health, enemySettings.maxHealth);
        if (Health <= 0)
            Die();
    }

    public virtual void Die()
    {
        ScoreManager.Instance.AddScore(enemySettings.scoreValue);

        HealthBarManager.Instance.UnregisterEnemy(transform);
        Destroy(gameObject);
    }

    //поведение врага
    public abstract void UpdateBehavior();
}
