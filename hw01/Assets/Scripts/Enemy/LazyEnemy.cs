using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LazyEnemy : Enemy
{
    float time = 0;
    const float MovingTime = 5;
    const float WaitingTime = 1;
    bool IsMoving = true;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= MovingTime && IsMoving)
        {
            _movementComponent.CancelMovement();
            time = 0;
            IsMoving = false;
        }
        if (time >= WaitingTime && !IsMoving)
        {
            _movementComponent.MoveAlongPath();
            time = 0;
            IsMoving = true;
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("castle"))
        {
            collision.collider.GetComponent<HealthComponent>().HealthValue -= Damage;
            HandleDeath();
            SpawnParticles();
        }
        else if (collision.gameObject.CompareTag("tower"))
        {
            collision.collider.GetComponent<HealthComponent>().HealthValue -= Damage * 2;
            HandleDeath();
            SpawnParticles();
        }
    }
}
