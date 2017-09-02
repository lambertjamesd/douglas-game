using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayState : State  {

    public float duration;
    public State nextState;
    private float currentDuration;

    public override void StateBegin()
    {
        currentDuration = duration;
    }
    
    public override IState UpdateState(float deltaTime)
    {
        currentDuration -= deltaTime;

        if (currentDuration < 0.0f)
        {
            return nextState;
        }
        else
        {
            return null;
        }
    }
}
