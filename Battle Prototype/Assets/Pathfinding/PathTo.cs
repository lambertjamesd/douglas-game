using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTo : State {
    public Pathfinder pathFinder;
    public State next;

    public override IState UpdateState(float deltaTime)
    {
        if (pathFinder.IsActive)
        {
            return null;
        }
        else
        {
            return next;
        }
    }
}
