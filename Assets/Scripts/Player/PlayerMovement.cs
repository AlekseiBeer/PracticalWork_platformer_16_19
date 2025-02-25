using UnityEngine;
using Platformer.Player.Inputs;
using System.Collections;

namespace Platformer.Player
{
    public enum State
    { 
        Idle,
        Run,
        Jump,
        Air
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _movementSpeed = 5f;
        [SerializeField] private float _jumpForce = 5f;

        [Header("Ground Check Settings")]
        [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.35f, 0.05f);
        [SerializeField] private LayerMask _groundLayerMask;
        [SerializeField] private float _groundCheckCooldown = 0.05f;

        [Header("Wall Check Settings")]
        [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.35f, 0.49f);
        [SerializeField] private Vector3 _posOffsetRight = Vector2.zero;
        [SerializeField] private Vector3 _posOffsetLeft = Vector2.zero;

        [Header("Melee Attack Settings")]
        [SerializeField] private float attackDamage = 10f;     
        [SerializeField] private float attackRange = 0.5f;         
        [SerializeField] private Vector2 attackOffset = new Vector2(0.5f, 0);
        [SerializeField] private LayerMask enemyLayerMask;


        private State _playerState;

        private PlayerInput _playerInput;
        private Rigidbody2D _playerRigidbody;
        private Animator _playerAnimator;
        private SpriteRenderer _playerSprite;

        private bool _isGrounded;
        private bool _isJumping;
        private bool _isRunning;
        private bool _isAttacking;
        private bool _canCheckGround = true;
        private bool _isOnCheckpointGround = false;

        private Vector2 _externalForceAccumulator = Vector2.zero;
        private Vector2 _currentPlatformVelocity = Vector2.zero;

        public bool IsGrounded => _isGrounded;
        public bool IsOnCheckpointGround => _isOnCheckpointGround;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerRigidbody = GetComponent<Rigidbody2D>();
            _playerSprite = GetComponent<SpriteRenderer>();
            _playerAnimator = GetComponent<Animator>();
        }

        void Update()
        {
            CheckGrounded();
            HandleMovement();
            HandleJump();
            HandleAttack();

            UpdateState();
            UpdateAnimation();
        }

        private void FixedUpdate()
        {
            float horizontal = _playerInput.Horizontal;

            if ((!_isGrounded && IsTouchingWallRight() && horizontal > 0) || (!_isGrounded && IsTouchingWallLeft() && horizontal < 0))
                horizontal = 0;

            float inputVelocity = horizontal * _movementSpeed;

            float extraDecay = 0f;
            if (horizontal != 0 && _externalForceAccumulator.x * horizontal < 0)
            {
                extraDecay = 4f;
            }
            float combinedDecayRate = 3f + extraDecay;

            float finalVelocityX = inputVelocity + _externalForceAccumulator.x + _currentPlatformVelocity.x;
            float finalVelocityY = _playerRigidbody.velocity.y;
                
            _playerRigidbody.velocity = new Vector2(finalVelocityX, finalVelocityY);

            // Плавное затухание внешних сил
            _externalForceAccumulator = Vector2.Lerp(_externalForceAccumulator, Vector2.zero, combinedDecayRate * Time.fixedDeltaTime);
        }

        private void HandleMovement()
        {
            float horizontal = _playerInput.Horizontal;
            if (Mathf.Abs(horizontal) > 0.01f)
            {
                Run(horizontal);
            }
            else
            {
                _isRunning = false;
            }
        }

        private void HandleJump()
        {
            if (_playerInput.Jump && _isGrounded)
            {
                Jump();
            }
        }

        private void CheckGrounded()
        {
            if (!_canCheckGround) return;

            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, _groundCheckSize, 0f, _groundLayerMask);
            if (colliders.Length > 0)
            {
                _isJumping = false;
                _isGrounded = true;
                foreach (var col in colliders)
                {
                    if (col.CompareTag("CheckpointGround"))
                        _isOnCheckpointGround = true;
                }
            }
            else
            {
                _isGrounded = false;
                _isOnCheckpointGround = false;
            }
        }

        private bool IsTouchingWallRight()
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + _posOffsetRight, _wallCheckSize, 0f, _groundLayerMask);
            if (colliders.Length > 0)
                return true;
            else
                return false;
        }
        private bool IsTouchingWallLeft()
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + _posOffsetLeft, _wallCheckSize, 0f, _groundLayerMask);
            if (colliders.Length > 0)
                return true;
            else
                return false;
        }

        private IEnumerator GroundCheckCooldown()
        {
            yield return new WaitForSeconds(_groundCheckCooldown);
            _canCheckGround = true;
        }

        private void UpdateState()
        {
            if (_isJumping)
            {
                _playerState = State.Jump;
            }
            else if (!_isGrounded)
            {
                _playerState = State.Air;
            }
            else if (_isRunning)
            {
                _playerState = State.Run;
            }
            else
            {
                _playerState = State.Idle;
            }
        }

        private void UpdateAnimation()
        {
            _playerAnimator.SetBool("IsRun", _playerState == State.Run);
            _playerAnimator.SetBool("IsJump", _playerState == State.Jump);
            _playerAnimator.SetBool("IsAir", _playerState == State.Air);
            _playerAnimator.SetBool("IsAttack", _isAttacking);
        }

        private void Jump()
        {
            _isJumping = true;
            _canCheckGround = false;
            if (!IsTouchingWallRight() && !IsTouchingWallLeft())
                AddExternalForce(_playerRigidbody.velocity);
            StartCoroutine(GroundCheckCooldown());
            _playerRigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }

        private void HandleAttack()
        {
            _isAttacking = _playerInput.Attack;
        }

        private void Run(float horizontal)
        {
            _isRunning = true;
            _playerSprite.flipX = horizontal < 0;
        }

        public void AddExternalForce(Vector2 force)
        {
            _externalForceAccumulator += force;
        }

        public void ResetExternalForces()
        {
            _externalForceAccumulator = Vector2.zero;
        }

        // Обработка столкновений с движущимися платформами
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                Rigidbody2D platformRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (platformRb != null)
                {
                    _currentPlatformVelocity = platformRb.velocity;
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                _currentPlatformVelocity = Vector2.zero;
            }
        }

        public void PerformMeleeAttack()
        {
            // Рассчитываем позицию атаки с учётом направления игрока
            Vector2 attackPos = (Vector2)transform.position;
            attackPos.y += attackOffset.y;
            if (_playerSprite.flipX)
            {
                // Если игрок смотрит влево, смещаем влево
                attackPos.x -= attackOffset.x;
            }
            else
            {
                // Если игрок смотрит вправо, смещаем вправо
                attackPos.x += attackOffset.x;
            }

            Debug.Log($"rgrewgwerg");

            // Выполняем проверку области атаки с использованием OverlapCircleAll
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, attackRange, enemyLayerMask);
            foreach (Collider2D enemy in hitEnemies)
            {
                // Если у врага есть компонент, реализующий IDamageable, наносим урон
                IDamageable damageable = enemy.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage((int)attackDamage);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_playerSprite != null)
            {
                Gizmos.color = Color.red;
                Vector2 attackPos = (Vector2)transform.position;
                attackPos.y += attackOffset.y;
                if (_playerSprite.flipX)
                {
                    attackPos.x -= attackOffset.x;
                }
                else
                {
                    attackPos.x += attackOffset.x;
                }
                Gizmos.DrawWireSphere(attackPos, attackRange);
            }
        }
    }
}