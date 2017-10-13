using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootOnSight : MonoBehaviour {
    public Damageable damageable;
    public Sight sight;
    public DefaultMovement movement;
    public PatrolParameters patrolParameters;
    public Bow shootFrom; 
    public float targetOffset;
    public float lineUpSpeed;
    public GunStats gunStats;
    public float preFireDelay = 0.5f;
    public float postFireDelay = 1.0f;
    public VariableStore variableStore;
    public Transform reloadLocation;
    public AnimationController animation;
    public GameObject shotBy;

    public void Start()
    {
        StartCoroutine(DoLogic());
        damageable.FilterDamage(OnDamage, false);
    }

    public DamageSource OnDamage(DamageSource source, Damageable damageable)
    {
        Vector2 pos2D = transform.position;
        movement.SetDirection(source.DamagePosition - pos2D);
        return source;
    }

    public IEnumerator DoLogic()
    {
        while (!damageable.IsDead)
        {
            yield return Patrol.PatrolUntilSight(movement, patrolParameters, sight);

            movement.LockRotation();

            Collider2D target = sight.GetVisibleObject();

            if (target != null)
            {
                yield return LineUpShot.LineUp(target.transform, shootFrom.transform, movement, sight, targetOffset, lineUpSpeed);

                if (sight.canSeeObject(target.gameObject))
                {
                    if (animation != null)
                    {
                        animation.SetTigger("Firing");
                    }
                    yield return AsyncUtil.Pause(preFireDelay);
                    shootFrom.Fire(gunStats);
                    gunStats.SetShotsLeft(variableStore, gunStats.GetShotsLeft(variableStore) - 1);
                    yield return AsyncUtil.Pause(postFireDelay);

                    if (gunStats.GetShotsLeft(variableStore) <= 0)
                    {
                        yield return ReloadGun.ReloadAnimation(gunStats, reloadLocation, variableStore);
                    }
                }
            }

            movement.UnlockRotation();
        }
    }
}
