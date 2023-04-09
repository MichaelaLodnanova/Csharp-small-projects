using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    public override void FireDamage()
    {
        target.GetComponent<HealthComponent>().HealthValue -= Damage;
    }

    void Start()
    {
        Destroy(gameObject, 5);
    }

    void Update()
    {
        if (target == null) { 
            Destroy(gameObject); return;
        }
        Vector3 direction = target.transform.position - transform.position;
        float distanceThisFrame = Speed * Time.deltaTime;
        transform.Translate(direction.normalized * distanceThisFrame * 20, Space.World);
    }
}
