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
            Vector2 moveTowards = Vector2.zero;
            float chargeTime = 0.0f;

            using (DamageListener damageListener = new DamageListener(damageable, (source, damageable) =>
            {
                moveTowards = -source.Direction;
                movement.SetDirection(moveTowards);
                movement.TargetVelocity = moveTowards * chargeSpeed;
                Vector2 position = transform.position;
                chargeTime = Vector2.Dot(moveTowards, source.DamagePosition - position) / chargeSpeed;
                return source;
            }, false))
            {
                yield return AsyncUtil.Race(new IEnumerator[] {
                    AsyncUtil.Sequence(new IEnumerator[] {
                        AsyncUtil.Race(new IEnumerator[] {
                            Patrol.PatrolForever(movement, patrolParameters),
                            AsyncUtil.WaitUntil(() => chargeTime > 0.0f),
                        }),
                        AsyncUtil.WaitUntil(() => {
                            chargeTime -= Time.deltaTime;
                            return chargeTime <= 0.0f;
                        }),
                    }),
                    AsyncUtil.WaitUntil(() => sight.GetVisibleObject() != null),
                });
            }
            Collider2D target = sight.GetVisibleObject();

            while (target != null && sight.canSeeObject(target.gameObject))
            {
                Vector2 offset = target.transform.position - transform.position;
                movement.TargetVelocity = offset.normalized * chargeSpeed;
                yield return null;
            }
        }
    }
}