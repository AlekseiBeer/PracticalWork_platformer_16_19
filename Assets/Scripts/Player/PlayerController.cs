using Platformer.Player;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] protected PlayerSettings playerSettings;
    [SerializeField] private PlayerHealthDisplay healthDisplay;
    public int Health { get; protected set; }
    public int Damage { get; protected set; }

    private float _knockBackForce;
    private Rigidbody2D _playerRigidBody;
    private Animator _playerAnimator;

    protected virtual void Awake()
    {
        Health = playerSettings.maxHealth;
        Damage = playerSettings.attackDamage;
        _knockBackForce = playerSettings.knockBackForce;
        _playerRigidBody = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage, Vector2 damageSource = default)
    {
        Health -= damage;
        if (damageSource != default && _playerRigidBody != null)
        {

            Vector2 knockbackDirection = (_playerRigidBody.worldCenterOfMass - damageSource).normalized;
            if (knockbackDirection != Vector2.zero)
            {
                if (TryGetComponent<PlayerMovement>(out var movement))
                {
                    movement.AddExternalForce(knockbackDirection * _knockBackForce);
                }
            }
        }
        healthDisplay.UpdateHealth(Health, playerSettings.maxHealth);
        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        _playerAnimator.SetBool("IsDead", true);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable target = other.GetComponent<IDamageable>();
            target?.TakeDamage(Damage);
        }
    }
}