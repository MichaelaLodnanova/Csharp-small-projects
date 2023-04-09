using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTower : Tower
{
    private Collider target = null;
    private double fireTimer = 0;

    public override Collider GetNearestEnemyCollider(Vector3 unitPosition, Collider[] enemies)
    {
        Collider bestEnemy = null;
        float? bestDistance = null;
        foreach (Collider enemy in enemies)
        {
            if (enemy.tag != "enemy")
            {
                continue;
            }
            float distance = Vector3.Distance(unitPosition, enemy.transform.position);
            if (!bestDistance.HasValue || distance < bestDistance)
            {
                bestDistance = distance;
                bestEnemy = enemy;
            }
        }
        return bestEnemy;
    }


    void Update()
    {
        Vector3 towerPosition = transform.position;
        if (target == null) {
            Collider[] hitColliders = Physics.OverlapSphere(towerPosition, ShotRange);
            target = GetNearestEnemyCollider(towerPosition, hitColliders);
        }
        else if (Vector3.Distance(towerPosition, target.transform.position) > ShotRange)
        {
            target = null;
        }
        else
        {
            var head = transform.Find(_objectToPan.name);
            head.LookAt(target.transform);
            
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0)
            {
                Fire(target);
                fireTimer = ShotFrequency;
            }
        }
    }
}