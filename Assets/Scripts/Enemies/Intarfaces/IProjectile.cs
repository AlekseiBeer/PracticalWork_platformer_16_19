using UnityEngine;

public interface IProjectile
{
    float Damage { get; }
    void OnHit(Collider2D collider);
}