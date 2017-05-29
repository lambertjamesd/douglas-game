using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : IState
{
    private State next;
    private IEnumerator script;

    public Interact(State next, IEnumerator script)
    {
        this.next = next;
        this.script = script;
    }

    public void StateBegin()
    {

    }

    public void StateEnd()
    {

    }

    public IState UpdateState(float deltaTime)
    {
        if (script != null && script.MoveNext())
        {
            return null;
        } else
        {
            return next;
        }
    }
}
