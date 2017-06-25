using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            pathfinder.PathToNearest(pathfinder.pathfinding.FindCover(target.transform.position, 0.5f));
        }

        return next;
    }
}
