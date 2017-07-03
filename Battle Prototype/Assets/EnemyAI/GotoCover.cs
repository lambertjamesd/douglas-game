using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GotoCover : State
{
    public Sight targetSight;
    public Pathfinder pathfinder;
    public State next;

    public override IState UpdateState(float deltaTime)
    {
        Collider2D target = targetSight.GetVisibleObject();
        if (target != null)
        {
            Vector3 targetPos = target.transform.position;
            Vector3 currentPos = transform.position;
            float targetDistance = (currentPos - targetPos).sqrMagnitude;

            System.Func<Vector3, bool> filter = pos =>
            {
                return targetDistance > (pos - targetPos).sqrMagnitude;
            };

            var split = pathfinder.GetPathFinding().FindCover(targetPos, 0.5f);

            if (!pathfinder.PathToNearest(split.Where(filter)))
            {
                pathfinder.PathToNearest(split.Where(pos => !filter(pos)));
            }
        }

        return next;
    }
}
