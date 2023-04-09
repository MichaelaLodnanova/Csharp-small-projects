using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] protected LayerMask _enemyLayerMask;
    [SerializeField] protected ParticleSystem _onHitParticleSystem;

    public int Damage;
    public int Speed;
    public double LifeTime;
    public Collider target;

    public void SetTarget(Collider target)
    {
        this.target = target;
    }

    public abstract void FireDamage();

    public void OnTriggerEnter(Collider collider)
    {
        if (!collider.gameObject.CompareTag("enemy"))
        {
            return;
        }

        FireDamage();
        Destroy(gameObject);
    }
}
