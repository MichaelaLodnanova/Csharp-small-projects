using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjektile : Projectile
{
    private int explosionRange = 5;

    private void HurtEnemies(Collider[] enemies)
    {
        foreach (Collider enemy in enemies) 
        {
            if (enemy.tag != "enemy")
            {
                continue;
            }
            enemy.GetComponent<HealthComponent>().HealthValue -= Damage;
        }
    }
    public override void FireDamage()
    {
        target.GetComponent<HealthComponent>().HealthValue -= Damage;
        Collider[] enemies = Physics.OverlapSphere(target.transform.position, explosionRange);
        HurtEnemies(enemies);
    }

    void Start()
    {
        Destroy(gameObject, 4);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(target); return;
        }
        Vector3 direction = target.transform.position - transform.position;
        float distanceThisFrame = Speed * Time.deltaTime;
        transform.Translate(direction.normalized * distanceThisFrame * 20, Space.World);
    }
}
