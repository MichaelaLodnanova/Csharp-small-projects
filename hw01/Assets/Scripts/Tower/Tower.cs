using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(HealthComponent))]
public abstract class Tower : MonoBehaviour
{
    [SerializeField] protected LayerMask _enemyLayerMask;
    [SerializeField] private HealthComponent _healthComponent;
    [SerializeField] protected Projectile _projectilePrefab;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] protected Transform _objectToPan;
    [SerializeField] protected Transform _projectileSpawn;
    [SerializeField] private GameObject _previewPrefab;

    public HealthComponent Health => _healthComponent;
    public BoxCollider Collider => _boxCollider;
    public GameObject BuildingPreview => _previewPrefab;
    
    public int Price;
    public int ShotRange;
    public string Name;
    public Projectile ProjectileType;
    public double ShotFrequency;


    public abstract Collider GetNearestEnemyCollider(Vector3 unitPosition, Collider[] enemies);
    private void Start()
    {
        _healthComponent.OnDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        _healthComponent.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }

    public void Fire(Collider enemy)
    {
        Projectile projectile = Instantiate(_projectilePrefab, _projectileSpawn.transform.position, Quaternion.identity);
        projectile.SetTarget(enemy);
    }
}
