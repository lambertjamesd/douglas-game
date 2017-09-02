using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckReload : State {
    public State reloadState;
    public State defaultState;
    public Gun gunState;

    public override IState UpdateState(float deltaTime)
    {
        if (gunState.GetShotsLeft() == 0)
        {
            return reloadState;
        }
        else
        {
            return defaultState;
        }
    }
}
