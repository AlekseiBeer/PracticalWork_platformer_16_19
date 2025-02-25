using UnityEngine;


public class ShooterEnemy : BaseEnemy
{

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    private float shootTimer;
    private float impulseForce;


    protected override void Awake()
    {
        base.Awake();
        shootTimer = enemySettings.attackInterval;
        impulseForce = enemySettings.impulseForceBullet;
    }

    public override void UpdateBehavior()
    {
        HandleShooting();
    }

    private void HandleShooting()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = enemySettings.attackInterval;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null && shootPoint != null)
        {
            GameObject bulletInstance = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

            Rigidbody2D bulletRb = bulletInstance.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(-shootPoint.right * impulseForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable target = other.GetComponent<IDamageable>();
            target?.TakeDamage(Damage);
        }
    }
}