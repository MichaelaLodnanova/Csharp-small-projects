using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementComponent), typeof(HealthComponent), typeof(BoxCollider))]
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected MovementComponent _movementComponent;
    [SerializeField] protected HealthComponent _healthComponent;
    [SerializeField] protected ParticleSystem _onDeathParticlePrefab;
    [SerializeField] protected ParticleSystem _onSuccessParticlePrefab;
    [SerializeField] protected LayerMask _attackLayerMask;

    public event Action OnDeath;
    public int Damage;
    public float Speed;
    public int Reward;

    private void Start()
    {
        _healthComponent.OnDeath += HandleDeath;
        _movementComponent.MoveAlongPath();
    }

    private void OnDestroy()
    {
        _healthComponent.OnDeath -= HandleDeath;
    }

    public void Init(EnemyPath path)
    {
        // TODO: Modify this so they have appropriate speed
        _movementComponent.Init(path, Speed);
    }

    protected void HandleDeath()
    {
        // TODO: Modify this so they give appropriate reward
        GameObject.FindObjectOfType<Player>().Resources += Reward;
        OnDeath?.Invoke();
        Destroy(gameObject);
    }


    public abstract void OnCollisionEnter(Collision collision);
    public void SpawnParticles()
    {
        ParticleSystem particle = Instantiate(_onDeathParticlePrefab, transform.position, transform.rotation);
        particle.Play();
        Destroy(particle, 3);
    }
}
