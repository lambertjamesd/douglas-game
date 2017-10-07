using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoCoroutine : IState {
    private AsyncManager enumerator;
    private IState after;
    public DoCoroutine(IEnumerator enumerator, IState after)
    {
        this.enumerator = new AsyncManager(enumerator);
        this.after = after;
    }

    public void StateBegin()
    {

    }

    public IState UpdateState(float deltaTime)
    {
        if (enumerator.Next())
        {
            return null;
        }
        else
        {
            return after;
        }
    }

    public void StateEnd()
    {

    }
}
