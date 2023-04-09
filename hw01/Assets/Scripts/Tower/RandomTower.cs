using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTower : Tower
{
    private Collider targetEnemy = null;
    private double fireTimer = 0;

    public override Collider GetNearestEnemyCollider(Vector3 unitPosition, Collider[] enemies)
    {
        Collider bestEnemy = null;
        int idx = Random.Range(0, enemies.Length - 1);
        bestEnemy = enemies[idx];
        if (bestEnemy.tag != "enemy")
        {
            bestEnemy = null;
        }
        return bestEnemy;
    }

    public void MakeDecision()
    {
        int decision = Random.Range(0, 10);
        if (decision <= 2)
        {
            Fire(targetEnemy);
            Fire(targetEnemy);
        }
        else if (decision > 4 && decision <= 10)
        {
            Fire(targetEnemy);
        }
        
    }
    void Update()
    {
        Vector3 towerPosition = transform.position;
        if (targetEnemy == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(towerPosition, ShotRange);
            targetEnemy = GetNearestEnemyCollider(towerPosition, hitColliders);
        }
        else if (Vector3.Distance(towerPosition, targetEnemy.transform.position) > ShotRange)
        {
            targetEnemy = null;
        }
        else
        {
            var head = transform.Find(_objectToPan.name);
            head.LookAt(targetEnemy.transform);

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0)
            {
                MakeDecision();
                fireTimer = ShotFrequency;
            }
            
        }
    }
}
