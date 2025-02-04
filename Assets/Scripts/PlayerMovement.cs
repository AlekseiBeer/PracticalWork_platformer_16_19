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
    public class PlayrMovment : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _movementSpeed = 5f;
        [SerializeField] private float _jumpForce = 5f;

        [Header("Ground Check Settings")]
        [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.35f, 0.05f);
        [SerializeField] private LayerMask _groundLayerMask;
        [SerializeField] private float _groundCheckCooldown = 0.05f;

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

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerRigidbody = GetComponent<Rigidbody2D>();
            _playerSprite = GetComponent<SpriteRenderer>();
            _playerAnimator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            CheckGrounded();
            HandleMovement();
            HandleJump();
            HandleAttack();
            
            UpdateState();
            UpdateAnimation();

            //Debug.Log(_playerState);
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
            }
            else
            {
                _isGrounded = false;
            }
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
            Vector2 velocity = _playerRigidbody.velocity;
            velocity.x = horizontal * _movementSpeed;
            _playerRigidbody.velocity = velocity;

            _playerSprite.flipX = _playerInput.Horizontal < 0;
        }
    }
}