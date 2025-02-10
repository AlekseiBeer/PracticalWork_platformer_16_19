using UnityEngine;

public class StationaryEnemy : BaseEnemy
{
    private Animator animator;
    private Rigidbody2D _rigidBody;
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    public override void UpdateBehavior() { }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable target = other.GetComponent<IDamageable>();
            target?.TakeDamage(Damage, _rigidBody.worldCenterOfMass);
        }
    }

    public override void TakeDamage(int damage, Vector2 damageSource = default)
    {
        animator.SetTrigger("IsDamage");

        base.TakeDamage(damage);
    }
}