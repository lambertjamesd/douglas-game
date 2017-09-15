using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindCoverEnemy : MonoBehaviour
{
    public Damageable damageable;
    public Sight sight;
    public DefaultMovement movement;
    public PatrolParameters patrolParameters;
    public Bow shootFrom;
    public float targetOffset;
    public float lineUpSpeed;
    public GunStats gunStats;
    public float postFireDelay = 1.0f;
    public VariableStore variableStore;
    public Transform reloadLocation;
    public Pathfinder pathfinder;

    public void Start()
    {
        StartCoroutine(DoLogic());
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
                    shootFrom.Fire(gunStats);
                    gunStats.SetShotsLeft(variableStore, gunStats.GetShotsLeft(variableStore) - 1);
                    yield return AsyncUtil.Pause(postFireDelay);

                    if (gunStats.GetShotsLeft(variableStore) <= 0)
                    {
                        var possibleCover = pathfinder.GetPathFinding().FindCover(target.transform.position, 0.5f);

                        pathfinder.PathToNearest(possibleCover);

                        movement.UnlockRotation();

                        var previousLocation = transform.position;

                        yield return pathfinder.FollowPath();

                        movement.TargetVelocity = Vector2.zero;

                        yield return ReloadGun.ReloadAnimation(gunStats, reloadLocation, variableStore);

                        pathfinder.PathTo(previousLocation);

                        yield return pathfinder.FollowPath();

                        movement.LockRotation();
                    }
                }
            }

            movement.UnlockRotation();
        }
    }
}
