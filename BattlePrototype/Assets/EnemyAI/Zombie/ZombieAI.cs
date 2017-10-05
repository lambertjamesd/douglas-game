using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZombieAI : MonoBehaviour
{
    public Damageable damageable;
    public Sight sight;
    public DefaultMovement movement;
    public PatrolParameters patrolParameters;
    public float chargeSpeed;
    private GameObject externalTarget;
    public float alertRadius = 3.0f;

    // Use this 
    public void Start()
    {
        StartCoroutine(DoLogic());
    }

    public void AlertNearbyZombies(GameObject target)
    {
        Vector3 pos = transform.position;
        foreach (GameObject zombie in GameObject.FindGameObjectsWithTag("Zombie"))
        {
            Vector2 offset = pos - zombie.transform.position;

            if (zombie != gameObject && offset.sqrMagnitude <= alertRadius * alertRadius)
            {
                ZombieAI otherAI = zombie.GetComponent<ZombieAI>();

                if (otherAI != null)
                {
                    otherAI.externalTarget = target;
                }
            }
        }
    }

    public IEnumerator DoLogic()
    {
        while (!damageable.IsDead)
        {
            Vector2 moveTowards = Vector2.zero;
            float chargeTime = 0.0f;

            using (DamageListener damageListener = new DamageListener(damageable, (source, damageable) =>
            {
                moveTowards = -source.Direction;
                movement.TargetVelocity = moveTowards * chargeSpeed;
                Vector2 position = transform.position;
                chargeTime = Vector2.Dot(moveTowards, source.DamagePosition - position) / chargeSpeed;
                return source;
            }, false))
            {
                AsyncManager patrol = new AsyncManager(Patrol.PatrolForever(movement, patrolParameters));

                while (patrol.Next())
                {
                    yield return null;

                    if (sight.GetVisibleObject() != null || externalTarget != null)
                    {
                        break;
                    }

                    if (chargeTime > 0.0f)
                    {
                        patrol = new AsyncManager(AsyncUtil.Pause(chargeTime));
                        chargeTime = 0.0f;
                    }
                }
            }

            GameObject target = null;

            if (externalTarget)
            {
                target = externalTarget;
                externalTarget = null;
            }

            Collider2D sightTest = sight.GetVisibleObject();
            
            if (sightTest != null)
            {
                target = sightTest.gameObject;
                AlertNearbyZombies(target);
            }

            while (target != null && sight.canSeeObject(target))
            {
                Vector2 offset = target.transform.position - transform.position;
                movement.TargetVelocity = offset.normalized * chargeSpeed;
                yield return null;
            }

            movement.TargetVelocity = Vector2.zero;
            yield return new WaitForSeconds(1.0f);
        }
    }
}