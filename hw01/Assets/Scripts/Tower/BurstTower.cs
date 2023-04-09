using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BurstTower : Tower
{
    private Collider targetEnemy = null;
    private double fireTimer = 0;
    private double shotDelay = 0.2;

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
                StartCoroutine(FireDelay());
                fireTimer = ShotFrequency - shotDelay;
            }
        }
    }

    private IEnumerator FireDelay()
    {
        Fire(targetEnemy);
        yield return new WaitForSeconds(0.2f);
        Fire(targetEnemy);
    }

    public override Collider GetNearestEnemyCollider(Vector3 unitPosition, Collider[] enemies)
    {
        Collider bestEnemy = null;
        int lives = 0;
        foreach (var enemy in enemies)
        {
            if (enemy.tag != "enemy")
            {
                continue;
            }
            int curr_lives = enemy.GetComponent<HealthComponent>().HealthValue;
            if (curr_lives > lives)
            {
                lives = curr_lives;
                bestEnemy = enemy; 
            }
        }
        return bestEnemy;
    }
}
