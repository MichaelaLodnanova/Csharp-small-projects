using UnityEngine;

public class AggresiveEnemy : Enemy
{
    private Collider target = null;
    private int targetRange = 10;

    public override void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "castle" || tag == "tower")
        {
            collision.collider.GetComponent<HealthComponent>().HealthValue -= Damage;
            HandleDeath();
            SpawnParticles();
        }
    }

    // Update is called once per frame
    private void findTarget(Collider[] colliders)
    {
        foreach(Collider collider in colliders)
        {
            if (collider.tag == "tower")
            {
                target = collider; break;
            }
        }
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, targetRange);
        findTarget(hitColliders);


        if (target == null)
        {
            _movementComponent.MoveAlongPath(); return;
        }

        else
        {
            _movementComponent.MoveTowards(target.transform);
        }

    }
}
