using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public Damageable damageable;
    public Sight sight;
    public DefaultMovement movement;
    public PatrolParameters patrolParameters;
    public float chargeSpeed;

    // Use this 
    public void Start()
    {
        StartCoroutine(DoLogic());
    }

    public IEnumerator DoLogic()
    {
        while (!damageable.IsDead)
        {
            yield return Patrol.PatrolUntilSight(movement, patrolParameters, sight);

            Collider2D target = sight.GetVisibleObject();

            while (sight.canSeeObject(target.gameObject))
            {
                Vector2 offset = target.transform.position - transform.position;
                movement.TargetVelocity = offset.normalized * chargeSpeed;
                yield return null;
            }
        }
    }
}