using UnityEngine;

public class Bullet : MonoBehaviour, IProjectile
{
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float damage = 10f;
    public float Damage => damage;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }


    public void OnHit(Collider2D collider)
    {
        IDamageable target = collider.GetComponent<IDamageable>();
        target.TakeDamage((int)Damage);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnHit(other);
        }
    }
}