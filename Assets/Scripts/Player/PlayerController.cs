using Platformer.Player;
using UnityEngine;
using System.Collections;
using Platformer.Player.Inputs;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] protected PlayerSettings playerSettings;
    [SerializeField] private PlayerHealthDisplay healthDisplay;

    [SerializeField] private float fallThreshold = -2f;      
    [SerializeField] private float deathAnimationTime = 2f;
    private bool isDead = false;

    public int Health { get; protected set; }
    public int Damage { get; protected set; }

    private float _knockBackForce;
    private Rigidbody2D _playerRigidBody;
    private Animator _playerAnimator;

    private PlayerCheckpointRecorder _checkpointRecorder;

    public bool IsDead => isDead;

    protected virtual void Awake()
    {
        Health = playerSettings.maxHealth;
        Damage = playerSettings.attackDamage;
        _knockBackForce = playerSettings.knockBackForce;
        _playerRigidBody = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _checkpointRecorder = GetComponent<PlayerCheckpointRecorder>();
    }

    void Update()
    {
        // Проверка на падение ниже порога
        if (!isDead && transform.position.y < fallThreshold)
        {
            Die();
        }
    }

    public void TakeDamage(int damage, Vector2 damageSource = default)
    {
        if (isDead) return;

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
        // Вычитаем очки за смерть игрока
        ScoreManager.Instance.SubtractScore(20);

        if (isDead) return;
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        isDead = true;

        // Отключаем управление
        var input = GetComponent<PlayerInput>();
        var movement = GetComponent<PlayerMovement>();
        if (input != null) input.enabled = false;
        if (movement != null) movement.enabled = false;
        _playerAnimator.SetBool("IsAttack", false);
        // Обнуляем горизонтальную скорость
        _playerRigidBody.velocity = new Vector2(0, _playerRigidBody.velocity.y);

        // Сбрасываем накопленный импульс
        if (movement != null)
            movement.ResetExternalForces();

        _playerAnimator.SetBool("IsDead", true);
        yield return new WaitForSeconds(deathAnimationTime);

        Vector3 respawnPos = _checkpointRecorder != null ? _checkpointRecorder.GetCheckpointPosition() : transform.position;
        transform.position = respawnPos;

        // Сбрасываем здоровье
        Health = playerSettings.maxHealth;
        healthDisplay.UpdateHealth(Health, playerSettings.maxHealth);

        _playerAnimator.SetBool("IsDead", false);

        if (input != null) input.enabled = true;
        if (movement != null) movement.enabled = true;

        isDead = false;
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