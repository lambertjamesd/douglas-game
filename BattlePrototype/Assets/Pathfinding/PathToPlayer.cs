using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathToPlayer : State {
    public Pathfinder pathfinder;
    public State pathState;

    public override IState UpdateState(float deltaTime)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            pathfinder.PathTo(player.transform.position);
            return pathState;
        }
        else
        {
            return null;
        }
    }
}
