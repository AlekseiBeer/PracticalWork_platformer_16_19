using System.Collections;
using UnityEngine;

public class PatrolEnemy : BaseEnemy
{
    private IMovement movementComponent;

    private Animator animator;
    private Rigidbody2D _rigidBody;

    private bool canMove = true;
    protected override void Awake()
    {
        base.Awake();
        movementComponent = GetComponent<PatrolMovement>();
        animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
    }
    protected override void Update()
    {
        if (canMove)
            UpdateBehavior();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable target = other.GetComponent<IDamageable>();
            target?.TakeDamage(Damage, _rigidBody.worldCenterOfMass);
        }
    }

    public override void UpdateBehavior()
    {
        animator.SetBool("IsMove", true);
        movementComponent?.Move();
    }

    private IEnumerator canMoveCooldown()
    {
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable target = other.GetComponent<IDamageable>();
            target?.TakeDamage(Damage);
        }
    }

    public override void TakeDamage(int damage, Vector2 damageSource = default)
    {
        animator.SetTrigger("IsDamage");
        canMove = false;
        StartCoroutine(canMoveCooldown());
        base.TakeDamage(damage);
    }
}